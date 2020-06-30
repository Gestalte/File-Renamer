using System;

namespace File_Renamer
{
    public class Filter
    {
        private readonly IFilterRule filterRule;

        public Filter(IFilterRule filterRule)
        {
            this.filterRule = filterRule ?? throw new ArgumentNullException(nameof(filterRule));
        }

        public string[] FilterFilepaths(string[] filePaths)
             => this.filterRule.ApplyRule(filePaths);
    }
}
