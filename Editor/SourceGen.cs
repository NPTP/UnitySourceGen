using System;
using System.IO;
using System.Linq;
using NPTP.UnitySourceGen.Editor.Enums;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Modifiable;
using NPTP.UnitySourceGen.Editor.ScriptWriting;
using NPTP.UnitySourceGen.Editor.Syntax;
using UnityEngine;

namespace NPTP.UnitySourceGen.Editor
{
    /// <summary>
    /// Entry point for source generation. Everything is created here and then configured fluently on
    /// itself, e.g.
    /// <code>
    /// SourceGen.WriteToPath("Assets/MyGame.Generated/ISW.cs",
    ///     SourceGen.NewStaticClass("ISW", AccessModifier.Public)
    ///         .InNamespace("MyGame")
    ///         .WithMethod(SourceGen.NewMethod("GetPlayer").Public().Static().Returning("InputPlayer")
    ///             .Taking(GeneratableParameter.Of&lt;int&gt;("playerID"))
    ///             .Expression("Runtime.GetPlayer(playerID)")));
    /// </code>
    /// </summary>
    public static class SourceGen
    {
        #region Creation

        public static GeneratableClass NewClass(string name, AccessModifier accessModifier) => new(name, accessModifier, isStatic: false);
        public static GeneratableClass NewStaticClass(string name, AccessModifier accessModifier) => new(name, accessModifier, isStatic: true);
        public static GeneratableStruct NewStruct(string name, AccessModifier accessModifier) => new(name, accessModifier);
        public static GeneratableEnum NewEnum(string name, AccessModifier accessModifier) => new(name, accessModifier);

        public static GeneratableMethod NewMethod(string name) => new(name);
        public static GeneratableEvent NewEvent(string name) => new(name);
        public static GeneratableField NewField(string name, TypeRef fieldType) => new(name, fieldType);
        public static GeneratableProperty NewProperty(string name, TypeRef propertyType) => new(name, propertyType);

        public static GeneratableCodeChunk NewCodeChunk() => new();

        /// <summary>A file that can hold several types, across several namespaces if needed.</summary>
        public static GeneratableFile NewFile() => new();

        #endregion

        #region Writing

        /// <summary>
        /// Write any generatable - class, struct or enum - to a path inside the project's Assets folder,
        /// e.g. "Assets/MyGame.Generated/PlayerActions.cs". Unlike the overloads that locate a file from an
        /// existing type, this can create files for types that do not exist yet, which is the normal case
        /// when generating a set of types that reference each other.
        /// <para>
        /// Returns whether the file was written, was already up to date, or failed. Nothing is logged on
        /// success, so a generator producing many files can report one summary of its own.
        /// </para>
        /// </summary>
        public static ScriptWriteResult WriteToPath(string pathInsideAssets, GeneratableBase generatable)
        {
            if (generatable == null)
            {
                Debug.LogError("Cannot write a null generatable.");
                return ScriptWriteResult.Failed;
            }

            return ScriptWriter.Write(new UnityAssetPath(pathInsideAssets), generatable.GenerateStringRepresentation());
        }

        /// <summary>Write a multi-type file to a path inside the project's Assets folder.</summary>
        public static ScriptWriteResult WriteToPath(string pathInsideAssets, GeneratableFile generatableFile)
        {
            if (generatableFile == null)
            {
                Debug.LogError("Cannot write a null file.");
                return ScriptWriteResult.Failed;
            }

            return ScriptWriter.Write(new UnityAssetPath(pathInsideAssets), generatableFile.GenerateStringRepresentation());
        }

        /// <summary>
        /// Write raw file contents to a path inside the project's Assets folder, for anything the
        /// generatable types do not cover yet.
        /// </summary>
        public static ScriptWriteResult WriteToPath(string pathInsideAssets, string contents)
        {
            return ScriptWriter.Write(new UnityAssetPath(pathInsideAssets), contents);
        }

        public static bool ReplaceClassInAssetsScriptFile(Type classType, GeneratableClass generatableClass)
        {
            generatableClass.InNamespace(classType.Namespace);
            return ScriptWriter.TryReplaceClass(classType, generatableClass);
        }

        #endregion

        #region Modification

        public static ModifiableScript GetScriptToModify<T>()
        {
            if (AssetsScriptGetter.TryGetSystemFilePathToScriptInAssets(typeof(T), out UnityAssetPath unityAssetPath))
            {
                try
                {
                    return new ModifiableScript(File.ReadAllLines(unityAssetPath.SystemPath).ToList(), unityAssetPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not read script: {e.Message}");
                }
            }

            return null;
        }

        #endregion
    }
}
