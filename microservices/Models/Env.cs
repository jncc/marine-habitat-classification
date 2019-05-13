using System;
using dotenv.net;

namespace microservices.Models
{
    public class Env
    {
        public string SITE { get; private set; }
        public string BIOTOPE_BASE_URL { get; private set; }
        public string QUEUE_INDEX { get; private set; }
        public string QUEUE_AWS_REGION { get; private set; }
        public string QUEUE_AWS_ACCESSKEY { get; private set; }
        public string QUEUE_AWS_SECRETACCESSKEY { get; private set; }
        public string SQS_ENDPOINT { get; private set; }
        public string SQS_PAYLOAD_BUCKET { get; private set; }

        public Env(string filePath = ".env")
        {
            DotEnv.Config(filePath: filePath);

            this.SITE = GetVariable("SITE");
            this.BIOTOPE_BASE_URL = GetVariable("BIOTOPE_BASE_URL");
            this.QUEUE_INDEX = GetVariable("QUEUE_INDEX");
            this.QUEUE_AWS_REGION = GetVariable("QUEUE_AWS_REGION");
            this.QUEUE_AWS_ACCESSKEY = GetVariable("QUEUE_AWS_ACCESSKEY");
            this.QUEUE_AWS_SECRETACCESSKEY = GetVariable("QUEUE_AWS_SECRETACCESSKEY");
            this.SQS_ENDPOINT = GetVariable("SQS_ENDPOINT");
            this.SQS_PAYLOAD_BUCKET = GetVariable("SQS_PAYLOAD_BUCKET");
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
