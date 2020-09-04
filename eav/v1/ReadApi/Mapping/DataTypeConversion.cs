namespace ReadApi.Mapping
{
    using System;
    using System.Collections.Concurrent;

    public static class DataTypeConversion
    {
        private static readonly ConcurrentDictionary<Type, DataElementDataType> _cache
            = new ConcurrentDictionary<Type, DataElementDataType>();

        public static DataElementDataType GetDataTypeForType(Type type)
        {
            if (!_cache.ContainsKey(type))
            {
                var result = DetermineDataTypeForType(type);
                _cache.TryAdd(type, result);

                return result;
            }

            return _cache[type];
        }

        private static DataElementDataType DetermineDataTypeForType(Type type)
        {
            foreach (DataElementDataType dataType in Enum.GetValues(typeof(DataElementDataType)))
            {
                if (dataType.IsUsedFor(type))
                    return dataType;
            }

            return DataElementDataType.Undefined;
        }
    }
}
