namespace ReadApi.Mapping
{
    internal class FluentEntityMapper<TEntity> : BaseEntityMapper<TEntity> where TEntity : class
    {
        public FluentEntityMapper(EntityMappingConfiguration<TEntity> configuration)
        {
            foreach(var mapping in configuration.Mappings)
            {
                Mappers.Add(mapping.DataElementId,
                    new ElementPropertyMapper<TEntity>(mapping.DataElementId, mapping.Property));
            }
        }


    }
}
