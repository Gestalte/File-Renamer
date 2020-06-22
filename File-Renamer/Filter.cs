using System;

namespace File_Renamer
{
    partial class Program
    {
        public class Filter
        {
            private readonly FilterRule filterRule;

            public Filter(FilterRule filterRule)
            {
                if (filterRule == null)
                    throw new ArgumentNullException(nameof(filterRule));

                this.filterRule = filterRule;
            }

            public string[] FilterFilepaths(string[] filePaths)
                 => this.filterRule.Filter(filePaths);
        }
    }
}
