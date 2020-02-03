using EasyNetQ;
using System;
using System.Threading;

namespace EasyNetQSendMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            //Subscribe();
            //Publish();

            //Receive();
            //Send();

            Response();
            Request();
        }

        #region Publish and Subscribe
        static void Publish()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus("host=localhost");

            while (true)
            {
                var message = new MyMessage
                {
                    Name = rand.RandomString(),
                    ShoeSize = rand.Next(1, 20)
                };

                // if there are no subscribers? this message will be hurtled into the void
                bus.Publish(message);
                Console.WriteLine("Published a message!");
                Thread.Sleep(1000);
            }
        }

        static void Subscribe()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Subscribe<MyMessage>("sub_id", x =>
            {
                Console.WriteLine("Received message: {0},{1}", x.Name, x.ShoeSize);
            });
        }
        #endregion

        #region Receive and Send
        static void Send()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus("host=localhost");

            while (true)
            {
                var message = new MyMessage
                {
                    Name = rand.RandomString(),
                    ShoeSize = rand.Next(1, 20)
                };
                var myOtherMessage = new MyOtherMessage
                {
                    Address = "123 " + rand.RandomString() + " Street",
                    Taxes = Convert.ToDecimal(rand.NextDouble())
                };

                // this messages will be put in the queue even if there aren't any receivers yet
                bus.Send("my.queue", message);
                bus.Send("my.queue", myOtherMessage);
                Console.WriteLine("Sent two different messages!");
                Thread.Sleep(1000);
            }
        }

        static void Receive()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Receive("my.queue", x => x
            .Add<MyMessage>(m =>
            {
                Console.WriteLine("Received MyMessage: {0},{1}", m.Name, m.ShoeSize);
            })
            .Add<MyOtherMessage>(m =>
           {
               Console.WriteLine("Received MyOtherMessage: {0},{1:0.0}", m.Address, m.Taxes);
           }));
        }
        #endregion

        #region Response and Request
        static void Request()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus("host=localhost");

            while (true)
            {
                var message = new MyMessage
                {
                    Name = rand.RandomString(),
                    ShoeSize = rand.Next(1, 20)
                };


                Console.WriteLine("Sending a request...");

                // this can be done asynchronously too
                // if there s no proces to receive requests, this will timeout
                var responce = bus.Request<MyMessage, MyResponse>(message);
                Console.WriteLine(responce.Message);
                Thread.Sleep(1000);
            }
        }

        static void Response()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Respond<MyMessage, MyResponse>(x =>
            {
                return new MyResponse
                {
                    Message = string.Format("Response: {0},{1}", x.Name, x.ShoeSize)
                };
            });
        }
        #endregion
    }
}
