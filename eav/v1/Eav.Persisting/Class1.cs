using System;
using System.Collections.Generic;

namespace Eav.Persisting
{
    public class IPersisting
    {
        public void CreateEntity(int customerId, string templateName, Dictionary<string, object> values)
        {
            
        }
        
        public void UpdateEntity(int customerId, Guid entityId, Dictionary<string, object> updatedValues)
        {
            
        }
        
        public void DeleteEntity(int customerId, Guid entityId)
        {
            
        }
    }
}
