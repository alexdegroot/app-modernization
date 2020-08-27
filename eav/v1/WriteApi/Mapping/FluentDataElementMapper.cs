namespace WriteApi.Mapping
{
    internal class FluentDataElementMapper<TEntity> : DataElementMapper<TEntity> where TEntity : class
    {
        public FluentDataElementMapper(EntityMappingConfiguration<TEntity> configuration)
        {
            foreach (var mapping in configuration.Mappings)
            {
                var property = mapping.Property;
                var dataElementId = mapping.DataElementId;

                DefineMapping(dataElementId, property);
            }
        }
    }
}
