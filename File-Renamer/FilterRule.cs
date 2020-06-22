namespace File_Renamer
{
    partial class Program
    {
        public interface FilterRule
        {
            string[] Filter(string[] FilePaths);
        }
    }
}
