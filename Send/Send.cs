using System;
using System.Text;
using RabbitMQ.Client;

namespace Send
{
	internal class Send
	{
		private static void Main(string[] args)
		{
			ConnectionFactory connectionFactory = new ConnectionFactory{ HostName = "localhost" };
			using IConnection connection = connectionFactory.CreateConnection();
			using IModel channel = connection.CreateModel();
			
			channel.QueueDeclare(queue: "hello", 
								 durable: false, 
								 exclusive: false, 
								 autoDelete: false, 
								 arguments: null);

			string messageText = "Hello World";
			byte[] messageBytes = Encoding.UTF8.GetBytes(messageText);

			channel.BasicPublish(exchange: "",
								 routingKey: "hello", 
								 basicProperties: null,
								 body: messageBytes);

			Console.WriteLine(" [X] Sent " + messageText);
			Console.WriteLine(" Press [enter] to exit");
			Console.ReadLine();
		}
	}
}
