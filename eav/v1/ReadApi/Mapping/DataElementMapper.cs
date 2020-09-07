namespace ReadApi.Mapping
{
    using System.Collections.Generic;
    using System.Reflection;

    internal abstract class DataElementMapper<TEntity> : IDataElementMapper<TEntity>
        where TEntity : class
    {
        private readonly Dictionary<int, PropertyDataElementMapper<TEntity>> _propertyMappersByDataElementId =
            new Dictionary<int, PropertyDataElementMapper<TEntity>>();

        protected void DefineMapping(int dataElementId, PropertyInfo property)
        {
            var mapper = new PropertyDataElementMapper<TEntity>(property, dataElementId);
            _propertyMappersByDataElementId.Add(dataElementId, mapper);
        }

        public List<DataElement> MapToDataElements(TEntity entity)
        {
            var result = new List<DataElement>();

            foreach (var propertyMapping in _propertyMappersByDataElementId.Values)
            {
                var dataElement = propertyMapping.GetDataElementForProperty(entity);
                if (dataElement != null)
                {
                    result.Add(dataElement);
                }
            }

            return result;
        }
    }
}
