namespace NPTP.UnitySourceGen.Editor.Generatable.Other
{
    public class Comment
    {
        private readonly string comment;
        
        public Comment(string comment)
        {
            this.comment = comment;
        }
        
        public override string ToString() => $"// {comment}";
        public bool Matches(string line) => line.Contains(ToString());

        public static implicit operator string(Comment comment) => comment.ToString();
    }
}