using Modbus.Device;
using System.Collections.Concurrent;

public class ModbusClient 
{
    private readonly string _ip;
    private readonly int _port;
    private readonly ConcurrentDictionary<ConnectionInfo, IModbusMaster> _dic = new ConcurrentDictionary<ConnectionInfo, IModbusMaster>();
    public ModbusClient(string ip, int port)
    {
        this._ip = ip;
        this._port = port;
        TcpPoolFactory.OnDisponsed += TcpPoolFactory_OnDisponsed;
    }

    private void TcpPoolFactory_OnDisponsed(object? sender, EventArgs e)
    {
        foreach (var item in _dic)
        {
            try
            {
                item.Value.Dispose();
            }
            catch 
            {

            }
        }
        _dic.Clear();
    }

    public ushort[] ReadNumbers(ushort address, ushort readCount, byte slave = 1)
    {
        return this.Read<ushort>(slave, address, readCount, master => master.ReadHoldingRegisters);
    }

    public ushort ReadNumber(ushort address, byte slave = 1)
    {
        return this.Read<ushort>(slave, address, 1, master => master.ReadHoldingRegisters)[0];
    }

    public bool[] ReadBooleans(ushort address, ushort readCount, byte slave = 1)
    {
        return this.Read<bool>(slave, address, readCount, master => master.ReadCoils);
    }

    public bool ReadBoolean(ushort address, byte slave = 1)
    {
        return this.Read<bool>(slave, address, 1, master => master.ReadCoils)[0];
    }

    private static T[][] GroupArray<T>(T[] array, int groupSize)
    {
        if (groupSize <= 0)
            throw new ArgumentException("Group size must be a positive integer.");

        var groups = array.Select((item, index) => new { item, groupIndex = index / groupSize })
                           .GroupBy(x => x.groupIndex)
                           .Select(g => g.Select(x => x.item).ToArray())
                           .ToArray();
        return groups;
    }

