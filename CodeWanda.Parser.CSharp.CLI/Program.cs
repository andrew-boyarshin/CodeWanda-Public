using System;
using System.IO;
using System.Text;
using Dawn;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using Serilog;

namespace CodeWanda.Parser.CSharp.CLI
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                MainInternal(args);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Top-level exception occurred");
            }
        }

        private static void MainInternal(string[] args)
        {
            Guard.Argument(args.Length, nameof(args.Length)).Equal(3);

            var path = new FileInfo(args[0]);
            Guard.Argument(path.Exists, nameof(path.Exists)).True();

            var outputPath = new FileInfo(args[1]);
            var outputCsPath = new FileInfo(args[2]);

            Log.Information("{Path} to {OutputPath} & {OutputCSPath}", path, outputPath, outputCsPath);

            var tree = CSharpSyntaxTree.ParseText(SourceText.From(path.OpenText().ReadToEnd(), Encoding.UTF8));
            var root = tree.GetCompilationUnitRoot();

            var classes = CSharpParser.ProcessCSharpSyntaxTree(root);

            {
                var jsonSerializer = JsonSerializer.CreateDefault();
                jsonSerializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
                jsonSerializer.TypeNameHandling = TypeNameHandling.Objects;
                using var textWriter = outputPath.Open(FileMode.Create, FileAccess.Write);
                using var streamWriter = new StreamWriter(textWriter);
                using var jsonWriter = new JsonTextWriter(streamWriter) {Formatting = Formatting.Indented};
                jsonSerializer.Serialize(jsonWriter, classes);
            }
            {
                using var textWriter = outputCsPath.Open(FileMode.Create, FileAccess.Write);
                using var streamWriter = new StreamWriter(textWriter);
                foreach (var classDefinition in classes)
                {
                    streamWriter.WriteLine(classDefinition);
                }
            }
        }
    }
}