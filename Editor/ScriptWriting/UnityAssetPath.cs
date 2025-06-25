using System.IO;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.ScriptWriting
{
    public struct UnityAssetPath
    {
        /// <summary>
        /// Path inside the project folder's top "Assets" folder. Used by AssetDatabase in Unity internally,
        /// so the slashes may not correspond to default OS standards, rather to Unity's "all forward slashes" convention.
        /// </summary>
        public string AssetsPath { get; }
        
        /// <summary>
        /// Complete system path on your machine. Slashes are fixed for the platform on which this is executing.
        /// </summary>
        public string SystemPath { get; }

        public UnityAssetPath(string assetsPath)
        {
            AssetsPath = assetsPath;
            SystemPath = Path.GetFullPath(Application.dataPath + assetsPath.Replace("Assets", string.Empty));
        }
    }
}