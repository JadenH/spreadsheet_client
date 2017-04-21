using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using ReactiveSockets;

namespace Network
{
    public class ServerConnection
    {
        private const string IPADDRESS = "155.98.111.70";

        //Logan's machine 155.98.111.70

        public StringChannel Protocol { get; private set; }

        public void Connect()
        {
            var client = new ReactiveClient(IPADDRESS, 2112);
            //            client.SetSocketOption(SocketOptionLevel.Tcp, );
            Protocol = new StringChannel(client);

            // The parsing of messages is done in a simple Rx query over the receiver observable
            // Note this protocol has a fixed header part containing the payload message length
            // And the message payload itself. Bytes are consumed from the client.Receiver 
            // automatically so its behavior is intuitive.
            IObservable<string> messages = from header in client.Receiver.Buffer(4)
                                           let length = BitConverter.ToInt32(header.ToArray(), 0)
                                           let body = client.Receiver.Take(length)
                                           select Encoding.UTF8.GetString(body.ToEnumerable().ToArray());

            // Finally, we subscribe on a background thread to process messages when they are available
            messages.SubscribeOn(TaskPoolScheduler.Default).Subscribe(Console.WriteLine);

            client.Connected += ClientOnConnected;

            client.ConnectAsync().Wait();
        }

        private void ClientOnConnected(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Connected");
            Protocol.SendAsync("Sent from client");
        }




    }
}
