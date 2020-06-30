namespace File_Renamer
{
    public interface IFilterRule
    {
        string[] ApplyRule(string[] FilePath);
    }
}
