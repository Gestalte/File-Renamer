namespace File_Renamer
{
    public interface IRenameRule
    {
        string ApplyRule(string FilePath);
    }
}
