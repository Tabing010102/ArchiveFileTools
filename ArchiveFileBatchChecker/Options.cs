using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveFileBatchChecker
{
    public class Options
    {
        [Option('i', "in", HelpText = "Input file location.", Group = "input")]
        public string InputFilePath { get; set; } = string.Empty;
        [Option('d', "dir", HelpText = "The directory to scan.", Group = "input")]
        public string DirectoryPath { get; set; } = string.Empty;
        [Flags]
        public enum SortMethod
        {
            Default,
            Asc,
            Desc
        }
        [Option('s', "sort", Default = SortMethod.Default, HelpText = "Sort method of input files.")]
        public SortMethod Sort { get; set; }

        [Option('e', "ext", Separator = ',', Default = new string[] { "zip", "z01", "7z", "001", "rar" },
            HelpText = "The file extension to scan for.")]
        public IEnumerable<string> Extensions { get; set; } = null!;
        [Option('p', "pwd", Separator = ',', Default = new string[] { }, HelpText = "The password to use for the archive files.")]
        public IEnumerable<string> Passwords { get; set; } = null!;

        [Option('c', "console", Required = true, HelpText = "Print to console.", SetName = "console")]
        public bool PrintToConsole { get; set; }
        [Option('o', "out", Required = true, HelpText = "Result file location.", SetName = "outfile")]
        public string ResultFilePath { get; set; } = string.Empty;
    }
}
