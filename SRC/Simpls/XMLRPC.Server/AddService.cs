using Horizon.XmlRpc.Server;
using XMLRPC.Contracts;

namespace XMLRPC.Server
{
    internal class AddService : XmlRpcListenerService, IAddService
    {
        public int AddNumbers(int numberA, int numberB)
        {
            System.Console.WriteLine($"Received request to Demo.addNumbers. Parameters: [{numberA}, {numberB}]");
            return numberA + numberB;
        }

        public string GetClose()
        {
            return "NG";
        }

        public string SetAlarmCode(int alarmCode)
        {
            return "OK";
        }
    }
}
