using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ExportAttributes;
using Exporter.Extensions;
using Exporter.Models;

// TODO: somewhere set a logging level attribute; add logging to methods; logger can screen out superfluous log entries

namespace Exporter.Services
{

    public interface IMetaObjectService : IService
    {
        DirectoryInfo ArchiveDirectory { get; set; }
        Batch Batch { get; set; }
        Builder GetBuilder(Document document);
        Encoding GetEncoding(in EncodingSettings encodingSettings);
        FieldInfo GetFieldInfo(Document document);
        StreamWriter GetStreamWriter(in Document document);
        DirectoryInfo LogDirectory { get; set; }
        string OrgCode { get; }
        OrgUnit OrgUnit { get; }
        DirectoryInfo OutputDirectory { get; set; }
        Process Process { get; }
        string ProcessCode { get; }
        DateTime StartTime { get; }
        DirectoryInfo WorkingDirectory { get; set; }
    }

    public delegate object ProcessMethod(Document document, Builder builder, Field field, object value);

    internal class MetaObjectService : Service, IMetaObjectService
    {
        const string modelAssy = "ExportDefinitions";

        internal MetaObjectService(DirectoryInfo exportRoot, OrgUnit orgUnit, Process process)
        {
            OrgUnit = orgUnit;
            Process = process;
            CreateDirectories(exportRoot);
            SetDefaultReplacements();
            SetupDocuments();
        }

        public DirectoryInfo ArchiveDirectory { get; set; }
        public Batch Batch { get; set; }
        public DirectoryInfo LogDirectory { get; set; }
        public string OrgCode => OrgUnit.Code;
        public OrgUnit OrgUnit { get; }
        public DirectoryInfo OutputDirectory { get; set; }
        public Process Process { get; }
        public string ProcessCode => Process.Code;
        public DateTime StartTime { get; } = DateTime.Now;
        public DirectoryInfo WorkingDirectory { get; set; }

        public Builder GetBuilder(Document document)
        {
            // TODO: think about a better way to set the field and record sizes; think about chunk size too
            return new Builder
            {
                Data = new StringBuilder(10000),
                Multi = new StringBuilder(10000),
                Record = new StringBuilder(255 * document.Fields.Count)
            };
        }

        public Encoding GetEncoding(in EncodingSettings encodingSettings)
        {
            // get the encoding options
            var bits = encodingSettings?.Bits ?? 8;
            var endian = (encodingSettings?.Endian ?? "BE") == "BE";
            var bom = (encodingSettings?.UseBOM ?? false);

            // get encoding helper and default to UTF8 if the requested helper is not found
            if (!encodingHelpers.TryGetValue(bits, out var helper))
                encodingHelpers.TryGetValue(8, out helper);

            return helper.Invoke(bom, endian);
        }

        public FieldInfo GetFieldInfo(Document document)
        {
            var index = 1;
            var displayIndex = 1;

            var keyField = new Field
            {
                ColumnName = document.KeyColumnName,
                DisplayIndex = 0,
                Index = 0
            };

            var all = new List<Field> { keyField };
            var horz = new List<Field> { keyField };
            var vert = new List<Field> { keyField };

            // separate the fields into horizontal and vertical
            document.Fields.ForEach(f =>
            {
                f.Index = index;
                f.DisplayIndex = (f.IsHidden) ? -1 : displayIndex;

                if (!f.IsHidden)
                {
                    if (f.IsVerticalField)
                        vert.Add(f);
                    else
                        horz.Add(f);

                    all.Add(f);
                    displayIndex++;
                }

                index++;
            });

            return new FieldInfo
            {
                AllFieldCount = all.Count,
                AllFields = all,
                HFieldCount = horz.Count,
                HFields = horz,
                KeyColumnName = document.KeyColumnName,
                MultiIndices = vert.Skip(1)
                    .Where(f => f.FieldType == FieldType.MultiRow)
                    .Select(f => f.DisplayIndex).ToList(),
                //ProcessMethods = new ProcessMethod[all.Count],
                VFieldCount = vert.Count,
                VFields = vert,
                VFieldsByType = vert.Skip(1)
                    .ToDictionary(f => (object)f.RecordTypeKey, f => f)
            };

        }

