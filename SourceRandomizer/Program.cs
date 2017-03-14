using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SourceRandomizer
{
    internal static class Program
    {
        private static readonly List<string> SourcePaths = new List<string>();
        private static string _sourceExtension;
        private static string _sourceLineFormat;

        public static void Main(string[] args)
        {
            var solutionPath = string.Empty;
            var comment = string.Empty;

            // Get arguments
            for (var i = 0; i < args.Length; i++)
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
                        _sourceLineFormat = "\n";
                        break;

                    case "-crlf":
                        _sourceLineFormat = "\r\n";
                        break;
                }
            }

            // Check arguments
            if (solutionPath == string.Empty)
            {
                Console.WriteLine("Solution not found (-s)");
                Console.ReadLine();
                Environment.Exit(1);
            }

            if (_sourceExtension == string.Empty)
            {
                Console.WriteLine("Extension not found (-e)");
                Console.ReadLine();
                Environment.Exit(1);
            }

            if (comment == string.Empty)
            {
                Console.WriteLine("Comment not found (-c)");
                Console.ReadLine();
                Environment.Exit(1);
            }

            if (!_sourceExtension.Contains("."))
                _sourceExtension = "." + _sourceExtension;

            ProcessFiles(solutionPath);

            var rnd = new Random(Environment.TickCount);

            var searchTemplate = comment + "{0}{1}";
            var openTemplate = string.Empty;
            var closeTag = "c";
            
            // Foreach file found
            foreach (var sourcePath in SourcePaths)
            {
                var sourceFile = File.ReadAllText(sourcePath);
                var modFile = sourceFile;
                var lineFormat = _sourceLineFormat;

                // Automatic line format detection
                if (lineFormat == string.Empty)
                {
                    lineFormat = lineFormat.Contains("\r\n") ? "\r\n" : "\n";
                }

                // Swap lines
                var swapLineOpen = searchTemplate.Replace("{0}", "s").Replace("{1}", openTemplate) + lineFormat;
                var swapLineClose = searchTemplate.Replace("{0}", "s").Replace("{1}", closeTag);
                var swapLines = new List<string>();

                // Get all matches (s tag)
                for (var i = 0; i < int.MaxValue; i++)
                {
                    var result = GetBetween(modFile, swapLineOpen, swapLineClose, i, lineFormat);

                    if (result == string.Empty)
                        break;

                    swapLines.Add(result);
                }

                // Foreach swap lines
                foreach (var match in swapLines)
                {
                    // Block
                    var blockOpen = searchTemplate.Replace("{0}", "b").Replace("{1}", openTemplate) + lineFormat;
                    var blockClose = searchTemplate.Replace("{0}", "b").Replace("{1}", closeTag);
                    var blocks = new List<string>();

                    // Get all matches (b tag)
                    for (var i = 0; i < int.MaxValue; i++)
                    {
                        var result = GetBetween(match, blockOpen, blockClose, i, lineFormat);

                        if (result == string.Empty)
                            break;

                        blocks.Add(result);
                    }

                    // Use line randomizer
                    if (blocks.Count == 0)
                    {
                        var lines = match.Split(new[] { lineFormat }, StringSplitOptions.None);
                        var linesRandomized = new string[lines.Length - 1];
                        var outputSb = new StringBuilder();

                        // Randomize lines
                        foreach (var line in lines)
                        {
                            if (line == string.Empty)
                                continue;

                            int lineRandom;

                            while (true)
                            {
                                lineRandom = rnd.Next(0, linesRandomized.Length);

                                if (linesRandomized[lineRandom] == null)
                                    break;
                            }

                            linesRandomized[lineRandom] = line;
                        }

                        // Create output
                        foreach (var line in linesRandomized)
                        {
                            var tempLine = line.Insert(1, "/*[temp]*/");
                            outputSb.Append(tempLine);
                            outputSb.Append(lineFormat);
                        }

                        // Replace
                        var index = modFile.IndexOf(match);
                        var length = match.Length;
                        modFile = modFile.Remove(index, length);
                        modFile = modFile.Insert(index, outputSb.ToString());
                    }
                    // Use blocks randomizer
                    else
                    {
                        var blocksRandomized = new string[blocks.Count];

                        // Randomize blocks
                        foreach (var block in blocks)
                        {
                            if(block == string.Empty)
                                continue;

                            int lineRandom;

                            while (true)
                            {
                                lineRandom = rnd.Next(0, blocksRandomized.Length);

                                if (blocksRandomized[lineRandom] == null)
                                    break;
                            }

                            blocksRandomized[lineRandom] = block;
                        }

                        // Replace
                        for (var i = 0; i < blocks.Count; i++)
                        {
                            var output = blocksRandomized[i].Insert(1, "/*[temp]*/");

                            var index = modFile.IndexOf(blocks[i]);
                            var length = blocks[i].Length;
                            modFile = modFile.Remove(index, length);
                            modFile = modFile.Insert(index, output);
                        }
                    }
                }

                // Remove temp
                modFile = modFile.Replace("/*[temp]*/", string.Empty);

                // Save randomized file
                if (modFile != sourceFile && modFile != string.Empty)
                {
                    File.WriteAllText(sourcePath, modFile);
                }
            }

            Environment.Exit(0);
        }

        private static void ProcessFiles(string path)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);

                if (extension != _sourceExtension)
                    continue;

                SourcePaths.Add(file);
            }

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                ProcessFiles(directory);
            }
        }

        private static string GetBetween(string source, string p1, string p2, int offset, string lineFormat)
        {
            var splitArray = source.Split(new[] { p1 }, StringSplitOptions.None);

            if (splitArray.Length - 1 < offset + 1)
                return string.Empty;

            var result = splitArray[offset + 1];
            var resultLines = result.Split(new[] { lineFormat }, StringSplitOptions.None);
            var output = string.Empty;

            foreach (var line in resultLines)
            {
                if (line.Contains(p2))
                    break;

                output += line + lineFormat;
            }

            return output;
        }
    }
}
