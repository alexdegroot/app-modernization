using System;

namespace WriteApi
{
    public class Mutation
    {
        public int EntityId { get; set; }

        public int DataElementId { get; set; }

        public string FieldValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
