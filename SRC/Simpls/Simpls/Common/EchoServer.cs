using System.Buffers;
using System.Net.Sockets;
using System.Text;

namespace Simpls
{
    public class EchoServer
    {
        private readonly TcpListener _lister = new TcpListener(2115);

        public void Start()
        {
            _lister.Start();    
            _lister.BeginAcceptSocket(this.Accept,_lister);
        }

        private void Accept(IAsyncResult iar)
        {
            var listener = iar.AsyncState as TcpListener;
            using (var socket = listener.EndAcceptSocket(iar))
            {
                listener.BeginAcceptSocket(Accept, listener);
                socket.SendBufferSize = 1024;
                while (true)
                {
                    this.Read(socket);
                    this.Write(socket);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                }
            }

        }

        private void Read(Socket socket)
        {
            using (NetworkStream ns = new NetworkStream(socket,false))
            {
                var buffer = ArrayPool<byte>.Shared.Rent(256);
                var readCount = ns.Read(buffer, 0, buffer.Length);
                var value = Encoding.ASCII.GetString(buffer, 0, readCount);
                ArrayPool<byte>.Shared.Return(buffer, true);
            }
        }

        private void Write(Socket socket)
        {
            using (var ns = new NetworkStream(socket,false))
            {
                var message = Guid.NewGuid().ToString();
                var buffer = Encoding.ASCII.GetBytes(message);
                ns.Write(buffer, 0, buffer.Length);
            }
        }
    }

    public class EchoClient
    {
        private readonly EchoServer _server = new EchoServer();
        private readonly TcpClient _client = new TcpClient();
        public EchoClient()
        {
            _server.Start();

        }

        public void Start()
        {
            _client.BeginConnect("127.0.0.1", 2115, ConnectAsync, _client);
        }

        private void ConnectAsync(IAsyncResult iar)
        {
            var client = iar.AsyncState as TcpClient;
            client.EndConnect(iar);
            client.ReceiveBufferSize = 1024;
            var ns = client.GetStream();
            while (true)
            {
                this.Write(ns);
                this.Read(ns);
            }
        }

        private void Write(NetworkStream ns)
        {
            var buffer = Encoding.ASCII.GetBytes("hello");
            ns.Write(buffer, 0, buffer.Length);
        }

        private void Read(NetworkStream ns)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(1024);
            var count = ns.Read(buffer, 0, buffer.Length);
            ArraySegment<byte> array = new ArraySegment<byte>(buffer);
            var spans = array.Slice(0, count).AsSpan<byte>();
            var message = Encoding.ASCII.GetString(spans);
            Console.WriteLine(message);
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
