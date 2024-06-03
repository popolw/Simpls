using Modbus.Device;
using System.Collections.Concurrent;

public class ModbusClient : IDisposable
{
    private readonly string _ip;
    private readonly int _port;
    private readonly ConcurrentDictionary<ConnectionInfo, IModbusMaster> _dic = new ConcurrentDictionary<ConnectionInfo, IModbusMaster>();
    public ModbusClient(string ip, int port)
    {
        this._ip = ip;
        this._port = port;
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

    static IEnumerable<ushort[]> GroupByPairs(ushort[] source)
    {
        for (int i = 0; i < source.Length; i += 2)
        {
            if (i + 1 < source.Length)
            {
                yield return new[] { source[i], source[i + 1] };
            }
            else
            {
                yield return new[] { source[i] };
            }
        }
    }

    public float[] ReadSingles(ushort address, ushort readCount, byte slave = 1)
    {
        var array = new float[readCount];
        var source = this.ReadNumbers(address, (ushort)(readCount * 2), slave);
        var pairs = GroupByPairs(source).ToArray();
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

    public void Dispose()
    {

    }
}