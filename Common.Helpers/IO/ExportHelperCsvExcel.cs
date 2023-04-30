using ClosedXML.Excel;
using CsvHelper;
using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Common.Helpers.IO
{
    public enum ExportFileType
    {
        Csv = 0,
        Xlsx = 1,
    }

    public static class ExportHelperCsvExcel
    {
        public static string GetName<T>(this T item) where T : Enum //,Nullable<Enum>
            => item != null ? Enum.GetName(typeof(T), item) : "";

        public static string ToEnumeratedNames<T>(
            this IEnumerable<T?> data, string div = ", ") where T : struct, Enum //,Nullable<Enum>
            => data?.Select(d => d?.GetName()).ToEnumeratedString(div) ?? "";

        private const string ExcelDateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        private static Dictionary<ExportFileType, string> _suffix = new Dictionary<ExportFileType, string>()
        {
            { ExportFileType.Csv, ".csv" },
            { ExportFileType.Xlsx, ".xlsx" },
        };

        internal static ILogger logger = LogProvider.GetLogger(nameof(ExportHelperCsvExcel));

        public static bool TryGetFile(
            this ExportFileType fileType, ref string filePath)
        {
            if (_suffix.TryGetValue(fileType, out string suffix))
                filePath += suffix;

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            return !fileInfo.IsFileLocked();
        }

        public static void WriteToFile<T>(this IEnumerable<T> data, string filePath, string fileName = "",
            ExportFileType fileType = ExportFileType.Xlsx, int[] hideColumnIndex = null, int[] dateColumnIndex = null)
        {
            try
            {
                string fullPath = Path.Combine(filePath, fileName);

                if (_suffix.TryGetValue(fileType, out string suffix))
                    FileHandler.ValidateExtension(ref fullPath, suffix);

                if (fileType.Equals(ExportFileType.Xlsx))
                {
                    using (var workbook = data.WriteToWorkbook(hideColumnIndex, dateColumnIndex))
                        workbook.SaveAs(fullPath);
                }
                else if (fileType.Equals(ExportFileType.Csv))
                {
                    using (var writer = new StreamWriter(fullPath, false))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        csv.WriteRecords(data);
                }
                else
                    throw new NotImplementedException(fileType.GetName());

                logger.LogDebug("Exported to '{0}'.", fullPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "File Export Error");
            }
        }

        public static string WriteToExcel<T>(this IEnumerable<T> data, string filePath, string fileName = "", params int[] hideColumnIndex)
        {
            string fullPath = Path.Combine(filePath, fileName);
            return data.WriteToExcel(fullPath, hideColumnIndex);
        }

        public static string WriteToExcel<T>(this IEnumerable<T> data, string filePath, params int[] hideColumnIndex)
        {
            try
            {
                if (_suffix.TryGetValue(ExportFileType.Xlsx, out string suffix))
                    filePath = FileHandler.GetValidatedFileName(filePath, suffix);

                using (var workbook = data.WriteToWorkbook(hideColumnIndex))
                    workbook.SaveAs(filePath);
                logger.LogDebug("Exported to '{0}'.", filePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "File Export Error");
            }
            return filePath;
        }

        public static MemoryStream WriteToStream<T>(this IEnumerable<T> data, ref string fileName, params int[] hideColumnIndex)
        {
            var outputStream = new MemoryStream();
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    fileName = DateTime.Now.ToDateTimeName();
                if (_suffix.TryGetValue(ExportFileType.Xlsx, out string suffix))
                    fileName = FileHandler.GetValidatedFileName(fileName, suffix);

                using (var workbook = data.WriteToWorkbook(hideColumnIndex))
                    workbook.SaveAs(outputStream);
                outputStream.Position = 0;

                logger.LogDebug("Exported '{0}' to stream.", fileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "File Export Error");
            }
            return outputStream;
        }

        public static XLWorkbook WriteToWorkbook<T>(this IEnumerable<T> data, int[] hideColumnIndex = null, int[] dateColumnIndex = null)
        {
            var workbook = new XLWorkbook();
            if (data.IsNotNullOrEmpty())
            {
                workbook.Worksheets.Add(data.CreateDataTable())
                    .Columns().AdjustToContents();
                var ws = workbook.Worksheets.FirstOrDefault();
                if (hideColumnIndex?.Length > 0)
                    foreach (var toHide in hideColumnIndex)
                        ws.Column(toHide).Hide();
                if (dateColumnIndex?.Length > 0)
                    foreach (var toDate in dateColumnIndex)
                        ws.Column(toDate).Style.DateFormat.Format = ExcelDateTimeFormat;
            }
            return workbook;
        }
    }
}

internal static class DataTableExtensionMethods
{
    /// <summary>
    /// Converts an IEnumerable object to a data table of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static DataTable CreateDataTable<T>(
        this IEnumerable<T> list)
    {
        Type type = typeof(T);
        var properties = type.GetProperties();

        DataTable dataTable = new DataTable
        {
            TableName = type.Name
        };

        foreach (var heading in properties)
        {
            dataTable.Columns.Add(new DataColumn(heading.Name.Replace('_', ' '),
                Nullable.GetUnderlyingType(heading.PropertyType)
                ?? heading.PropertyType));
        }

        foreach (T entity in list)
        {
            object[] values = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                values[i] = properties[i].GetValue(entity);
            }
            dataTable.Rows.Add(values);
        }

        dataTable.AcceptChanges();
        return dataTable;
    }
}
