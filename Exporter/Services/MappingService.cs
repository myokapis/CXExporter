using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ExportAttributes;
using Exporter.Models;

namespace Exporter.Services
{

    public interface IMappingService : IService
    {
        IEnumerable<string> GetDocumentData(IDataReader reader);
    }

    internal class MappingService : Service, IMappingService
    {
        private readonly IMetaObjectService metaObjectService;
        private int documentNo = 0;

        internal MappingService(IMetaObjectService metaObjectService)
        {
            this.metaObjectService = metaObjectService;
        }

        public IEnumerable<string> GetDocumentData(IDataReader reader) =>
            string.IsNullOrWhiteSpace(CurrentDocument.KeyColumnName) ? GetSimpleData(reader) : GetCompositeData(reader);

        #region Protected Items

        protected Document CurrentDocument => metaObjectService.Process.Documents[documentNo];

        protected void FormatFieldData(in Document document, in Field field, in Builder builder,
            ProcessMethod processMethod, object fieldData)
        {
            // append a delimiter
            if (field.DisplayIndex > 1) builder.Record.Append(document.Delimiter);

            // append the field value
            if (fieldData == DBNull.Value)
            {
                builder.Record.Append(document.NullValue);
                return;
            }

            // do special processing if a method has been assigned
            var value = processMethod?.Invoke(document, builder, field, fieldData) ?? fieldData;

            // check if field needs to be escaped
            var isEscaped = document.ConditionalEscaping.HasValue
                    && (document.ConditionalEscaping.Value || document.EscapingRegex.IsMatch(value as string));

            if (isEscaped) builder.Record.Append("\"");

            // add the value to the row output
            if (field.FormatString == null)
                builder.Record.Append(value);
            else
                builder.Record.AppendFormat(field.FormatString, value);

            if (isEscaped) builder.Record.Append("\"");
        }


        protected IEnumerable<string> GetCompositeData(IDataReader reader)
        {
            // setup working objects
            var document = CurrentDocument;
            var fieldInfo = metaObjectService.GetFieldInfo(document);
            var builder = metaObjectService.GetBuilder(document);

            // output the headers
            if(GetHeader(document, builder))
            {
                yield return builder.Record.ToString();
                builder.Record.Clear();
            }

            // assign special processing methods to horizontal data fields based on config and data
            SetProcessMethods(fieldInfo, in reader, false);

            // queue the horizontal data
            var rows = QueueHorizontalData(reader, fieldInfo);
            var rowCount = rows.Count;

            // advance to the vertical recordset and get the first record
            reader.NextResult();
            var hasData = reader.Read();
            object currentKey = reader[fieldInfo.KeyColumnName];

            // assign special processing methods to vertical data fields based on config and data
            SetProcessMethods(fieldInfo, in reader, true);

            // dequeue each item and process corresponding vertical data
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var row = rows.Dequeue();
                MergeVerticalData(in fieldInfo, in reader, row, ref hasData, ref currentKey);

                // iterate the fields to format them and append them to the row
                foreach(var field in fieldInfo.AllFields.Skip(1))
                {
                    FormatFieldData(in document, in field, in builder, field.ProcessMethod, row[field.DisplayIndex]);
                }

                yield return builder.Record.ToString();
                builder.Record.Clear();
            }

            documentNo++;
        }

        protected void MergeVerticalData(in FieldInfo fieldInfo, in IDataReader reader, object[] row,
            ref bool hasData, ref object currentKey)
        {
            // get the key value for this row
            var keyValue = row[0];

            // initialize any multitext fields
            fieldInfo.MultiIndices.ForEach(idx => row[idx] = new List<object>());

            while (hasData && keyValue.Equals(currentKey))
            {
                // map vertical data record to field in row[]
                if (fieldInfo.VFieldsByType.TryGetValue(reader["RecordTypeKey"], out var item))
                {
                    // apply a field filter if it exists
                    if (!item.HasFilter || reader[item.FilterColumn].Equals(item.FilterValue))
                    {
                        if (item.FieldType == FieldType.MultiRow)
                            (row[item.Index] as List<object>).Add(reader[item.ColumnName]);
                        else
                            row[item.Index] = reader[item.ColumnName];
                    }
                }

                // advance to the next record
                hasData = reader.Read();
                currentKey = hasData ? reader[fieldInfo.KeyColumnName] : null;
            }
        }

