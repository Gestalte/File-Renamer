using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace File_Renamer
{
    partial class Program
    {
        static void Main(string[] args)
        {
            List<FilterRule> filterRules = null;
            List<RenameRule> renameRules = null;

            var consoleWriter = new ConsoleWriter();

            try
            {
                IConfigurationRoot Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

                var settings = Configuration.GetSection("AppSettings");
                var filterRuleNames = settings.GetSection("FilterRules").Value.Split(",");
                var renameRulesNames = settings.GetSection("RenameRules").Value.Split(",");

                foreach (var filterRule in filterRuleNames)
                {
                    Type type = Type.GetType(filterRule, throwOnError: true);
                    FilterRule fr = (FilterRule)Activator.CreateInstance(type);
                    filterRules.Add(fr);
                }

                foreach (var renameRule in renameRulesNames)
                {
                    Type type = Type.GetType(renameRule, throwOnError: true);
                    RenameRule rr = (RenameRule)Activator.CreateInstance(type);
                    renameRules.Add(rr);
                }

                DoWork doWork = new DoWork(new FileRetriever(), consoleWriter, filterRules, renameRules);

                doWork.Convert();

                consoleWriter.WriteColorMessage("Done!", messageType.success);
            }
            catch (Exception e)
            {
                consoleWriter.WriteColorMessage(e.Message, messageType.failure);
            }
        }
    }

    public class DoWork
    {
        private readonly FileProvider fileProvider;
        private readonly MessageWriter messageWriter;
        private readonly List<FilterRule> filterRules;
        private readonly List<RenameRule> renameRules;

        public DoWork(
            FileProvider fileProvider,
            MessageWriter messageWriter,
            List<FilterRule> filterRules,
            List<RenameRule> renameRules)
        {
            this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            this.messageWriter = messageWriter ?? throw new ArgumentNullException(nameof(messageWriter));
            this.filterRules = filterRules ?? throw new ArgumentNullException(nameof(filterRules));
            this.renameRules = renameRules ?? throw new ArgumentNullException(nameof(renameRules));
        }

        public void Convert()
        {
            var files = fileProvider.GetAllFiles();

            foreach (var filterRule in filterRules)
            {
                Filter filter = new Filter(filterRule);

                files = filter.FilterFilepaths(files);
            }

            foreach (var filepath in files)
            {
                string unchangedFilepath = filepath;
                string mutatingFilepath = filepath;

                foreach (var renameRule in renameRules)
                {
                    Renamer renamer = new Renamer(renameRule);

                    mutatingFilepath = renamer.Rename(mutatingFilepath);
                }

                messageWriter.WriteColorMessage(Path.GetFileName(unchangedFilepath), messageType.incorrect);
                messageWriter.WriteColorMessage("converted to:", messageType.information);
                messageWriter.WriteColorMessage(Path.GetFileName(mutatingFilepath), messageType.correct);
                messageWriter.WriteMessage("");
            }
        }
    }

    public interface FileProvider
    {
        string[] GetAllFiles();
    }

    public class FileRetriever : FileProvider
    {
        public string[] GetAllFiles()
            => Directory.GetFiles(Directory.GetCurrentDirectory());
    }

    public interface RenameRule
    {
        string ApplyRule(string FilePath);
    }

    public interface FilterRule
    {
        string[] ApplyRule(string[] FilePath);
    }

    public class Renamer
    {
        private readonly RenameRule renameRule;

        public Renamer(RenameRule renameRule)
        {
            if (renameRule == null)
                throw new ArgumentNullException(nameof(renameRule));

            this.renameRule = renameRule;
        }

        public string Rename(string FilePath)
            => this.renameRule.ApplyRule(FilePath);
    }

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
             => this.filterRule.ApplyRule(filePaths);
    }

    public interface MessageWriter
    {
        void WriteMessage(string message);

        void WriteColorMessage(string message, messageType color); // Not sure about this.
    }

    public enum messageType
    {
        correct,
        incorrect,
        success,
        failure,
        information,
    }

    public class ConsoleWriter : MessageWriter
    {
        public void WriteColorMessage(string message, messageType type)
        {
            switch (type)
            {
                case messageType.correct:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case messageType.incorrect:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case messageType.success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case messageType.failure:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case messageType.information:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                default:
                    break;
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}