        public StreamWriter GetStreamWriter(in Document document)
        {
            var file = WorkingDirectory.FileIn(GetFileName(document));
            var encoding = GetEncoding(document?.EncodingSettings);
            return new StreamWriter(file.FullName, false, encoding);
        }

        protected Dictionary<char, string> DefaultReplacements { get; set; }

        protected Dictionary<string, Regex> BuildRegexes(Dictionary<char, string> replacements)
        {
            int? lastValue = null;
            var codes = new List<int>();
            var ranges = new List<string>();
            var dic = new Dictionary<string, Regex>();

            Func<List<int>, string> ConvertCodesToRange = (codeList) =>
            {
                return (codeList.Count <= 2) ?
                    codeList.Select(c => $"\\u{c.ToString("X4")}").ToList().Concat() :
                    $"\\u{codeList.First().ToString("X4")}-\\u{codeList.Last().ToString("X4")}";
            };

            // group replacements by replacement value
            var groups = replacements
                .GroupBy(kvp => kvp.Value)
                .ToDictionary(g => g.Key, g => g.Select(i => (int)i.Key).OrderBy(i => i));

            foreach(var group in groups)
            {
                var itemCount = group.Value.Count();

                for (var i = 0; i < itemCount; i++)
                {
                    var value = group.Value.ElementAt(i);

                    if(lastValue.GetValueOrDefault(int.MinValue) + 1 != value)
                    {
                        // push any existing codes into the range and reset codes
                        ranges.Add(ConvertCodesToRange(codes));
                        codes.Clear();
                    }
                    
                    codes.Add(value);
                    lastValue = value;

                    // if this is the last item of the group then push the codes to the range
                    if (i == itemCount - 1) ranges.Add(ConvertCodesToRange(codes));
                }

                // create a regex for the ranges
                dic.Add(group.Key, new Regex($"[{ranges.Concat("")}]", RegexOptions.Singleline)); 

                codes.Clear();
                ranges.Clear();
                lastValue = null;
            }

            return dic;
        }

        protected void CreateDirectories(DirectoryInfo exportRoot)
        {
            // setup standard directories
            ArchiveDirectory = exportRoot.DirectoryIn(OrgCode, "Exports", ProcessCode, "Archive").CreateSafe();
            LogDirectory = exportRoot.DirectoryIn(OrgCode, "Exports", ProcessCode, "Logs").CreateSafe();
            OutputDirectory = (string.IsNullOrWhiteSpace(Process.OutputDirectory)) ? null :
                new DirectoryInfo(Process.OutputDirectory);
            WorkingDirectory = exportRoot.DirectoryIn(OrgCode, "Exports", ProcessCode, "Working").CreateSafe();
        }

        protected string GetFileName(Document document)
        {
            // TODO: make a way to replace other tokens
            //       have batch procedure return an optional dataset containing any tokens which would get added to the
            //       Batch object as a dictionary of object
            var fileName = document.FilePattern.Replace("{fileDate}", StartTime.ToString(document.DatePattern));
            return fileName.Replace("{batchId}", Batch.BatchId.ToString());
        }

        protected List<Field> GetDocumentFields(Type modelType, Dictionary<Type, ModelFormatAttribute> formatAttributes)
        {
            var i = 0;
            var fields = new List<Field>();

            foreach (var pi in modelType.GetProperties())
            {
                // get any field attribute
                FieldDefinitionAttribute fieldAttribute = pi.GetCustomAttributes(typeof(FieldDefinitionAttribute), false)
                    .FirstOrDefault() as FieldDefinitionAttribute;

                // determine the format string
                var formatString = fieldAttribute?.FormatString ??
                    formatAttributes?.GetSafe(pi.PropertyType)?.FormatString;

                // create a field using the reflected properties
                var field = new Field
                {
                    CaseItemIds = fieldAttribute?.CaseItemIds,
                    Category = fieldAttribute?.Category,
                    ColumnName = fieldAttribute?.ColumnName ?? pi.Name,
                    FieldType = fieldAttribute?.FieldType ?? FieldType.Default,
                    FilterValue = fieldAttribute?.FilterValue,
                    FilterColumn = fieldAttribute?.FilterColumn,
                    FormatString = formatString,
                    HasFilter = (!string.IsNullOrWhiteSpace(fieldAttribute?.FilterColumn) && (fieldAttribute?.FilterValue != null)),
                    Header = fieldAttribute?.Header ?? pi.Name,
                    Index = i,
                    IsHidden = fieldAttribute?.IsHidden ?? false,
                    IsVerticalField = fieldAttribute?.IsVerticalData ?? false,
                    MultiRowDelimiter = fieldAttribute?.MultiRowDelimiter,
                    QuestionIds = fieldAttribute?.QuestionIds,
                    QuestionTags = fieldAttribute?.QuestionTags,
                    RecordType = fieldAttribute?.RecordType,
                    RecordTypeKey = (fieldAttribute?.RecordType == null) ? null :
                        $"{fieldAttribute?.RecordType}{fieldAttribute.RecordKey}",
                    ScaleIds = fieldAttribute?.ScaleIds
                };

                fields.Add(field);
                i++;
            }

            return fields;
        }

