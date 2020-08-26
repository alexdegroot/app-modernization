using System.Diagnostics.CodeAnalysis;

namespace MutationExtractor
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Configuration
    {
        public string Sql_Connectionstring { get; set; }
        public string Queue_Connectionstring { get; set; }
        public string Queue_Name { get; set; }
    }
}