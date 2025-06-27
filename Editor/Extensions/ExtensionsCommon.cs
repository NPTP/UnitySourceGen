using NPTP.UnitySourceGen.Editor.Options;
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
        
        internal static bool CheckValidName(NameSyntax nameSyntax)
        {
            if (!nameSyntax.IsValid())
            {
                Debug.LogWarning("Tried to add a null- or empty-named element to a generatable type definition.");
                return false;
            }
            
            return true;
        }
    }
}