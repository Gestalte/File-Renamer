using System;
using System.IO;

namespace File_Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static string[] GetAllFiles()
            => Directory.GetFiles(Directory.GetCurrentDirectory());

        // TODO: Create an Interface for Filter Rules to apply to the files found in the direcory string[] -> string[]

        // TODO: Create an Interface for Rules to apply to fileNames. string -> string

        // TODO: Get all files in the current directory

        // TODO: load all the Filter Rules and apply them to the list of files to filter it.

        // TODO: loop through the filtered list of files and load and apply each rule to the current file's file name then rename it.
    }
}
