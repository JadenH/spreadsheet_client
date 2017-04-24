using ReactiveProtobuf.Protocol;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReactiveSockets;

namespace Network
{
    /// <summary>
    /// Implements a communication channel over a socket that 
    /// has a fixed length header and a variable length string 
    /// payload.
    /// </summary>
    public class StringChannel : IChannel<string>
    {
        private Encoding encoding;
        private ReactiveClient socket;

        /// <summary>
        /// Initializes the channel with the given socket, using 
        /// the default UTF8 encoding for messages.
        /// </summary>
        public StringChannel(ReactiveClient socket)
            : this(socket, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes the channel with the given socket, using 
        /// the given encoding for messages.
        /// </summary>
        public StringChannel(ReactiveClient socket, Encoding encoding)
        {
            this.socket = socket;
            this.encoding = encoding;

            Receiver = from data in socket.Receiver
                       let body = socket.Receiver.TakeWhile(b => b != (byte)'\n')
                       select Encoding.UTF8.GetString(body.ToEnumerable().ToArray());
        }

        public IObservable<string> Receiver { get; private set; }

        public Task SendAsync(string message)
        {
            return socket.IsConnected ? socket.SendAsync(Convert(message + "\n")) : null;
        }

        internal byte[] Convert(string message)
        {
            return encoding.GetBytes(message);
        }

        public void Disconnect()
        {
            if (socket.IsConnected)
            {
                socket.Disconnect();
                socket.Dispose();
            }

        }
    }
}
