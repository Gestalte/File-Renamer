using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace File_Renamer
{
    public class RemovePeriods : IRenameRule
    {
        public string ApplyRule(string FilePath)        
            =>Path.GetDirectoryName(FilePath) + Path.GetFileNameWithoutExtension(FilePath).Replace(".", "") + Path.GetExtension(FilePath);        
    }
}
