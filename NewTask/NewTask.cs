using System;
using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
    class NewTask
    {
        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory{ HostName = "localhost" };
			using IConnection connection = connectionFactory.CreateConnection();
			using IModel channel = connection.CreateModel();
			
			channel.QueueDeclare(queue: "task_queue", 
								 durable: false, 
								 exclusive: false, 
								 autoDelete: false, 
								 arguments: null);

            string message = GetMessage(args);
            byte[] body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: "task_queue",
                                 basicProperties: properties,
                                 body: body);
        }

        private static string GetMessage(string[] args)
            => ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}
