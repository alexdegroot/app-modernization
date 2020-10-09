using System.Diagnostics.CodeAnalysis;

namespace ReadApi
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Configuration
    {
        public string Mongo_Connectionstring { get; set; }
    }
}