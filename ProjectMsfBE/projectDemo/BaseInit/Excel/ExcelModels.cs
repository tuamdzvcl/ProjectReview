using System.Collections.Generic;

namespace projectDemo.BaseInit.Excel
{
    public class ExcelError
    {
        public int Row { get; set; }
        public string? Column { get; set; }
        public string? Message { get; set; }

        public override string ToString()
        {
            return $"[Dòng {Row}, Cột {Column}: {Message}]";
        }
    }

    public class ExcelImportResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public List<ExcelError> Errors { get; set; } = new List<ExcelError>();
        public bool IsSuccess => Errors.Count == 0;
    }
}
