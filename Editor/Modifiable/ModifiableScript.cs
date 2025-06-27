using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPTP.UnitySourceGen.Editor.Generatable.Directives;
using NPTP.UnitySourceGen.Editor.ScriptWriting;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.Modifiable
{
    public class ModifiableScript<T>
    {
        private readonly List<string> scriptLines;
        private readonly UnityAssetPath unityAssetPath;
        
        internal ModifiableScript()
        {
            if (AssetsScriptGetter.TryGetSystemFilePathToScriptInAssets(typeof(T), out unityAssetPath))
            {
                try
                {
                    scriptLines = File.ReadAllLines(unityAssetPath.SystemPath).ToList();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not read script: {e.Message}");
                    throw;
                }
            }
            else
            {
                throw new Exception($"Could not find script of type {typeof(T)}");
            }
        }

        internal void ExecuteModification()
        {
            try
            {
                File.WriteAllLines(unityAssetPath.SystemPath, scriptLines);
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not execute script modification: {e.Message}");
            }
        }

        internal void AddDirective(string directive)
        {
            Directive d = new Directive(directive);
            if (scriptLines.Any(line => d.Matches(line)))
                return;
            
            scriptLines.Insert(0, d);
        }
        
        internal void AddAlias(string alias, Type originalType)
        {
            Alias a = new Alias(alias, originalType);
            if (scriptLines.Any(line => a.Matches(line)))
                return;
            
            scriptLines.Insert(0, new Alias(alias, originalType));
        }
    }
}
