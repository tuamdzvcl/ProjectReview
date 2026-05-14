using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

namespace projectDemo.BaseInit.Excel
{
    public class ExcelService : IExcelService
    {
        private readonly ILogger<ExcelService> _logger;
        
        // Cache individual property accessors for performance
        private static readonly ConcurrentDictionary<PropertyInfo, object> _propertyCache = new ConcurrentDictionary<PropertyInfo, object>();

        public ExcelService(ILogger<ExcelService> logger)
        {
            _logger = logger;
        }

        public ExcelImportResult<T> Import<T>(Stream fileStream) where T : new()
        {
            var result = new ExcelImportResult<T>();
            try
            {
                using var workbook = new XLWorkbook(fileStream);
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null) return result;

                // Materialize rows to avoid multiple enumerations
                var allRows = worksheet.RangeUsed().RowsUsed().ToList();
                if (allRows.Count == 0) return result;

                var headerRow = allRows[0];
                var dataRows = allRows.Skip(1).ToList();

                // Map header column index to property
                var properties = typeof(T).GetProperties()
                    .Where(p => p.GetCustomAttribute<ExcelColumnAttribute>() != null)
                    .Select(p => new
                    {
                        Property = p,
                        Attribute = p.GetCustomAttribute<ExcelColumnAttribute>()!
                    })
                    .ToList();

                var columnMapping = new Dictionary<int, PropertyInfo>();
                foreach (var cell in headerRow.Cells())
                {
                    var headerName = cell.Value.ToString().Trim();
                    var match = properties.FirstOrDefault(p => p.Attribute.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        columnMapping.Add(cell.Address.ColumnNumber, match.Property);
                    }
                }

                foreach (var row in dataRows)
                {
                    var item = new T();
                    bool hasRowError = false;
                    int rowNumber = row.RowNumber();

                    foreach (var entry in columnMapping)
                    {
                        var cell = row.Cell(entry.Key);
                        var property = entry.Value;
                        string headerName = headerRow.Cell(entry.Key).Value.ToString();

                        try
                        {
                            if (cell.IsEmpty())
                            {
                                // Check if property is non-nullable value type
                                bool isNullable = !property.PropertyType.IsValueType || Nullable.GetUnderlyingType(property.PropertyType) != null;
                                if (!isNullable)
                                {
                                    result.Errors.Add(new ExcelError { Row = rowNumber, Column = headerName, Message = "Dữ liệu không được để trống" });
                                    hasRowError = true;
                                }
                                continue;
                            }

                            var value = GetCellValue(cell, property.PropertyType);
                            property.SetValue(item, value);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error parsing cell at Row {Row}, Column {Col}", rowNumber, headerName);
                            result.Errors.Add(new ExcelError { Row = rowNumber, Column = headerName, Message = "Sai định dạng dữ liệu" });
                            hasRowError = true;
                        }
                    }

                    if (!hasRowError)
                    {
                        var validationContext = new ValidationContext(item);
                        var validationResults = new List<ValidationResult>();
                        if (!Validator.TryValidateObject(item, validationContext, validationResults, true))
                        {
                            foreach (var validationResult in validationResults)
                            {
                                result.Errors.Add(new ExcelError
                                {
                                    Row = rowNumber,
                                    Column = "Logic",
                                    Message = validationResult.ErrorMessage
                                });
                            }
                            hasRowError = true;
                        }
                    }

                    if (!hasRowError)
                    {
                        result.Data.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during Excel import");
                result.Errors.Add(new ExcelError { Row = 0, Column = "System", Message = "Lỗi hệ thống khi đọc file" });
            }

            return result;
        }

        public byte[] Export<T>(List<T> data, string sheetName = "Sheet1")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<ExcelColumnAttribute>() != null)
                .Select(p => new
                {
                    Property = p,
                    Attribute = p.GetCustomAttribute<ExcelColumnAttribute>()!
                })
                .OrderBy(p => p.Attribute.Order)
                .ToList();

            // Setup Header Row
            var headerRow = worksheet.Row(1);
            for (int i = 0; i < properties.Count; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = properties[i].Attribute.Name;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F81BD");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Freeze header row
            worksheet.SheetView.FreezeRows(1);

            // Get cached accessors for performance
            var accessors = GetAccessors<T>(properties.Select(p => p.Property).ToList());

            // Populate Data
            for (int r = 0; r < data.Count; r++)
            {
                var currentRow = worksheet.Row(r + 2);
                for (int c = 0; c < properties.Count; c++)
                {
                    var value = accessors[c](data[r]);
                    var cell = worksheet.Cell(r + 2, c + 1);
                    
                    if (value != null)
                    {
                        cell.Value = XLCellValue.FromObject(value);
                    }

                    // Basic row styling: Borders
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.LightGray;
                }
                
                // Zebra striping for readability
                if (r % 2 == 1)
                {
                    currentRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                }
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GenerateTemplate<T>(string sheetName = "Template")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<ExcelColumnAttribute>() != null)
                .Select(p => new
                {
                    Property = p,
                    Attribute = p.GetCustomAttribute<ExcelColumnAttribute>()!
                })
                .OrderBy(p => p.Attribute.Order)
                .ToList();

            for (int i = 0; i < properties.Count; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = properties[i].Attribute.Name;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F81BD");
                cell.Style.Font.FontColor = XLColor.White;
                
                // Add a comment or validation tip if needed
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private object? GetCellValue(IXLCell cell, Type targetType)
        {
            var rawValue = cell.Value;
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (underlyingType == typeof(string)) return cell.GetFormattedString();
                if (underlyingType == typeof(int)) return Convert.ToInt32(cell.Value.GetNumber());
                if (underlyingType == typeof(long)) return Convert.ToInt64(cell.Value.GetNumber());
                if (underlyingType == typeof(double)) return cell.Value.GetNumber();
                if (underlyingType == typeof(decimal)) return Convert.ToDecimal(cell.Value.GetNumber());
                if (underlyingType == typeof(bool)) return cell.Value.GetBoolean();
                if (underlyingType == typeof(DateTime)) return cell.GetDateTime();

                return Convert.ChangeType(cell.Value, underlyingType);
            }
            catch (Exception ex)
            {
                // Fallback to string-based parsing if direct conversion fails
                var stringValue = cell.Value.ToString();
                try
                {
                    if (underlyingType == typeof(DateTime)) return DateTime.Parse(stringValue);
                    return Convert.ChangeType(stringValue, underlyingType);
                }
                catch
                {
                    throw new InvalidCastException($"Cannot convert '{stringValue}' to {underlyingType.Name}", ex);
                }
            }
        }

        private List<Func<T, object?>> GetAccessors<T>(List<PropertyInfo> properties)
        {
             var list = new List<Func<T, object?>>();
    var type = typeof(T);
    foreach (var prop in properties)
    {
        // Lấy hoặc tạo mới accessor cho từng PropertyInfo một
        var accessor = (Func<T, object?>)_propertyCache.GetOrAdd(prop, p => 
        {
            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.Property(parameter, (PropertyInfo)p);
            var castToObject = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<T, object?>>(castToObject, parameter);
            return lambda.Compile();
        });
        
        list.Add(accessor);
    }
    return list;
        }
    }
}
