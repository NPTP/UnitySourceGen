using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    internal static class ExtensionsCommon
    {
        internal static bool CheckValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("Tried to add a null- or empty-named element to a generatable type definition.");
                return false;
            }
            
            return true;
        }
    }
}