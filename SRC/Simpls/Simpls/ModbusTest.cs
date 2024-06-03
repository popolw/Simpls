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
                var number = modbus.ReadSingles(40000, 1);

                Console.WriteLine(number);
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
