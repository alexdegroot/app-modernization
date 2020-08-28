using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace WriteApi.Mapping
{
    public abstract class EntityMappingConfiguration<TEntity>
        where TEntity : class
    {
        private readonly Dictionary<int, ElementPropertyMapping> _mappings
            = new Dictionary<int, ElementPropertyMapping>();

        protected EntityMappingConfiguration()
        {
            Configure();
        }

        public IEnumerable<ElementPropertyMapping> Mappings => _mappings.Values;

        public ElementPropertyMapping GetMapping(int dataElementId)
        {
            return _mappings.TryGetValue(dataElementId, out var mapping) ? mapping : null;
        }

        public EntityMappingConfiguration<TEntity> HasProperty<TPropertyValue>(Expression<Func<TEntity, TPropertyValue>> property,
            int dataElementId)
        {
            var mapping = new ElementPropertyMapping(dataElementId, GetProperty(property));
            if (_mappings.ContainsKey(dataElementId))
            {
                throw new Exception(
                    $"Data element ID {dataElementId} has already been mapped for {mapping.Property.Name}");
            }

            _mappings.Add(dataElementId, mapping);
            return this;
        }

        protected abstract void ConfigureMapping();

        private static PropertyInfo GetProperty<TPropertyValue>(Expression<Func<TEntity, TPropertyValue>> property)
        {
            var member = (MemberExpression)property.Body;
            var propertyName = member.Member.Name;
            return typeof(TEntity).GetProperty(propertyName);
        }

        private void Configure()
        {
            ConfigureMapping();
        }
    }
}