    public float[] ReadSingles(ushort address, ushort readCount, byte slave = 1)
    {
        var array = new float[readCount];
        //float是两位读取
        var source = this.ReadNumbers(address, (ushort)(readCount * 2), slave);
        var pairs = GroupArray(source, 2).ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            var item = pairs[i];
            array[i] = Modbus.Utility.ModbusUtility.GetSingle(item[0], item[1]);
        }
        return array;
    }

    public float ReadSingle(ushort address, byte slave = 1)
    {
        return this.ReadSingles(address, 1, slave)[0];
    }

    private T[] Read<T>(byte slave, ushort address, ushort readCount, Func<IModbusMaster, Func<byte, ushort, ushort, T[]>> onRead)
    {
        var pool = TcpPoolFactory.Create(this._ip, this._port, 1);
        var connection = pool.GetClient();
        IModbusMaster master = _dic.GetOrAdd(connection, (key) => ModbusIpMaster.CreateIp(connection.Client));
        try
        {
            var data = onRead(master).Invoke(slave, address, readCount);
            pool.ReturnClient(connection);
            return data;
        }
        catch
        {
            if (_dic.TryRemove(connection, out master))
            {
                master.Dispose();
            }
            connection.Reset();
            pool.ReturnClient(connection);
            return new T[readCount];
        }
    }

    public void WriteBooleans(ushort address, bool[] data, byte slave = 1)
    {
        this.Write(master => master.WriteMultipleCoils(slave, address, data));
    }

    public void WriteBoolean(ushort address, bool value, byte slave = 1)
    {
        this.Write(master => master.WriteSingleCoil(slave, address, value));
    }

    public void WriteNumbers(ushort address, ushort[] data, byte slave = 1)
    {
        this.Write(master => master.WriteMultipleRegisters(slave, address, data));
    }

    public void WriteNumber(ushort address, ushort value, byte slave = 1)
    {
        this.Write(master => master.WriteSingleRegister(slave, address, value));
    }

    private void Write(Action<IModbusMaster> onWrite)
    {
        var pool = TcpPoolFactory.Create(this._ip, this._port, 1);
        var connection = pool.GetClient();
        IModbusMaster master = _dic.GetOrAdd(connection, (key) => ModbusIpMaster.CreateIp(connection.Client));
        try
        {
            onWrite(master);
            pool.ReturnClient(connection);
        }
        catch
        {
            if (_dic.TryRemove(connection, out master))
            {
                master.Dispose();
            }
            connection.Reset();
            pool.ReturnClient(connection);
        }
    }

    private ushort[] GetUShorts(float value)
    {
        byte[] floatBytes = BitConverter.GetBytes(value);

        // 检查系统默认的字节序，并可能需要反转字节顺序以适应Modbus设备的大端字节序  
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(floatBytes);
        }

        // 如果你的设备需要两个独立的ushort值而不是一个连续的字节数组，  
        // 你可以使用Buffer.BlockCopy来提取这两个ushort  
        ushort[] registers = new ushort[2];
        Buffer.BlockCopy(floatBytes, 0, registers, 0, sizeof(ushort) * 2);

        // 但是，上面的Buffer.BlockCopy调用不会按你期望的方式工作，因为它试图将整个floatBytes数组  
        // 复制到registers数组中，这会导致溢出并且不会正确地分离出两个ushort值。  
        // 因此，你应该手动提取这两个ushort值。  

        registers[0] = (ushort)((floatBytes[0] << 8) | floatBytes[1]); // 假设大端字节序  
        registers[1] = (ushort)((floatBytes[2] << 8) | floatBytes[3]);
        return registers;
    }

    public void WriteSingle(ushort address,float value,byte slave=1)
    {
        var registers = this.GetUShorts(value);
        this.Write(master => {
            master.WriteMultipleRegisters(slave, address, registers);
        });
    }

    public void WriteSingles(ushort address, float[] data, byte slave = 1)
    {
        var list = new List<ushort>();
        for (int i = 0; i < data.Length; i++)
        {
            var registers = this.GetUShorts(data[i]);
            foreach (var register in registers)
            {
                list.Add(register);
            }
        }
        this.Write(master => {
            master.WriteMultipleRegisters(slave, address, list.ToArray());
        });
    }

    private ushort[] GetUShorts(double value)
    {
        byte[] doubleBytes = BitConverter.GetBytes(value);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(doubleBytes);
        }

        ushort[] ushortValues = new ushort[doubleBytes.Length / sizeof(ushort)];
        Buffer.BlockCopy(doubleBytes, 0, ushortValues, 0, doubleBytes.Length);

        for (int i = 0; i < ushortValues.Length; i++)
        {
            ushortValues[i] = (ushort)((doubleBytes[i * 2] << 8) | doubleBytes[i * 2 + 1]);
        }
        return ushortValues;
    }

    public void WriteDouble(ushort address, double value,byte slave=1)
    {
        var shorts = this.GetUShorts(value);
        this.Write(master => master.WriteMultipleRegisters(slave, address, shorts));
    }

    public void WriteDoubles(ushort address, double[] data,byte slave = 1)
    {
        var list = new List<ushort>();
        for (int i = 0; i < data.Length; i++)
        {
            var registers = this.GetUShorts(data[i]);
            foreach (var register in registers)
            {
                list.Add(register);
            }
        }
        this.Write(master => {
            master.WriteMultipleRegisters(slave, address, list.ToArray());
        });
    }

    public double ReadDouble(ushort address, byte slave = 1)
    {
        var source = this.ReadNumbers(address, 4, slave);
        return Modbus.Utility.ModbusUtility.GetDouble(source[0], source[1], source[2], source[3]);
    }

    public double[] ReadDoubles(ushort address,int readCount, byte slave = 1) 
    {
        var array = new double[readCount];
        var source = this.ReadNumbers(address, (ushort)(readCount * 4), slave);
        var pairs = GroupArray(source, 4);
        for (int i = 0; i < array.Length; i++)
        {
            var item = pairs[i];
            array[i] = Modbus.Utility.ModbusUtility.GetDouble(item[0], item[1], item[2], item[3]);
        }
        return array;
    }

}