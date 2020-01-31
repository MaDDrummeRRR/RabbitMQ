using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQConsumer
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

            channel.QueueDeclare("RabbitMQQueueTest", true, false, false, null);    // Create Queue. We need it to know exactly if queue is exist or not

            var consumer = new EventingBasicConsumer(channel);      // Create consumer
            consumer.Received += (model, e) =>                      // Subscribe to Received event with lambda expression
            {
                var bode = e.Body;      // Extracting message from body

                Console.WriteLine("Received message: " + Encoding.UTF8.GetString(bode));    // Encoding message from array of bytes and printing it

                Thread.Sleep(3000);     // Emulating some work

                channel.BasicAck(e.DeliveryTag, false);     // Acknowledge message

            };

            channel.BasicConsume(queue: "RabbitMQQueueTest", autoAck: false, consumer: consumer);        // Consume message from queue

            Console.WriteLine("Now waiting for messages... ");
            Console.ReadKey();
        }
    }
}
