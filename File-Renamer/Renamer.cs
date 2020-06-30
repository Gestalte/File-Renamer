using System;

namespace File_Renamer
{
    public class Renamer
    {
        private readonly IRenameRule renameRule;

        public Renamer(IRenameRule renameRule)
        {
            this.renameRule = renameRule ?? throw new ArgumentNullException(nameof(renameRule));
        }

        public string Rename(string FilePath)
            => this.renameRule.ApplyRule(FilePath);
    }
}
