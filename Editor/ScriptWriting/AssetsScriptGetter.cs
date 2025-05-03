using System;
using UnityEditor;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.UnitySourceGen.Editor.ScriptWriting
{
    internal static class AssetsScriptGetter
    {
        internal static string GetSystemFilePathToScriptInAssets<T>() => GetSystemFilePathToScriptInAssets(typeof(T));
        internal static string GetSystemFilePathToScriptInAssets(Type type) => GetSystemFilePathToScriptInAssetsCommon(type);
        private static string GetSystemFilePathToScriptInAssetsCommon(Type type)
        {
            string[] guids = AssetDatabase.FindAssets("t:Script a:assets");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (scriptAsset == null)
                {
                    continue;
                }

                if (IsEnum(type, scriptAsset) ||
                    scriptAsset.GetClass() == type ||
                    type.IsAssignableFrom(scriptAsset.GetClass()) ||
                    IsStruct(type, scriptAsset))
                    // TODO: record support
                    // || IsRecord(type, scriptAsset)
                {
                    string path = Application.dataPath + assetPath.Replace("Assets", string.Empty);
                    return path;
                }
            }

            return string.Empty;
        }

        private static bool IsEnum(Type type, MonoScript scriptAsset)
        {
            return type.IsEnum && scriptAsset.text.Contains($"enum {type.Name}");
        }

        private static bool IsStruct(Type type, MonoScript scriptAsset)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum && scriptAsset.text.Contains($"struct {type.Name}");
        }

        // TODO: record support
        private static bool IsRecord(Type type, MonoScript scriptAsset)
        {
            return type.IsClass && scriptAsset.text.Contains($"record {type.Name}");
        }
    }
}