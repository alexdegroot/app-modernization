using System.Reflection;

namespace WriteApi.Mapping
{
    public class ElementPropertyMapping
    {
        public int DataElementId { get; }

        public PropertyInfo Property { get; }

        public ElementPropertyMapping(int dataElementId, PropertyInfo property)
        {
            DataElementId = dataElementId;
            Property = property;
        }
    }
}