        protected Dictionary<char, string> MergeReplacements(Dictionary<char, string> replacements)
        {
            // get a copy of the default replacements
            var newReplacements = new Dictionary<char, string>(DefaultReplacements);

            // if there are no replacements defined in the config then use the default replacements
            if ((replacements?.Count ?? 0) == 0)
                return newReplacements;

            foreach(var kvp in replacements)
            {
                if(kvp.Value == null)
                {
                    if (newReplacements.ContainsKey(kvp.Key)) newReplacements.Remove(kvp.Key);
                }
                else
                {
                    newReplacements[kvp.Key] = kvp.Value;
                }
            }

            return newReplacements;
        }

        protected void SetDefaultReplacements()
        {
            DefaultReplacements = new Dictionary<char, string>();

            foreach (var i in Enumerable.Range(0, 32)) // 31
            {
                DefaultReplacements[Convert.ToChar(i)] = " ";
            }

            foreach (var i in Enumerable.Range(127, 33)) // 159
            {
                DefaultReplacements[Convert.ToChar(i)] = " ";
            }

            foreach (var i in Enumerable.Range(65279, 257)) // 65535
            {
                DefaultReplacements[Convert.ToChar(i)] = " ";
            }
        }

        protected void SetupDocuments()
        {
            foreach(var document in Process.Documents)
            {
                
                // TODO: need to allow config from file, db, and model
                if(!string.IsNullOrWhiteSpace(document.ModelName))
                {
                    // find model through reflection
                    var typeName = $"{modelAssy}.Models.{OrgCode}.{document.ModelName}, {modelAssy}";
                    var modelType = Type.GetType(typeName, true);

                    // get any model definition attribute
                    var definitionAttribute = modelType
                        .GetCustomAttributes(typeof(ModelDefinitionAttribute), false)
                        .Select(a => (ModelDefinitionAttribute)a)
                        .FirstOrDefault();

                    // get any model format attributes
                    var formatAttributes = modelType.GetCustomAttributes(typeof(ModelFormatAttribute), false)
                        .Select(a => (ModelFormatAttribute)a)
                        .ToDictionary(a => a.Type, a => a);

                    // set the document properties from the reflected model values
                    document.Delimiter = definitionAttribute?.Delimiter ?? document.Delimiter;
                    document.Fields = GetDocumentFields(modelType, formatAttributes);
                    document.KeyColumnName = definitionAttribute?.KeyColumnName ?? document.KeyColumnName;
                    document.FilePattern = definitionAttribute?.FilePattern ??
                        document.FilePattern ?? $"{document.ModelName}_{{fileDate}}.txt";

                    // merge any json and text replacement changes
                    var jsonReplacements = MergeReplacements(document.JsonReplacements);
                    var textReplacements = MergeReplacements(document.TextReplacements);

                    // set regexes
                    document.JsonCleaner = BuildRegexes(jsonReplacements);
                    document.TextCleaner = BuildRegexes(textReplacements);

                }
            }
        }

        protected static Dictionary<int, Func<bool, bool, Encoding>> encodingHelpers = new Dictionary<int, Func<bool, bool, Encoding>>
        {
            { 7, (bom, endian) => new UTF7Encoding() },
            { 8, (bom, endian) => new UTF8Encoding(bom) },
            { 16, (bom, endian) => new UnicodeEncoding(endian, bom) },
            { 32, (bom, endian) => new UTF32Encoding(endian, bom) }
        };

    }

}
