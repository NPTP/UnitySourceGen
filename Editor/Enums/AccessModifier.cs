using System;

namespace NPTP.UnitySourceGen.Editor.Enums
{
    public enum AccessModifier
    {
        Public = 0,
        Private,
        Protected,
        Internal,
        ProtectedInternal,
        PrivateProtected,
        
        /// <summary>
        /// Potentially unsupported access modifier in Unity's version of C#
        /// </summary>
        File
    }
    
    public static class AccessModifierExtensions
    {
        public static string AsString(this AccessModifier accessModifier)
        {
            return accessModifier switch
            {
                AccessModifier.Public => "public",
                AccessModifier.Private => "private",
                AccessModifier.Protected => "protected",
                AccessModifier.Internal => "internal",
                AccessModifier.ProtectedInternal => "protected internal",
                AccessModifier.PrivateProtected => "private protected",
                AccessModifier.File => "file",
                _ => throw new ArgumentOutOfRangeException(nameof(accessModifier), accessModifier, null)
            };
        }
    }
}