        protected bool GetHeader(Document document, Builder builder)
        {
            var fields = document.Fields.Where(f => !f.IsHidden).ToArray();
            var fieldCount = fields.Count();
            var headers = fields.Select(f => f.Header).Where(h => !string.IsNullOrWhiteSpace(h));
            if (headers.Count() != fieldCount) return false;

            builder.Record.Clear();

            for (var i = 0; i < fieldCount; i++)
            {
                if (i > 0) builder.Record.Append(document.Delimiter);

                var headerText = fields[i].Header;

                foreach (var kvp in document.TextCleaner)
                {
                    if (kvp.Value.IsMatch(headerText)) headerText = kvp.Value.Replace(headerText, kvp.Key);
                }

                var isEscaped = document.ConditionalEscaping.HasValue
                    && (document.ConditionalEscaping.Value || document.EscapingRegex.IsMatch(headerText));

                if (isEscaped) builder.Record.Append("\"");
                builder.Record.Append(headerText);
                if (isEscaped) builder.Record.Append("\"");
            }

            return true;
        }

        protected IEnumerable<string> GetSimpleData(IDataReader reader)
        {
            // setup working objects
            var document = CurrentDocument;
            var fieldInfo = metaObjectService.GetFieldInfo(document);
            var builder = metaObjectService.GetBuilder(document);

            // output the headers
            if (GetHeader(document, builder))
            {
                yield return builder.Record.ToString();
                builder.Record.Clear();
            }

            // assign special processing methods to horizontal data fields based on config and data
            SetProcessMethods(fieldInfo, in reader, false);

            while (reader.Read())
            {
                // iterate the fields to format them and append them to the row
                foreach (var field in fieldInfo.AllFields.Skip(1))
                {
                    FormatFieldData(in document, in field, in builder, field.ProcessMethod, reader[field.ColumnName]);
                }

                yield return builder.Record.ToString();
                builder.Record.Clear();
            }

            documentNo++;
        }

        

        protected object ProcessJsonField(Document document, Builder builder, Field field, object value)
        {
            var fieldValue = value as string;

            if (value == null) return document.NullValue;

            foreach(var kvp in document.JsonCleaner)
            {
                if (kvp.Value.IsMatch(fieldValue)) fieldValue = kvp.Value.Replace(fieldValue, kvp.Key);
            }

            return fieldValue;
        }

        protected object ProcessMultiTextField(Document document, Builder builder, Field field, object value)
        {
            var index = 0;

            // get the field value as a list and return a null field if no data is present
            var list = value as List<object>;
            if (value == null) return document.NullValue;

            builder.Multi.Clear();

            list.ForEach(v =>
            {
                var fieldValue = v as string;

                foreach (var kvp in document.TextCleaner)
                {
                    if (kvp.Value.IsMatch(fieldValue)) fieldValue = kvp.Value.Replace(fieldValue, kvp.Key);
                }

                if (index > 0) builder.Multi.Append(field.MultiRowDelimiter);
                builder.Multi.Append(fieldValue);
                index++;
            });

            return builder.Multi;
        }

        protected object ProcessTextField(Document document, Builder builder, Field field, object value)
        {
            var fieldValue = value as string;

            if (value == null) return document.NullValue;

            foreach (var kvp in document.TextCleaner)
            {
                if (kvp.Value.IsMatch(fieldValue)) fieldValue = kvp.Value.Replace(fieldValue, kvp.Key);
            }

            return fieldValue;
        }

        protected Queue<object[]> QueueHorizontalData(IDataReader reader, FieldInfo fieldInfo)
        {
            var rows = new Queue<object[]>();

            while (reader.Read())
            {
                var row = new object[fieldInfo.AllFieldCount];

                foreach (var field in fieldInfo.HFields)
                {
                    row[field.DisplayIndex] = reader[field.ColumnName];
                }

                rows.Enqueue(row);
            }

            return rows;
        }

        protected void SetProcessMethods(FieldInfo fieldInfo, in IDataReader reader, bool IsVertical)
        {
            var fields = (IsVertical) ? fieldInfo.VFields.Skip(1) : fieldInfo.HFields.Skip(1);

            foreach (var field in fields)
            {
                if (!stringTypes.Contains(field.FieldType))
                {
                    var dataType = reader.GetFieldType(reader.GetOrdinal(field.ColumnName));
                    if (dataType != typeof(string)) continue;
                    field.FieldType = FieldType.String;
                }

                var fieldType = field.FieldType;

                if (field.FieldType == FieldType.JSON)
                    //fieldInfo.ProcessMethods[field.DisplayIndex] = ProcessJsonField;
                    field.ProcessMethod = ProcessJsonField;
                else if (field.FieldType == FieldType.MultiRow)
                    //fieldInfo.ProcessMethods[field.DisplayIndex] = ProcessMultiTextField;
                    field.ProcessMethod = ProcessMultiTextField;
                else
                    //fieldInfo.ProcessMethods[field.DisplayIndex] = ProcessTextField;
                    field.ProcessMethod = ProcessTextField;
            }
        }

        protected static HashSet<FieldType> stringTypes = new HashSet<FieldType> { FieldType.JSON, FieldType.MultiRow, FieldType.String };

        #endregion

    }

}
