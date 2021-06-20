using RabbitMQ.Client;
using System;
using System.Text;

namespace EmitLog
{
    class EmitLog
    {
        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "logs",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            
            Console.WriteLine($" [x] Sent {message}");

            Console.Write(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
            => ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
    }
}
