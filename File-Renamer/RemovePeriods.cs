using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace File_Renamer
{
    public class RemovePeriods : RenameRule
    {
        public string ApplyRule(string FilePath)        
            =>Path.GetDirectoryName(FilePath) + Path.GetFileNameWithoutExtension(FilePath).Replace(".", "") + Path.GetExtension(FilePath);        
    }
}
