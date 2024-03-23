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
                this.Read(socket);
                this.Write(socket);
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
                var message = "(0,0,0)";
                var buffer = Encoding.ASCII.GetBytes(message);
                ns.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
