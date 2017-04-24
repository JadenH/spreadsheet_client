using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using ReactiveSockets;
using SpreadsheetGUI;
using System.Windows.Forms;

namespace Network
{
    public class ServerConnection: ISpreadsheetServer
    {
      
        private string _spreadsheetName;

        public event Action<string> MessageReceived;
        public event Action ClientDisconnected;

        public StringChannel Protocol { get; private set; }

        public void Connect(string ipaddress, string spreadsheetName)
        {
            _spreadsheetName = spreadsheetName;
            var client = new ReactiveClient(ipaddress, 2112);
            Protocol = new StringChannel(client);

            Protocol.Receiver.Subscribe(ReceiveMessage);

            client.Disconnected += ClientOnDisconnected;
            client.Connected += ClientOnConnected;

            //Lock this object so that client cannot send a message while receiving one.
            lock (this)
            {
                try
                {
                    client.ConnectAsync().Wait();
                }
                catch
                {

                    MessageBox.Show("Could not connect to server! Check that the server is running and that you typed the ip correctly.", "Disconnect",
                         MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClientDisconnected?.Invoke();
                }
            }
        
        }

        public void Disconnect()
        {
            Protocol.Disconnect();
        }

        private void ReceiveMessage(string s)
        {
            Console.WriteLine(s);
            MessageReceived?.Invoke(s);
        }

        public void SendMessage(string s)
        {
            lock (this)
            {
                Protocol.SendAsync(s);
            }
        }

        private void ClientOnConnected(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Connected");
            Protocol.SendAsync($"Connect\t{_spreadsheetName}\t");
        }

        private void ClientOnDisconnected(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Disconnected");
            ClientDisconnected?.Invoke();
        }


    }
}
