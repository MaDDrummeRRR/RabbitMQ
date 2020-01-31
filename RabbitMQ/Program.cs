using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.HostName = "localhost";

            var conn = factory.CreateConnection();  // Create connection

            var channel = conn.CreateModel();       // Create channel on that connection

            channel.ExchangeDeclare("RabbitMQExchangeTest", ExchangeType.Direct);               // Create Exchange

            channel.QueueDeclare("RabbitMQQueueTest", false, false, false, null);               // Create Queue

            channel.QueueBind("RabbitMQQueueTest", "RabbitMQExchangeTest", "routing key");      // Bind the Queue to the Exchange

            for (var i = 0; i < 5; i++)
            {
                var message = string.Format("Message #{0}: {1}", i, Guid.NewGuid());            // Create text message
                var messageBodyBytes = Encoding.UTF8.GetBytes(message);                         // Encode message to byte array in UTF8 format

                channel.BasicPublish("RabbitMQExchangeTest", "routing key", null, messageBodyBytes);    // Publish message to queue with array of bytes



                Console.WriteLine("Published message: " + message);
            }






            channel.Dispose();                      // Release channel in that connection
            conn.Dispose();                         // Release resources!!!!!
        }
    }
}
