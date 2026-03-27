namespace projectDemo.Common
{
    public static class EnumHelper
    {
        public static string ToEnumString<T>(this T enumValue) where T : Enum
        {
            return enumValue.ToString();
        }

        public static T ToEnumValue<T>(this string value) where T : struct, Enum
        {
            if (Enum.TryParse<T>(value, true, out var result))
            {
                return result;
            }

            throw new Exception($"Giá trị enum không hợp lệ: {value}");
        }

    }
}
