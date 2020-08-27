using System.Collections.Generic;

namespace WriteApi.Mapping
{
    internal abstract class BaseEntityMapper<TEntity> : IEntityMapper<TEntity>
        where TEntity : class
    {
        //private readonly DataTypeConverter _converter = new DataTypeConverter();

        protected Dictionary<int, ElementPropertyMapper<TEntity>> Mappers = new Dictionary<int, ElementPropertyMapper<TEntity>>();

        public void MapToEntity(TEntity entity, DataElementRow dataElementRow)
        {
            if (HasMapper(dataElementRow.Id))
            {
                MapValueToProperty(entity, dataElementRow.Id, dataElementRow.Value);
            }
            //else
            //{
            //    entity.SetUnmappedDataElementValue(dataElementRow.Id,
            //        dataElementRow.ExportTag,
            //        _converter.ConvertTo(dataElementRow.DataType.GetConversionType(), dataElementRow.Value));
            //}
        }

        private void MapValueToProperty(TEntity entity, int dataElementId, string fieldValue)
        {
            Mappers[dataElementId].SetPropertyValue(entity, fieldValue);
        }

        private bool HasMapper(int dataElementId)
        {
            return Mappers.ContainsKey(dataElementId);
        }
    }
}
