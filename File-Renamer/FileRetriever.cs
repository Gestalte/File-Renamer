using System.IO;

namespace File_Renamer
{
    public class FileRetriever : IFileProvider
    {
        public string[] GetAllFiles()
            => Directory.GetFiles(Directory.GetCurrentDirectory());
    }
}
