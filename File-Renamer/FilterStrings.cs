using System;
using System.Collections.Generic;

namespace File_Renamer
{
    public class FilterStrings
    {
        private readonly List<Filter> filters;

        public FilterStrings(List<Filter> filters)
        {
            this.filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }

        public string[] Filter(string[] inputList)
        {
            string[] localList = inputList; // Doesn't mutate array.

            foreach (var filter in filters)
            {
                localList = filter.FilterFilepaths(localList);
            }

            return localList;
        }
    }
}
