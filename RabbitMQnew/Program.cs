using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQnew
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();   // Create new Random variable

            var factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.HostName = "localhost";

            var conn = factory.CreateConnection();  // Create connection

            var channel = conn.CreateModel();       // Create channel on that connection
            var properties = channel.CreateBasicProperties();   // Create properties
            properties.DeliveryMode = 2;                        // Delivery mode: Persistent

            channel.ExchangeDeclare("RabbitMQExchangeTest", ExchangeType.Direct);               // Create Exchange

            channel.QueueDeclare("RabbitMQQueueTest", false, false, false, null);               // Create Queue

            channel.QueueBind("RabbitMQQueueTest", "RabbitMQExchangeTest", "routing key");      // Bind the Queue to the Exchange

            //---------------------------------------Messages with Class------------------------------------------------------------------------------------------

            while (true)
            {
                var message = new MyMessage            // Create text message
                {
                    Address = "Moscow, Russia",
                    ShoeSize = random.Next(1, 20),
                    Name = "Rustam" + random.Next(1, 20)
                };

                var messageBodyString = JsonConvert.SerializeObject(message);                               // Serializing message to JSON format with JsonConvert
                var messageBodyBytes = Encoding.UTF8.GetBytes(messageBodyString);                           // Encode messageBodyString to byte array in UTF8 format

                channel.BasicPublish("RabbitMQExchangeTest", "routing key", properties, messageBodyBytes);        // Publish message to queue with array of bytes

                Console.WriteLine("Published message: " + messageBodyString);

                Thread.Sleep(1000);     // Wait before sending next message
            }
        }
    }
}
