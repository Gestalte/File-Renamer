using System;

namespace File_Renamer
{
    public class FileRenamed
    {
        public readonly string OldName;
        public readonly string NewName;

        public FileRenamed(string oldName, string newName)
        {
            this.OldName = oldName ?? throw new ArgumentNullException(nameof(oldName));
            this.NewName = newName ?? throw new ArgumentNullException(nameof(newName));
        }
    }
}
