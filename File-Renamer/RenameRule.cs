namespace File_Renamer
{
    partial class Program
    {
        public interface RenameRule
        {
            string ApplyRule(string FilePath);
        }
    }
}
