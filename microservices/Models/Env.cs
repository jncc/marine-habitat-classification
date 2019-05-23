using dotenv.net;
using System;
using System.IO;
using System.Reflection;

namespace microservices.Models
{
    public class Env
    {
        public string FLEXSEARCH_URL { get; private set; }
        public string SITE { get; private set; }
        public string BIOTOPE_BASE_URL { get; private set; }
        public string SEARCH_INDEX { get; private set; }
        public string SEARCH_AWS_REGION { get; private set; }
        public string SEARCH_AWS_ACCESSKEY { get; private set; }
        public string SEARCH_AWS_SECRETACCESSKEY { get; private set; }
        public string SQS_ENDPOINT { get; private set; }
        public string SQS_PAYLOAD_BUCKET { get; private set; }
        public string ES_ENDPOINT { get; private set; }

        public Env()
        {
            DotEnv.Config(filePath: Path.Combine(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath, ".env"));

            this.FLEXSEARCH_URL = GetVariable("FLEXSEARCH_URL");
            this.SITE = GetVariable("SITE");
            this.BIOTOPE_BASE_URL = GetVariable("BIOTOPE_BASE_URL");
            this.SEARCH_INDEX = GetVariable("SEARCH_INDEX");
            this.SEARCH_AWS_REGION = GetVariable("SEARCH_AWS_REGION");
            this.SEARCH_AWS_ACCESSKEY = GetVariable("SEARCH_AWS_ACCESSKEY");
            this.SEARCH_AWS_SECRETACCESSKEY = GetVariable("SEARCH_AWS_SECRETACCESSKEY");
            this.SQS_ENDPOINT = GetVariable("SQS_ENDPOINT");
            this.SQS_PAYLOAD_BUCKET = GetVariable("SQS_PAYLOAD_BUCKET");
            this.ES_ENDPOINT = GetVariable("ES_ENDPOINT");
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
