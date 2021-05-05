using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Dtos.Config
{
    public class CustomLoggingOptions
    {
        public List<string> Using { get; set; }
        public string MinimumLevel { get; set; }
        public List<string> Enrich { get; set; }
        public List<CustomLoggingDB> WriteTo { get;set;}
    }

    public class CustomLoggingDB
    {
        public string Name { get; set; }
        public CustomLoggingDBArgs Args { get; set; }
    }

    public class CustomLoggingDBArgs
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string AutoCreateSqlTable { get; set; }
        public string RestrictedToMinimumLevel { get; set; }
    }
}
