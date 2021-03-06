﻿namespace ReadApi.Infrastructure.Mapping
{
    public interface IEntityMapper<in TEntity>
        where TEntity : class
    {
        void MapToEntity(TEntity entity, DataElementRow dataElementRow);
    }
}
