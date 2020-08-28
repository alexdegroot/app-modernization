using System.Diagnostics.CodeAnalysis;

namespace MutationProcessor
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Configuration
    {
        public string Queue_Connectionstring { get; set; }
        public string Queue_Name { get; set; }
        
        public string Mongo_Connectionstring { get; set; }
    }
}