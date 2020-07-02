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
        public StringRetriever StringRetriever { get; }
        public FilterStrings FilterStrings { get; }
        public RenameFiles RenameFiles { get; }

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

            this.RenameFiles = new RenameFiles(new FileRenamedEventHandler(new ConsoleWriter()), renamers);
        }
    }

    public class FileRenamed
    {
        public readonly string OldName;
        public readonly string NewName;

        public FileRenamed(string oldName, string newName)
        {
            this.OldName = oldName ?? throw new ArgumentNullException(nameof(oldName));
            this.NewName = newName ?? throw new ArgumentNullException(nameof(newName));
        }
    }

    public interface IEventHandler<TEvent>
    {
        void Handle(TEvent e);
    }

    public class FileRenamedEventHandler : IEventHandler<FileRenamed>
    {
        private readonly IMessageWriter messageWriter;

        public FileRenamedEventHandler(IMessageWriter messageWriter)
        {
            this.messageWriter = messageWriter ?? throw new ArgumentNullException(nameof(messageWriter));
        }

        public void Handle(FileRenamed e)
        {
            messageWriter.WriteColorMessage(Path.GetFileName(e.OldName), MessageType.incorrect);
            messageWriter.WriteColorMessage("converted to:", MessageType.information);
            messageWriter.WriteColorMessage(Path.GetFileName(e.NewName), MessageType.correct);
            messageWriter.WriteMessage("");
        }
    }

    public class RenameFiles
    {
        private readonly List<Renamer> renamers;
        private readonly IEventHandler<FileRenamed> fileRenamedEventHandler;

        public RenameFiles(IEventHandler<FileRenamed> fileRenamedEventHandler, List<Renamer> renamers)
        {
            this.fileRenamedEventHandler = fileRenamedEventHandler
                ?? throw new ArgumentNullException(nameof(fileRenamedEventHandler));
            this.renamers = renamers
                ?? throw new ArgumentNullException(nameof(renamers));
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

                this.fileRenamedEventHandler.Handle(new FileRenamed(unchangedFilepath, mutatingFilepath));
            }
        }
    }
}
