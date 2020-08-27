using System.Collections.Generic;

namespace WriteApi.Mapping
{
    public interface IDataElementMapper<in TEntity>
        where TEntity : class
    {
        List<DataElement> MapToDataElements(TEntity entity);
    }
}
