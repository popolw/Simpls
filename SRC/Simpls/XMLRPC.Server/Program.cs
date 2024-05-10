using System.Net;

namespace XMLRPC.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var listener = new HttpListener();
            //listener.Prefixes.Add("http://127.0.0.1:5678/");
            //listener.Start();

            Simpls.HttpListenerServer server = new Simpls.HttpListenerServer(54320);
            server.Use((context, next) => {
                var service = new AddService();
                service.ProcessRequest(context);
                next(context);
            });
            server.Start();
            Console.Read();

            //System.Console.WriteLine("Started Demo service. Press CTRL+C to exit...");
            //while (true)
            //{
            //    var context = listener.GetContext();
            //    StreamReader reader = new StreamReader(context.Request.InputStream);
            //    var c= reader.ReadToEnd();
            //    var service = new AddService();
            //    service.ProcessRequest(context);
            //}
        }
    }
}
