using System;
using System.Text.Json.Serialization;
using MutationProcessor.Queue;

namespace MutationProcessor
{
    public class Change
    {
        public int TenantId { get; set; }
        
        public int EntityId { get; set; }
        
        public int EntityParentId { get; set; }

        public bool EntityDeleted { get; set; }

        public int EntityTemplateId { get; set; }

        public int MutationId { get; set; }

        public int FieldId { get; set; }

        public DateTime? MutationStartDate { get; set; }

        public DateTime? MutationEndDate { get; set; }

        [JsonConverter(typeof(FieldValueConverter))]
        public object FieldValue { get; set; }

        public bool MutationDeleted { get; set; }
    }
}