namespace ReadApi.Mapping
{
    using System;
    using System.Globalization;

    internal class DataTypeConverter
    {
        private readonly CultureInfo _culture;

        public DataTypeConverter()
            : this(new CultureInfo("nl"))
        {
        }

        public DataTypeConverter(CultureInfo cultureInfo)
        {
            _culture = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        }

        public virtual object ConvertTo(Type type, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (type == typeof(string))
            {
                return value.ToString();
            }

            return ConvertToValueType(value, GetResultType(type), _culture);
        }

        private object ConvertToValueType(object value, Type type, CultureInfo culture)
        {
            if (type.IsEnum && int.TryParse(value.ToString(), out int enumValue)
                            && Enum.IsDefined(type, enumValue))
                return Enum.Parse(type, value.ToString() ?? string.Empty);

            if (type == typeof(int))
                return Convert.ToInt32(value, _culture.NumberFormat);

            if (type == typeof(long))
                return Convert.ToInt64(value, _culture.NumberFormat);

            if (type == typeof(short))
                return Convert.ToInt16(value, _culture.NumberFormat);

            if (type == typeof(DateTime))
                return Convert.ToDateTime(value, _culture.DateTimeFormat);

            if (type == typeof(decimal))
                return Convert.ToDecimal(value, _culture.NumberFormat);

            if (type == typeof(double))
                return Convert.ToDouble(value, _culture.NumberFormat);

            if (type == typeof(float))
                return Convert.ToSingle(value, _culture.NumberFormat);

            if (type == typeof(bool))
                return Convert.ToBoolean(value);

            if (type == typeof(char))
                return Convert.ToChar(value);

            if (type == typeof(byte))
                return Convert.ToByte(value, _culture.NumberFormat);

            if (type == typeof(sbyte))
                return Convert.ToSByte(value, _culture.NumberFormat);

            if (type == typeof(uint))
                return Convert.ToUInt32(value, _culture.NumberFormat);

            if (type == typeof(ulong))
                return Convert.ToUInt64(value, _culture.NumberFormat);

            if (type == typeof(ushort))
                return Convert.ToUInt16(value, _culture.NumberFormat);

            return value;
        }

        private static Type GetResultType(Type destinationType)
        {
            var valueType = destinationType;
            return destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? destinationType.GetGenericArguments()[0]
                : valueType;
        }
    }
}
