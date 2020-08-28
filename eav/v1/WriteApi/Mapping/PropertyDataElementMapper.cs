using System.Reflection;

namespace WriteApi.Mapping
{
    internal class PropertyDataElementMapper<TEntity> where TEntity : class
    {
        public int DataElementId { get; }

        public PropertyInfo Property { get; }

        public DataElementDataType DataType { get; }

        internal PropertyDataElementMapper(PropertyInfo property, int dataElementId)
        {
            DataElementId = dataElementId;
            Property = property;
            DataType = DataTypeConversion.GetDataTypeForType(property.PropertyType);
        }

        public DataElement GetDataElementForProperty(TEntity entity)
        {
            object value = Property.GetValue(entity);

            if (value == null)
                return null;

            if (Property.PropertyType.IsEnum)
                value = (int)value;

            return new DataElement(DataElementId, value) { DataType = DataType };
        }
    }
}
