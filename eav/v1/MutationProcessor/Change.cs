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

        public int EntityTemplateId { get; set; }

        public DateTime EntityStartDate { get; set; }

        public DateTime EntityEndDate { get; set; }

        public bool EntityDeleted { get; set; }

        public int MutationId { get; set; }

        public int FieldId { get; set; }

        public DateTime MutationDateTime { get; set; }

        public DateTime MutationStartDate { get; set; }

        public DateTime MutationEndDate { get; set; }

        // Disabled FieldValueConverter for now, since we might want to leave the
        // conversion to code that reads and consumes the data from the MongoDB (and
        // acts appropriately in case of conversion errors).
        //[JsonConverter(typeof(FieldValueConverter))]
        public string FieldValue { get; set; }

        public bool MutationDeleted { get; set; }
    }
}