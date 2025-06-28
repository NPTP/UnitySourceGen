using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPTP.UnitySourceGen.Editor.Extensions;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Generatable.Directives;
using NPTP.UnitySourceGen.Editor.Generatable.Other;
using NPTP.UnitySourceGen.Editor.ScriptWriting;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor.Modifiable
{
    public class ModifiableScript
    {
        private readonly List<string> scriptLines;
        private readonly UnityAssetPath unityAssetPath;

        internal ModifiableScript(List<string> scriptLines, UnityAssetPath unityAssetPath)
        {
            this.scriptLines = scriptLines;
            this.unityAssetPath = unityAssetPath;
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
            
            scriptLines.Insert(0, a);
        }
        
        internal void AddComment(string comment)
        {
            scriptLines.Add(new Comment(comment));
        }

        internal void RemoveLinesContaining(string content)
        {
            for (int i = 0; i < scriptLines.Count;)
            {
                if (scriptLines[i].Contains(content))
                    scriptLines.RemoveAt(i);
                else
                    i++;
            }
        }
        
        internal void PutCodeChunkInRegion(string regionName, GeneratableCodeChunk codeChunk, bool replaceExistingCodeInRegion)
        {
            int regionStartLineIndex = -1;
            int regionEndLineIndex = -1;
            
            for (int i = 0; i < scriptLines.Count; i++)
            {
                string scriptLine = scriptLines[i];
                if (scriptLine.ContainsAll("#region", regionName))
                    regionStartLineIndex = i;
                else if (scriptLine.Contains("#endregion"))
                    regionEndLineIndex = i;
                
                if (regionStartLineIndex >= 0 && regionEndLineIndex > 0)
                    break;
            }

            if (regionStartLineIndex == -1 || regionEndLineIndex == -1)
            {
                return;
            }

            int regionInteriorStartIndex = regionStartLineIndex + 1;
            
            if (replaceExistingCodeInRegion && regionEndLineIndex > regionStartLineIndex + 1)
            {
                scriptLines.RemoveRange(regionInteriorStartIndex, regionEndLineIndex - regionInteriorStartIndex);
            }

            codeChunk.Indent = scriptLines[regionInteriorStartIndex].GetIndentLevel();
            scriptLines.InsertRange(regionInteriorStartIndex, codeChunk.GenerateStringRepresentationLines());
        }
    }
}
