using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPCServer
{
    class RPCServer
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);

            Console.WriteLine(" [x] Awaiting RPC requests");

            consumer.Received += (model, eventArgs) =>
            {
                string response = null;

                byte[] body = eventArgs.Body.ToArray();
                var properties = eventArgs.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = properties.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    int number = int.Parse(message);

                    Console.WriteLine($" [.] fib({message})");

                    response = Fib(number).ToString();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(" [.] " + ex.Message);
                    response = "";
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: properties.ReplyTo, basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                }
            };
            channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static int Fib(int n)
        {
            if (n == 0 || n == 1)
                return n;

            return Fib(n - 1) + Fib(n - 2);
        }
    }
}
