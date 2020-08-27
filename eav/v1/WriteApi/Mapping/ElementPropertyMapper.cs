using System;
using System.Reflection;

namespace WriteApi.Mapping
{
    internal class ElementPropertyMapper<TEntity>
        where TEntity : class
    {
        private readonly DataTypeConverter _typeConverter;

        public string PropertyName { get; }

        public int? DataElementId { get; }

        internal Action<TEntity, object> Setter { get; }

        public Type ReturnType { get; }

        public Type EntityType { get; }

        public ElementPropertyMapper(PropertyInfo property)
        {
            _typeConverter = new DataTypeConverter();

            if (property == null)
                throw new ArgumentNullException(nameof(property));

            PropertyName = property.Name;
            ReturnType = property.PropertyType;
            Setter = property.SetValue;
            EntityType = typeof(TEntity);
        }

        public ElementPropertyMapper(int dataElementId, PropertyInfo property)
            : this(property)
        {
            DataElementId = dataElementId;
        }

        public void SetPropertyValue(TEntity instance, object value)
        {
            var val = _typeConverter.ConvertTo(ReturnType, value);
            Setter.Invoke(instance, val);
        }
    }
}
