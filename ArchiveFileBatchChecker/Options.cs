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
        [Option('e', "ext", Separator = ',', Default = "zip,rar,7z,001", HelpText = "The file extension to scan for.")]
        public IList<string> Extensions { get; set; } = new List<string>();
        [Option('p', "pwd", Separator = ',', Default = "", HelpText = "The password to use for the archive files.")]
        public IList<string> Passwords { get; set; } = new List<string>();
    }
}
