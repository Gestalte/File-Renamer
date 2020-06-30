using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace File_Renamer
{
    public class PdfOnly : IFilterRule
    {
        public string[] ApplyRule(string[] FilePath)
            => FilePath.Where(f => f.Contains(".pdf")).ToArray();     
    }
}
