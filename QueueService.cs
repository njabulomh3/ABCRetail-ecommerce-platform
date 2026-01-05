    using Azure.Storage.Queues;
    using System.Threading.Tasks;
namespace ABCRetail_Cloud_.Services
{

        public class QueueService
        {
            private readonly QueueClient _queueClient;

            public QueueService(string connectionString, string queueName)
            {
                _queueClient = new QueueClient(connectionString, queueName);
                _queueClient.CreateIfNotExists();
            }

            public async Task SendMessageAsync(string message)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    await _queueClient.SendMessageAsync(message);
                }
            }

            public async Task<string> ReceiveMessageAsync()
            {
                var receivedMessage = await _queueClient.ReceiveMessageAsync();
                if (receivedMessage.Value != null)
                {
                    string messageText = receivedMessage.Value.MessageText;
                    await _queueClient.DeleteMessageAsync(receivedMessage.Value.MessageId, receivedMessage.Value.PopReceipt);
                    return messageText;
                }
                return null;
            }

            public async Task DeleteMessageAsync(string messageId, string popReceipt)
            {
                await _queueClient.DeleteMessageAsync(messageId, popReceipt);
            }
        }

    }
