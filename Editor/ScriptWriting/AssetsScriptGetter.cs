using System;
using UnityEditor;

namespace NPTP.UnitySourceGen.Editor.ScriptWriting
{
    internal static class AssetsScriptGetter
    {
        internal static bool TryGetSystemFilePathToScriptInAssets<T>(out UnityAssetPath unityAssetPath) => TryGetSystemFilePathToScriptInAssets(typeof(T), out unityAssetPath);
        internal static bool TryGetSystemFilePathToScriptInAssets(Type type, out UnityAssetPath unityAssetPath) => GetSystemFilePathToScriptInAssetsCommon(type, out unityAssetPath);
        private static bool GetSystemFilePathToScriptInAssetsCommon(Type type, out UnityAssetPath unityAssetPath)
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
                    unityAssetPath = new UnityAssetPath(assetPath);
                    return true;
                }
            }

            unityAssetPath = default;
            return false;
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