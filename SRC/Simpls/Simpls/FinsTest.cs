using NUnit.Framework;
using System.Net;
using System.Text.RegularExpressions;

namespace Simpls
{

    public class FinsTest
    {

        private readonly Regex _regex = new Regex("^(W|D)(0|([1-9])\\d*)((\\.\\d+){0,1})$");

        public enum FArea : byte
        {
            DataMemory = 130,
            CommonIO = 48,
            Work = 49,
            Holding = 50,
            Auxiliary = 51
        }

        private struct PLCAddress
        {
            public FArea Area { get; private set; }
            public ushort Address { get; private set; }
            public byte? Position { get; private set; }

            public PLCAddress(FArea area, ushort address, byte? position)
            {
                Area = area;
                Address = address;
                Position = position;
            }

        }


        [Test]
        public  void Read()
        {

            //CableRobot.Fins.FinsClient client = new CableRobot.Fins.FinsClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9600));
            //var x = client.ReadData(100, 2);
            //client.Close();

            PLC_Omron_Standard.PlcOmron plc = new PLC_Omron_Standard.PlcOmron(IPAddress.Parse("127.0.0.1"), 9600, false, 129, 10);
            plc.Connect();
            plc.Write(200, new[] { 433.33f, 422.22f, 411.11f },1);
            var x= plc.ReadFloatArray((ushort)200,3);
            plc.Disconnect();
            Task.Delay(1000).Wait();

            //plc.Write(101, (ushort)2);


            //var array = plc2.Read(100, 2);


            //var array = plc.ReadShortArray(100, 2);

            //plc.Write(101, (ushort)2.23);
            //plc.Write(1, (ushort)2.23);
            //plc.Write(1, (float)1.345);
            //var v= plc.ReadFloat(1);
            //plc.Write(2, (float)4.36);
            //var array = plc.ReadFloatArray(100,2);
            //plc.Write(0, (ushort)15);
            //plc.Write(2,(short)1);
            //var value = plc.ReadUShort(2);
            //var x = plc.Write(0, new byte[] { 1, 0 }, 1);

            //var ar= plc.ReadUShortArray(0,7);

        }

        private PLCAddress? ToAddress(string address)
        {
            var match = _regex.Match(address);
            if (match.Success)
            {
                var area = address[0] == 'W' ? FArea.Work : FArea.DataMemory;
                var array = address.TrimStart('W').TrimStart('D').Split('.');
                return new PLCAddress(area, Convert.ToUInt16(array[0]), array.Length == 2 ? Convert.ToByte(array[1]) : null);
            }
            return null;
        }

        [Test]
        public void PaseAddress()
        {
            var address = ToAddress("W0.");
        }

    }
}