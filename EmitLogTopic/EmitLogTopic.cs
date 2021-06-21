using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace EmitLogTopic
{
    class EmitLogTopic
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

            string routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
            string message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "topic_logs",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");
        }
    }
}
