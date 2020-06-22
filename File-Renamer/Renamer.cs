using System;

namespace File_Renamer
{
    partial class Program
    {
        public class Renamer
        {
            private readonly RenameRule renameRule;

            public Renamer(RenameRule renameRule)
            {
                if (renameRule == null)
                    throw new ArgumentNullException(nameof(renameRule));

                this.renameRule = renameRule;
            }

            public string Rename(string FilePath)
                => this.renameRule.ApplyRule(FilePath);
        }
    }
}
