using System;
using NPTP.UnitySourceGen.Editor.Modifiable;
using UnityEditor;

namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class ModifiableScriptExtensions
    {
        public static ModifiableScript<T> AddDirective<T>(this ModifiableScript<T> mod, string directive)
        {
            if (!ExtensionsCommon.CheckValidName(directive)) return mod;
            mod.AddDirective(directive);
            return mod;
        }
        
        public static ModifiableScript<T> AddAlias<T>(this ModifiableScript<T> mod, string alias, Type originalType)
        {
            if (!ExtensionsCommon.CheckValidName(alias)) return mod;
            mod.AddAlias(alias, originalType);
            return mod;
        }

        public static ModifiableScript<T> ExecuteModification<T>(this ModifiableScript<T> mod)
        {
            mod.ExecuteModification();
            AssetDatabase.Refresh();
            return mod;
        }
    }
}
