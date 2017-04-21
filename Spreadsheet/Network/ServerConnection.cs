using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using ReactiveSockets;
using SpreadsheetGUI;

namespace Network
{
    public class ServerConnection: ISpreadsheetServer
    {
        private string _spreadsheetName;

        public event Action<string> MessageReceived;

        public StringChannel Protocol { get; private set; }

        public void Connect(string ipaddress, string spreadsheetName)
        {
            _spreadsheetName = spreadsheetName;
            var client = new ReactiveClient(ipaddress, 2112);
            Protocol = new StringChannel(client);

            Protocol.Receiver.Subscribe(ReceiveMessage);

            client.Connected += ClientOnConnected;
            client.ConnectAsync().Wait();
        }

        private void ReceiveMessage(string s)
        {
            Console.WriteLine(s);
            MessageReceived?.Invoke(s);
        }

        public void SendMessage(string s)
        {
            Protocol.SendAsync(s);
        }

        private void ClientOnConnected(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Connected");
            Protocol.SendAsync($"Connect\t{_spreadsheetName}\t");
        }

    }
}
