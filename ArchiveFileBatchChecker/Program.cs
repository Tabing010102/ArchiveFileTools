using CommandLine;
using SevenZip;
using System.Text.RegularExpressions;

namespace ArchiveFileBatchChecker
{
    internal class Program
    {
        private static Regex _regex = new(@"\.part(?!0*1)\d*\.");

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                          .WithParsed<Options>(Run);
        }

        static void Run(Options options)
        {
            var dir = options.DirectoryPath;
            var pwds = new List<string> { string.Empty };
            pwds.AddRange(options.Passwords);
            var files = new List<string>();
            foreach (var ext in options.Extensions)
            {
                files.AddRange(Directory.EnumerateFiles(dir, $"*.{ext}", SearchOption.AllDirectories));
            }
            files = files.Where(x => _regex.IsMatch(x) == false).ToList();
            foreach (var file in files)
            {
                var result = false;
                foreach (var pwd in pwds)
                {
                    SevenZipExtractor extractor;
                    if (pwd == string.Empty)
                    {
                        extractor = new SevenZipExtractor(file);
                    }
                    else
                    {
                        extractor = new SevenZipExtractor(file, pwd);
                    }
                    if (extractor.Check())
                    {
                        result = true;
                        break;
                    }
                }
                Console.WriteLine($"{file} check {result.ToString()}");
            }
        }
    }
}
