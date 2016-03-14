using System.Collections.Generic;

namespace DataGenerator.Commands
{
    public class DbDataBoostraperOptions
    {
        public string DbContextAssembly { get; set; }
        public string GeneratorAssembly { get; set; }
        public string GeneratorType { get; set; }
        public string StartupAssembly { get; set; }
        public string Environment { get; set; }

        public bool Recreate { get; set; }
        public string DeleteHost { get; set; }
        public string DeleteDatabase { get; set; }        
    }


}