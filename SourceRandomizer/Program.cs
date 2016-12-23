using System;
using System.Collections.Generic;
using System.IO;

namespace SourceRandomizer
{
    class Program
    {
        private static readonly List<string> SourcePaths = new List<string>();
        private static string _sourceExtension;
        private static string _sourceLineEnding;

        public static void Main(string[] args)
        {
            var solutionPath = string.Empty;
            var comment = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-s":
                        solutionPath = args[i + 1];
                        break;

                    case "-e":
                        _sourceExtension = args[i + 1];
                        break;

                    case "-c":
                        comment = args[i + 1];
                        break;

                    case "-lf":
                        _sourceLineEnding = "\n";
                        break;

                    case "-crlf":
                        _sourceLineEnding = "\r\n";
                        break;
                }
            }

            if (solutionPath == string.Empty)
            {
                Console.WriteLine("Solution not found (-s)");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (_sourceExtension == string.Empty)
            {
                Console.WriteLine("Extension not found (-e)");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (comment == string.Empty)
            {
                Console.WriteLine("Comment not found (-c)");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (_sourceLineEnding == string.Empty)
            {
                Console.WriteLine("Line ending not found (-lf/-crlf)");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (!_sourceExtension.Contains("."))
                _sourceExtension = "." + _sourceExtension;

            ProcessFiles(solutionPath);

            var rnd = new Random(Environment.TickCount);

            var searchTemplate = comment + "[{1}{0}]";
            var openTemplate = string.Empty;
            var closeTemplae = "/";

            // Foreach file found
            foreach (var sourcePath in SourcePaths)
            {
                var sourceFile = File.ReadAllText(sourcePath);
                var modFile = sourceFile;

                // Swap lines
                var swapLinesOpenTag = searchTemplate.Replace("{0}", "swap_lines").Replace("{1}", openTemplate) + _sourceLineEnding;
                var swapLinesCloseTag = searchTemplate.Replace("{0}", "swap_lines").Replace("{1}", closeTemplae);
                var swapLinesMatchesList = new List<string>();

                // Get all matches
                for (int i = 0; i < int.MaxValue; i++)
                {
                    var result = GetBetween(modFile, swapLinesOpenTag, swapLinesCloseTag, i);

                    if (result == string.Empty)
                        break;

                    swapLinesMatchesList.Add(result);
                }

                // Foreach match
                foreach (var match in swapLinesMatchesList)
                {
                    var lines = match.Split(new[] { _sourceLineEnding }, StringSplitOptions.None);
                    var modLinesArray = new string[lines.Length - 1];
                    var modLines = string.Empty;

                    // Randomize lines
                    foreach (var line in lines)
                    {
                        if (line == string.Empty)
                            continue;

                        var lineRandom = 0;

                        while (true)
                        {
                            lineRandom = rnd.Next(0, modLinesArray.Length);

                            if (modLinesArray[lineRandom] == null)
                                break;
                        }

                        modLinesArray[lineRandom] = line;
                    }

                    // Create replace
                    foreach (string line in modLinesArray)
                    {
                        modLines += line;
                        modLines += _sourceLineEnding;
                    }

                    // Replace
                    modFile = modFile.Replace(match, modLines);
                }

                // Save randomized file
                if (modFile != sourceFile)
                {
                    File.WriteAllText(sourcePath, modFile);
                }
            }

            Environment.Exit(0);
        }

        private static void ProcessFiles(string path)
        {
            var files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                var extension = Path.GetExtension(file);

                if (extension != _sourceExtension)
                    continue;

                SourcePaths.Add(file);
            }

            var directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                ProcessFiles(directory);
            }
        }

        private static string GetBetween(string source, string p1, string p2, int offset)
        {
            try
            {
                var result = source.Split(new[] { p1 }, StringSplitOptions.None)[offset + 1];
                var resultLines = result.Split(new [] { _sourceLineEnding }, StringSplitOptions.None);
                var output = string.Empty;

                foreach (string line in resultLines)
                {
                    if (line.Contains(p2))
                        break;

                    output += line + _sourceLineEnding;
                }

                return output;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
