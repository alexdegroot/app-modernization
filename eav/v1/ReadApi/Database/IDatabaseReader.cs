using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadApi.Database
{
    public interface IDatabaseReader
    {
        Task<Entity> ReadEntity(string tenantCode, int entityId);

        Task<IList<Entity>> ReadEntities(string tenantCode, int parentEntityId, int templateId = 21);
    }
}
