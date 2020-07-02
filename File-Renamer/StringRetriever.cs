using System;

namespace File_Renamer
{
    public class StringRetriever
    {
        private readonly IFileProvider fileProvider;

        public StringRetriever(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }

        public string[] GetFileList()
            => fileProvider.GetAllFiles();
    }
}
