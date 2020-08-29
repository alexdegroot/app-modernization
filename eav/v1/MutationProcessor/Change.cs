using System;

namespace MutationProcessor
{
    public class Change
    {
        public int TenantId { get; set; }

        public int EntityId { get; set; }

        public DateTime? EntityStartDate { get; set; }

        public DateTime? EntityEndDate { get; set; }

        public bool EntityDeleted { get; set; }

        public int TemplateId { get; set; }

        public int MutationId { get; set; }

        public int FieldId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public object Value { get; set; }

        public bool IsDeleted { get; set; }
    }
}