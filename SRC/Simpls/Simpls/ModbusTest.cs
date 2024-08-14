using NUnit.Framework;

namespace Simpls
{
    public class MechodCallAttribute : Attribute
    {

    }


    public class Robot
    {

        public void Action_MirWaitForMissionFinish(string args)
        {

        }

        [MechodCall]
        public void Action_URLoadProgram(string program)
        {

        }

        public void Action_URStartProgram(string args) 
        {

        }

        public void Action_URWaitForProgramFinish(string args)
        {

        }

        public void Play()
        {

        }

        public void Pause()
        {

        }

        public void Stop() 
        {

        }

    }


    public class ModbusTest
    {
		[Test]
        public void ReadInt()
        {
            //using (var modbus = new ModbusClient("127.0.01", 502))
            //{
            //    //modbus.WriteSingles(40000, [0.12f, 0.13f, 0.14f]);
            //    //var pi = modbus.ReadSingles(40000, 3);
            //    //modbus.WriteDouble(40000,3.1415926);
            //    //modbus.WriteDoubles(40000, [3.1415926d, 3.1415925d, 3.1415924d]);
            //    //var xx= modbus.ReadDoubles(40000, 3);
            //    //var value = modbus.ReadDouble(40000);
            //    var numbers = modbus.ReadNumbers(40000, 2);
            //}
            TcpPoolFactory.Disponse();
        }



        [Test]
        public void WriteInt()
        {
            //using (var modbus = new ModbusClient("127.0.01", 502))
            //{
            //    var number = modbus.ReadSingle(40000);
            //    Console.WriteLine(number);
            //}
        }
    }


}
