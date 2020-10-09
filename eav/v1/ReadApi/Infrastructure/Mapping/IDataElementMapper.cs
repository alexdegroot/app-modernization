namespace ReadApi.Infrastructure.Mapping
{
    using System.Collections.Generic;

    public interface IDataElementMapper<in TEntity>
        where TEntity : class
    {
        List<DataElement> MapToDataElements(TEntity entity);
    }
}
