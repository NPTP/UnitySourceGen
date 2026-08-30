namespace NPTP.UnitySourceGen.Editor.ScriptWriting
{
    /// <summary>
    /// What a write actually did. Generators that produce many files can use this to report a single
    /// summary rather than one console entry per file, and to skip an AssetDatabase refresh entirely when
    /// nothing changed.
    /// </summary>
    public enum ScriptWriteResult
    {
        Failed = 0,

        /// <summary>The file was created or its contents differed and were overwritten.</summary>
        Written,

        /// <summary>The file already had exactly this content, so it was left alone.</summary>
        Unchanged
    }
}
