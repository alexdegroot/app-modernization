namespace ReadApi.Mapping
{
    using System;

    internal static class DataElementDataTypeExtensions
    {
        public static Type GetConversionType(this DataElementDataType dataElementDataType)
        {
            switch (dataElementDataType)
            {
                case DataElementDataType.AlphaNumeric:
                    return typeof(string);

                case DataElementDataType.Numeric:
                    return typeof(float);

                case DataElementDataType.Date:
                    return typeof(DateTime);

                case DataElementDataType.Boolean:
                    return typeof(bool);
            }

            return null;
        }

        public static bool IsUsedFor(this DataElementDataType dataElementDataType, Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
                type = underlyingType;

            switch (dataElementDataType)
            {
                case DataElementDataType.AlphaNumeric:
                    return type == typeof(string);
                case DataElementDataType.Numeric:
                    return type == typeof(int) ||
                           type == typeof(long) ||
                           type == typeof(short) ||
                           type == typeof(double) ||
                           type == typeof(float) ||
                           type == typeof(decimal) ||
                           type.IsEnum;
                case DataElementDataType.Date:
                    return type == typeof(DateTime);
                case DataElementDataType.Currency:
                    return type == typeof(double) ||
                           type == typeof(float) ||
                           type == typeof(decimal);
                case DataElementDataType.Boolean:
                    return type == typeof(bool);
            }

            return false;
        }
    }
}