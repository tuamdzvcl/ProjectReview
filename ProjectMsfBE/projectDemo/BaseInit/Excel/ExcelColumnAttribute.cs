using System;

namespace projectDemo.BaseInit.Excel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnAttribute :Attribute
    {
        public string Name { get; }
        public int Order { get; }

        public ExcelColumnAttribute(string name, int order = 0)
        {
            Name = name;
            Order = order;
        }
    }
}
