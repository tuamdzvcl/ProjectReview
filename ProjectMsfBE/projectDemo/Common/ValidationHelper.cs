using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace projectDemo.Common
{
    public static class ValidationHelper
    {
        public static string NormalizeSpaces(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }
            string trimmed = input.Trim();
            return Regex.Replace(trimmed, @"\s+", " ");
        }
        
        public static bool HasSpecialCharacters(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            var regex = new Regex(@"^[\p{L}\p{N}\s]+$");          
            return !regex.IsMatch(input);
        }

        public static void NormalizeAllStrings<T>(T obj)
        {
            if (obj == null) return;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    prop.SetValue(obj, NormalizeSpaces(value));
                }
            }
        }

        public static bool HasSpecialCharactersInAny(params string[] inputs)
        {
            foreach (var input in inputs)
            {
                if (HasSpecialCharacters(input)) return true;
            }
            return false;
            
        }
    }
}
