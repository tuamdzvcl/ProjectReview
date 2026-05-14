using System.Collections.Generic;
using System.IO;

namespace projectDemo.BaseInit.Excel
{
    public interface IExcelService
    {
        ExcelImportResult<T> Import<T>(Stream fileStream) where T : new();
        byte[] Export<T>(List<T> data, string sheetName = "Sheet1");
        byte[] GenerateTemplate<T>(string sheetName = "Template");
    }
}
