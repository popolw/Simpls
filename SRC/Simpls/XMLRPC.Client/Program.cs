using Horizon.XmlRpc.Client;

namespace XMLRPC.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var proxy = XmlRpcProxyGen.Create<IAddServiceProxy>();
            proxy.Url = "http://127.0.0.1:54320";

            //Console.WriteLine("Calling Demo.addNumbers with [3,4]...");
            //var result = proxy.AddNumbers(3, 4);
            //Console.WriteLine("Received result: " + result);
            //var close = proxy.GetClose();
            var xok = proxy.SetAlarmCode(10);
        }
    }
}
