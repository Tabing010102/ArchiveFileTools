using CommandLine;
using Microsoft.Extensions.Logging;
using SevenZip;
using System.Text.RegularExpressions;

namespace ArchiveFileBatchChecker
{
    internal class Program
    {
        private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(o =>
        {
            o.IncludeScopes = true;
            o.SingleLine = true;
            o.TimestampFormat = "yyyy-MM-dd HH:mm:sszzz ";
        }));
        private static ILogger _logger = _loggerFactory.CreateLogger<Program>();

        private static Regex _regex = new(@"\.part(\d*[02-9]|0*[1-9]\d*1)\.");

        private static List<(string, string)> _succeedFiles = new();
        private static List<string> _failedFiles = new();

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                          .WithParsed<Options>(Run);
        }

        static void Run(Options options)
        {
            // add empty password
            var pwds = new List<string> { string.Empty };
            pwds.AddRange(options.Passwords);
            var files = new List<string>();
            if (options.InputFilePath != string.Empty)
            {
                _logger.LogInformation($"Reading files from \"{options.InputFilePath}\"");
                files.AddRange(File.ReadAllLines(options.InputFilePath)
                                   .Where(x => string.IsNullOrWhiteSpace(x) == false));
                _logger.LogInformation($"Added {files.Count} files");
            }
            else
            {
                foreach (var ext in options.Extensions)
                {
                    _logger.LogInformation($"Adding files for extension {ext}");
                    var cntFiles = Directory.EnumerateFiles(options.DirectoryPath, $"*.{ext}", SearchOption.AllDirectories);
                    files.AddRange(cntFiles);
                    _logger.LogInformation($"Added {cntFiles.Count()} files for extension {ext}");
                }
                _logger.LogInformation($"Totally added {files.Count} files");
                var removedFiles = 0;
                files = files.Where(file =>
                {
                    var result = _regex.IsMatch(file);
                    if (result == true)
                    {
                        removedFiles++;
                        _logger.LogInformation($"Removing file: \"{file}\"");
                    }
                    return result == false;
                }).ToList();
                _logger.LogInformation($"Removed {removedFiles} files");
            }

            
            foreach (var file in files)
            {
                var result = false;
                foreach (var pwd in pwds)
                {
                    _logger.LogInformation($"Checking \"{file}\" with password \"{pwd}\"");
                    SevenZipExtractor extractor;
                    if (pwd == string.Empty)
                    {
                        extractor = new SevenZipExtractor(file);
                    }
                    else
                    {
                        extractor = new SevenZipExtractor(file, pwd);
                    }

                    bool checkResult = extractor.Check();
                    extractor.Dispose();
                    if (checkResult == true)
                    {
                        result = true;
                        _succeedFiles.Add((file, pwd));
                        _logger.LogInformation($"Check for \"{file}\" passed");
                        break;
                    }
                }
                if (result == false)
                {
                    _failedFiles.Add(file);
                    _logger.LogWarning($"Check for \"{file}\" failed");
                }
            }
            if (options.PrintToConsole)
            {
                _logger.LogInformation($"Printing results to console");
                Console.Out.Flush();
                Thread.Sleep(200);
                WriteResult(Console.Out);
            }
            // save results to file
            else
            {
                _logger.LogInformation($"Writing results to \"{options.ResultFilePath}\"");
                Console.Out.Flush();
                Thread.Sleep(200);
                using var fs = new FileStream(options.ResultFilePath, FileMode.Append, FileAccess.Write);
                using var sw = new StreamWriter(fs);
                WriteResult(sw);
            }
        }

        static void WriteResult(TextWriter sw)
        {
            sw.WriteLine($"Check result at {DateTime.Now.ToString("O")}");
            sw.WriteLine($"{_failedFiles.Count} files check failed.");
            foreach (var file in _failedFiles)
            {
                sw.WriteLine(file);
            }
            sw.WriteLine();
            sw.WriteLine($"{_succeedFiles.Count} files check passed.");
            foreach (var file in _succeedFiles)
            {
                var pwd = file.Item2 == string.Empty ? "no password" : "password \"" + file.Item2 + "\"";
                sw.WriteLine($"{file.Item1} with {pwd}");
            }
            sw.WriteLine();
        }
    }
}
