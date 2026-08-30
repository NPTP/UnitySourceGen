using System;
using System.IO;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.ScriptWriting
{
    public struct UnityAssetPath
    {
        private const string ASSETS = "Assets";

        /// <summary>
        /// Path inside the project folder's top "Assets" folder. Used by AssetDatabase in Unity internally,
        /// so the slashes may not correspond to default OS standards, rather to Unity's "all forward slashes" convention.
        /// </summary>
        public string AssetsPath { get; }

        /// <summary>
        /// Complete system path on your machine. Slashes are fixed for the platform on which this is executing.
        /// </summary>
        public string SystemPath { get; }

        public bool IsValid => !string.IsNullOrEmpty(AssetsPath);

        public UnityAssetPath(string assetsPath)
        {
            assetsPath = (assetsPath ?? string.Empty).Replace('\\', '/').TrimStart('/');

            if (!assetsPath.Equals(ASSETS, StringComparison.Ordinal) && !assetsPath.StartsWith(ASSETS + "/", StringComparison.Ordinal))
            {
                Debug.LogError($"\"{assetsPath}\" is not a path inside the project's Assets folder.");
                AssetsPath = null;
                SystemPath = null;
                return;
            }

            AssetsPath = assetsPath;

            // Only the leading "Assets" segment is removed. Replacing every occurrence would corrupt any
            // path with a folder such as "MyAssets" in it.
            SystemPath = Path.GetFullPath(Application.dataPath + assetsPath.Substring(ASSETS.Length));
        }

        public override string ToString() => AssetsPath;
    }
}
