using System;
using System.Collections.Generic;

namespace File_Renamer
{
    public class RenameFiles
    {
        private readonly List<Renamer> renamers;
        private readonly IEventHandler<FileRenamed> fileRenamedEventHandler;

        public RenameFiles(IEventHandler<FileRenamed> fileRenamedEventHandler, List<Renamer> renamers)
        {
            this.fileRenamedEventHandler = fileRenamedEventHandler
                ?? throw new ArgumentNullException(nameof(fileRenamedEventHandler));
            this.renamers = renamers
                ?? throw new ArgumentNullException(nameof(renamers));
        }

        public void Convert(string[] files)
        {
            foreach (var filepath in files)
            {
                string unchangedFilepath = filepath;
                string mutatingFilepath = filepath;

                foreach (var renamer in renamers)
                {
                    mutatingFilepath = renamer.Rename(mutatingFilepath);
                }

                this.fileRenamedEventHandler.Handle(new FileRenamed(unchangedFilepath, mutatingFilepath));
            }
        }
    }
}
