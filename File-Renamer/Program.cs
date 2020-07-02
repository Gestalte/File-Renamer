using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace File_Renamer
{
    partial class Program
    {
        static void Main()
        {
            ConsoleWriter localConsoleWriter = new ConsoleWriter();

            try
            {
                var compositionRoot = new CompositionRoot();

                string[] list = compositionRoot.StringRetriever.GetFileList();

                string[] filteredList = compositionRoot.FilterStrings.Filter(list);

                compositionRoot.RenameFiles.Convert(list);

                localConsoleWriter.WriteColorMessage("Done!", MessageType.success);
            }
            catch (Exception e)
            {
                localConsoleWriter.WriteColorMessage(e.Message, MessageType.failure);
            }
        }
    }

    public class CompositionRoot
    {
        public StringRetriever StringRetriever { get; set; }
        public FilterStrings FilterStrings { get; set; }
        public RenameFiles RenameFiles { get; set; }

        public CompositionRoot()
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

            this.StringRetriever = new StringRetriever(new FileRetriever());

            this.FilterStrings = new FilterStrings(filters);

            this.RenameFiles = new RenameFiles(new Notifier(new ConsoleWriter()), renamers);
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

    public interface INotificationService
    {
        void Notify(string input, string result);
    }

    public class Notifier : INotificationService
    {
        private readonly IMessageWriter messageWriter;

        public Notifier(IMessageWriter messageWriter)
        {
            this.messageWriter = messageWriter ?? throw new ArgumentNullException(nameof(messageWriter));
        }

        public void Notify(string input, string result)
        {
            messageWriter.WriteColorMessage(Path.GetFileName(input), MessageType.incorrect);
            messageWriter.WriteColorMessage("converted to:", MessageType.information);
            messageWriter.WriteColorMessage(Path.GetFileName(result), MessageType.correct);
            messageWriter.WriteMessage("");
        }
    }

    public class RenameFiles
    {
        private readonly List<Renamer> renamers;
        private readonly INotificationService notification;

        public RenameFiles(
            INotificationService notification,
            List<Renamer> renamers)
        {
            this.notification = notification ?? throw new ArgumentNullException(nameof(notification));
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

                this.notification.Notify(unchangedFilepath, mutatingFilepath);
            }
        }
    }
}
