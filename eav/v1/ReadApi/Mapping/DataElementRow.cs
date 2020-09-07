
namespace ReadApi.Mapping
{
    public class DataElementRow
    {
        public int Id { get; }

        public string Value { get; }

        public DataElementDataType DataType { get; }

        public string ExportTag { get; }

        public DataElementRow(int id, string value, DataElementDataType dataType, string exportTag = null)
        {
            Id = id;
            Value = value;
            DataType = dataType;
            ExportTag = exportTag;
        }
    }
}
