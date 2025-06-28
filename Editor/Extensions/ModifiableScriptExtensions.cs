using System;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Modifiable;
using UnityEditor;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class ModifiableScriptExtensions
    {
        public static ModifiableScript RemoveLinesContaining(this ModifiableScript mod, string content)
        {
            mod.RemoveLinesContaining(content);
            return mod;
        }
        
        public static ModifiableScript AddDirective(this ModifiableScript mod, string directive)
        {
            if (!directive.CheckValidGenerationName()) return mod;
            mod.AddDirective(directive);
            return mod;
        }
        
        public static ModifiableScript AddAlias(this ModifiableScript mod, string alias, Type originalType)
        {
            if (!alias.CheckValidGenerationName()) return mod;
            mod.AddAlias(alias, originalType);
            return mod;
        }
        
        public static ModifiableScript AddComment(this ModifiableScript mod, string comment)
        {
            if (!comment.CheckValidGenerationName()) return mod;
            mod.AddComment(comment);
            return mod;
        }
        
        public static void ExecuteModification(this ModifiableScript mod, bool refreshAssets)
        {
            mod.ExecuteModification();
            if (refreshAssets) AssetDatabase.Refresh();
        }

        public static ModifiableScript PutCodeChunkInRegion(this ModifiableScript mod, string regionName,
            bool replaceExistingCodeInRegion, GeneratableCodeChunk codeChunk)
        {
            mod.PutCodeChunkInRegion(regionName, codeChunk, replaceExistingCodeInRegion);
            return mod;
        }
    }
}
