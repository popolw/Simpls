using NUnit.Framework;

namespace Simpls
{


    public class ModbusTest
    {
		[Test]
        public void ReadInt()
        {
            using (var modbus = new ModbusClient("127.0.01", 502))
            {
                //modbus.WriteSingles(40000, [0.12f, 0.13f, 0.14f]);
                //var pi = modbus.ReadSingles(40000, 3);
                //modbus.WriteDouble(40000,3.1415926);
                //modbus.WriteDoubles(40000, [3.1415926d, 3.1415925d, 3.1415924d]);
                //var xx= modbus.ReadDoubles(40000, 3);
                //var value = modbus.ReadDouble(40000);

                var a = sizeof(float);
                var d= sizeof(decimal);
                var x = sizeof(double);
            }

        }

        [Test]
        public void WriteInt()
        {
            using (var modbus = new ModbusClient("127.0.01", 502))
            {
                var number = modbus.ReadSingle(40000);
                Console.WriteLine(number);
            }
        }
    }


}
