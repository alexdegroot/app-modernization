
namespace WriteApi.Mapping
{
    public class DataElement
    {
        public int Id { get; set; }

        public int EntityId { get; set; }

        public int ParentEntityId { get; set; }

        public EntityType EntityType { get; set; }

        public object Value { get; set; }

        public DataElementDataType DataType { get; set; }

        public DataElement(int id, object value)
        {
            Id = id;
            Value = value;
        }

        public DataElement(int id, int entityId, object value)
        {
            Id = id;
            EntityId = entityId;
            Value = value;
        }
    }
}
