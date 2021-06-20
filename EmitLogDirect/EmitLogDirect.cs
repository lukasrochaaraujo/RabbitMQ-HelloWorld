using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace EmitLogDirect
{
    class EmitLogDirect
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

            string severity = (args.Length > 0) ? args[0] : "info";
            string message = (args.Length > 1)
                             ? string.Join(" ", args.Skip(1).ToArray())
                             : "Hello World";
            
            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "direct_logs",
                                 routingKey: severity,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($" [x] Sent {severity}:{message}");

            Console.Write(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
