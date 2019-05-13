using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using microservices.Models;

namespace microservices.Clients
{
    public interface IQueueClient
    {
        Task Send(string message);
    }

    public class QueueClient : IQueueClient, IDisposable
    {
        private readonly Env env;
        private readonly AmazonS3Client s3;
        private readonly AmazonSQSClient sqs;
        private readonly AmazonSQSExtendedClient client;

        public QueueClient(Env env)
        {
            this.env = env;
            var credentials = new BasicAWSCredentials(env.QUEUE_AWS_ACCESSKEY, env.QUEUE_AWS_SECRETACCESSKEY);
            var region = RegionEndpoint.GetBySystemName(env.QUEUE_AWS_REGION);

            this.s3 = new AmazonS3Client(credentials, region);
            this.sqs = new AmazonSQSClient(credentials, region);
            this.client = new AmazonSQSExtendedClient(sqs, new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3, env.SQS_PAYLOAD_BUCKET));
        }

        public async Task Send(string message)
        {
            await SendMessage(client, env.SQS_ENDPOINT, message);
        }

        public static async Task<SendMessageResponse> SendMessage(AmazonSQSExtendedClient client, string endpoint, string message)
        {
            return await client.SendMessageAsync(endpoint, message);
        }

        public void Dispose()
        {
            client.Dispose();
            sqs.Dispose();
            s3.Dispose();
        }
    }
}
