using dotenv.net;
using System;
using System.IO;
using System.Reflection;

namespace website.Models
{
    public class Env
    {
        public string JNCC_SEARCH_URL { get; private set; }
        public string MICROSERVICE_URL { get; private set; }
        public string FLEXSEARCH_URL { get; private set; }
        public string MAPPER_URL { get; private set; }

        public Env()
        {
            DotEnv.Config(filePath: Path.Combine(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath, ".env"));
            
            this.JNCC_SEARCH_URL = GetVariable("JNCC_SEARCH_URL");
            this.MICROSERVICE_URL = GetVariable("MICROSERVICE_URL");
            this.FLEXSEARCH_URL = GetVariable("FLEXSEARCH_URL");
            this.MAPPER_URL = GetVariable("MAPPER_URL");
        }

        string GetVariable(string name, bool required = true, string defaultValue = null)
        {
            string value = Environment.GetEnvironmentVariable(name) ?? defaultValue;

            if (required && value == null)
                throw new Exception($"Environment variable {name} is required but not set.");

            return value;
        }
    }
}
