using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace File_Renamer
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var files = GetAllFiles(); // TODO: DI for this.

            IConfigurationRoot filteringRules = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("filtering-rules.json")
                .Build();

            // TODO: Read filter rules from json file

            var jsonFilterRules = new string[] { };

            foreach (var jsonRule in jsonFilterRules)
            {
                Type type = Type.GetType(jsonRule, throwOnError: true);
                FilterRule filterRule = (FilterRule)Activator.CreateInstance(type);

                Filter filter = new Filter(filterRule);

                files = filter.FilterFilepaths(files);
            }

            IConfigurationRoot renamingRules = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("renaming-rules.json")
                .Build();

            // TODO: Read rename rules from json file

            var jsonRenameRules = new string[] { };

            foreach (var filepath in files)
            {
                string unchangedFilepath = filepath;
                string mutatingFilepath = filepath;

                foreach (var jsonRule in jsonRenameRules)
                {
                    Type type = Type.GetType(jsonRule, throwOnError: true);
                    RenameRule renameRule = (RenameRule)Activator.CreateInstance(type);

                    Renamer renamer = new Renamer(renameRule);

                    mutatingFilepath = renamer.Rename(mutatingFilepath);
                }

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(Path.GetFileName(unchangedFilepath));
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("converted to:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Path.GetFileName(mutatingFilepath));
                Console.WriteLine();
            }
        }

        public static string[] GetAllFiles()
            => Directory.GetFiles(Directory.GetCurrentDirectory());
    }
}
