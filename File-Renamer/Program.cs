using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace File_Renamer
{
    partial class Program
    {
        static void Main(string[] args) // Composition Root
        {
            var consoleWriter = new ConsoleWriter();

            try
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

                IEnumerable<string> filterRuleNames = Configuration.GetSection("FilterRules")
                    .GetChildren()
                    .Select(x => x.Value);

                IEnumerable<string> renameRulesNames = Configuration.GetSection("RenameRules")
                    .GetChildren()
                    .Select(x => x.Value);

                List<IFilterRule> filterRules = new List<IFilterRule>();

                foreach (string filterRule in filterRuleNames)
                {
                    Type type = Type.GetType(filterRule, throwOnError: true);
                    IFilterRule fr = (IFilterRule)Activator.CreateInstance(type);
                    filterRules.Add(fr);
                }

                List<IRenameRule> renameRules = new List<IRenameRule>();

                foreach (string renameRule in renameRulesNames)
                {
                    Type type = Type.GetType(renameRule, throwOnError: true);
                    IRenameRule rr = (IRenameRule)Activator.CreateInstance(type);
                    renameRules.Add(rr);
                }

                List<Filter> filters = new List<Filter>();

                foreach (IFilterRule filterRule in filterRules)
                    filters.Add(new Filter(filterRule));

                List<Renamer> renamers = new List<Renamer>();

                foreach (IRenameRule renameRule in renameRules)
                    renamers.Add(new Renamer(renameRule));

                StringRetriever stringRetriever = new StringRetriever(new FileRetriever());

                string[] list = stringRetriever.GetFileList();

                FilterStrings filterStrings = new FilterStrings(filters);

                string[] filteredList = filterStrings.Filter(list);

                RenameFiles renameFiles = new RenameFiles(consoleWriter, renamers);

                renameFiles.Convert(list);

                consoleWriter.WriteColorMessage("Done!", MessageType.success);
            }
            catch (Exception e)
            {
                consoleWriter.WriteColorMessage(e.Message, MessageType.failure);
            }
        }
    }

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

    public class RenameFiles // TODO: split out message writing to better conform to the SRP
    {
        private readonly IMessageWriter messageWriter;
        
        private readonly List<Renamer> renamers;

        public RenameFiles(
            IMessageWriter messageWriter,            
            List<Renamer> renamers)
        {
            
            this.messageWriter = messageWriter ?? throw new ArgumentNullException(nameof(messageWriter));
            
            this.renamers = renamers ?? throw new ArgumentNullException(nameof(renamers));
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

                messageWriter.WriteColorMessage(Path.GetFileName(unchangedFilepath), MessageType.incorrect);
                messageWriter.WriteColorMessage("converted to:", MessageType.information);
                messageWriter.WriteColorMessage(Path.GetFileName(mutatingFilepath), MessageType.correct);
                messageWriter.WriteMessage("");
            }
        }
    }
}
