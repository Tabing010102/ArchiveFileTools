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
        [Option('d', "dir", Required = true, HelpText = "The directory to scan.")]
        public string DirectoryPath { get; set; } = string.Empty;
        [Option('e', "ext", Separator = ',', Default = new string[] { "zip", "z01", "7z", "001", "rar" },
            HelpText = "The file extension to scan for.")]
        public IEnumerable<string> Extensions { get; set; } = null!;
        [Option('p', "pwd", Separator = ',', Default = new string[] { }, HelpText = "The password to use for the archive files.")]
        public IEnumerable<string> Passwords { get; set; } = null!;
        [Option('c', "console", Required = true, HelpText = "Print to console.", SetName = "console")]
        public bool PrintToConsole { get; set; }
        [Option('f', "file", Required = true, HelpText = "Result file location.", SetName = "file")]
        public string ResultFilePath { get; set; } = string.Empty;
    }
}
