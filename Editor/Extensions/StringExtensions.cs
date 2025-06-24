namespace NPTP.UnitySourceGen.Editor.Extensions
{
    public static class StringExtensions
    {
        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            
            char firstChar = char.ToUpper(s[0]);
            if (s.Length == 1) return firstChar.ToString();
            return  firstChar + s[1..];
        }
        
        public static string LowercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            
            char firstChar = char.ToLower(s[0]);
            if (s.Length == 1) return firstChar.ToString();
            return  firstChar + s[1..];
        }
    }
}