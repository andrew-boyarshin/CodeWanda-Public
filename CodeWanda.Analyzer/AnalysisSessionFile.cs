//#define JSON_IMPORT_EXPORT

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CodeWanda.Model.Semantic;
using CodeWanda.Parser.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Serilog;
#if JSON_IMPORT_EXPORT
using Newtonsoft.Json;
#endif

namespace CodeWanda.Analyzer
{
    public class AnalysisSessionFile : IEquatable<AnalysisSessionFile>
    {
        public AnalysisSessionFile(FileInfo sourceFile)
        {
            SourceFile = sourceFile;
            {
                using var stream = sourceFile.OpenRead();
                using var sha512 = new SHA512Managed();
                var hash = sha512.ComputeHash(stream);
                HashString = BitConverter.ToString(hash).Replace("-", "", StringComparison.Ordinal);
            }
            {
                using var stream = sourceFile.OpenText();
                SourceContents = stream.ReadToEnd();
            }
            // todo: set ParsedJsonFile
        }

        public string HashString { get; }
        public string SourceContents { get; }
        public FileInfo SourceFile { get; }
        public List<ClassDefinition> ParsedClasses { get; set; }
#if JSON_IMPORT_EXPORT
        public FileInfo ParsedJsonFile { get; set; }
        public FileInfo ControlFlowJsonFile { get; set; }
#endif

        public bool Equals(AnalysisSessionFile other)
        {
	        if (other is null) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return HashString == other.HashString;
        }

        public override bool Equals(object obj)
        {
	        if (obj is null) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        return obj.GetType() == GetType() && Equals((AnalysisSessionFile) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HashString);
        }

        public static bool operator ==(AnalysisSessionFile left, AnalysisSessionFile right)
        {
	        return Equals(left, right);
        }

        public static bool operator !=(AnalysisSessionFile left, AnalysisSessionFile right)
        {
	        return !Equals(left, right);
        }

        public bool LoadParsedSource()
        {
            if (SourceFile.Extension != ".cs")
            {
                Log.Warning("Source file ({Extension}) not supported", SourceFile.Extension);
                return false;
            }

#if JSON_IMPORT_EXPORT
            if (ParsedJsonFile != null)
            {
                using var stream = ParsedJsonFile.Open(FileMode.Open, FileAccess.Read);
                using var reader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(reader);
                var serializer = new JsonSerializer();
                ParsedClasses = new List<ClassDefinition>(serializer.Deserialize<IList<ClassDefinition>>(jsonReader));
                return true;
            }
#endif

            var tree = CSharpSyntaxTree.ParseText(SourceText.From(SourceContents, Encoding.UTF8));
            var root = tree.GetCompilationUnitRoot();

            ParsedClasses = CSharpParser.ProcessCSharpSyntaxTree(root);

#if JSON_IMPORT_EXPORT
            if (ParsedJsonFile != null)
            {
                var jsonSerializer = JsonSerializer.CreateDefault();
                jsonSerializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
                jsonSerializer.TypeNameHandling = TypeNameHandling.Objects;
                using var textWriter = ParsedJsonFile.Open(FileMode.Create, FileAccess.Write);
                using var streamWriter = new StreamWriter(textWriter);
                var jsonWriter = new JsonTextWriter(streamWriter) {Formatting = Formatting.Indented};
                jsonSerializer.Serialize(jsonWriter, ParsedClasses);
            }
#endif

            return true;
        }
    }
}