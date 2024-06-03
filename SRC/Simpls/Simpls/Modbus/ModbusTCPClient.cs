using System.Net.Sockets;

public class ModbusTCPClient:IDisposable 
{
    private string ip = "";
    private int port = 502;
    private byte station = 1;
    public bool isStartWithZero = false;
    public DataFormat format = DataFormat.ABCD;
    private readonly TcpClientPool _pool;

    public ModbusTCPClient(string ip) : this(ip, 502, 1) { }

    public ModbusTCPClient(string ip, int port, byte station)
    {
        this.ip = ip;
        this.port = port;
        this.station = station;
        _pool = TcpPoolFactory.Create(this.ip, this.port, 1);
    }

    public ModbusTCPClient(string ip, int port, byte station, Func<string, int, TcpClientPool> poolfun)
    {
        this.ip = ip;
        this.port = port;
        this.station = station;
        this._pool = poolfun(ip, port);
    }

    public Result<bool> ReadBool(string address)
    {
        Result<bool> r = new Result<bool>();
        var tempr = ReadBool(address, 1);
        if (tempr != null && tempr.isSuccess)
        {
            r.isSuccess = true;
            r.result = tempr.result[0];
        }
        return r;
    }

    public Result<bool[]> ReadBool(string address, ushort length)
    {
        Result<bool[]> r = new Result<bool[]>();
        if (GetCommonReadResultBytes(address, length,1, out byte[] receiveBytes))
        {
            r.isSuccess = true;
            r.result = ByteTransform.ByteToBoolArray(receiveBytes, length);
        }
        return r;
    }

    public Result<ushort> ReadUshort(string address)
    {
        Result<ushort> r = new Result<ushort>();
        var tempr = ReadUshort(address, 1);
        if (tempr != null && tempr.isSuccess)
        {
            r.isSuccess = true;
            r.result = tempr.result[0];
        }
        return r;
    }

    public Result<ushort[]> ReadUshort(string address, ushort length)
    {
        Result<ushort[]> r = new Result<ushort[]>();
        if (GetCommonReadResultBytes(address, length,3, out byte[] receiveBytes))
        {
            r.isSuccess = true;
            r.result = ByteTransform.TransUInt16(receiveBytes, 0, length);
        }
        return r;
    }

    public Result<short> ReadShort(string address)
    {
        Result<short> r = new Result<short>();
        var tempr = ReadShort(address, 1);
        if (tempr != null && tempr.isSuccess)
        {
            r.isSuccess = true;
            r.result = tempr.result[0];
        }
        return r;
    }

    public Result<short[]> ReadShort(string address, ushort length)
    {
        Result<short[]> r = new Result<short[]>();
        if (GetCommonReadResultBytes(address, length, 3, out byte[] receiveBytes))
        {
            r.isSuccess = true;
            r.result = ByteTransform.TransInt16(receiveBytes, 0, length);
        }
        return r;
    }

    public Result<int> ReadInt(string address)
    {
        Result<int> r = new Result<int>();
        var tempr = ReadInt(address, 1);
        if (tempr != null && tempr.isSuccess)
        {
            r.isSuccess = true;
            r.result = tempr.result[0];
        }
        return r;
    }

    public Result<int[]> ReadInt(string address, ushort length)
    {
        Result<int[]> r = new Result<int[]>();
        if (GetCommonReadResultBytes(address, (ushort)(length * 2),3, out byte[] receiveBytes))
        {
            r.isSuccess = true;
            r.result = ByteTransform.TransInt32(format, receiveBytes, 0, length);
        }
        return r;
    }

    public Result<float> ReadFloat(string address)
    {
        Result<float> r = new Result<float>();
        var tempr = ReadFloat(address, 1);
        if (tempr != null && tempr.isSuccess)
        {
            r.isSuccess = true;
            r.result = tempr.result[0];
        }
        return r;
    }

    public Result<float[]> ReadFloat(string address, ushort length)
    {
        Result<float[]> r = new Result<float[]>();
        if (GetCommonReadResultBytes(address, length,3, out byte[] receiveBytes))
        {
            r.isSuccess = true;
            r.result = ByteTransform.TransSingle(format, receiveBytes, 0, length);
        }
        return r;
    }

    private bool GetCommonReadResultBytes(string address, ushort length,byte function, out byte[] contentBytes)
    {
        contentBytes = new byte[8];
        if (ushort.TryParse(address, out ushort tempaddress))
        {
            tempaddress = isStartWithZero ? (ushort)(tempaddress - 1) : tempaddress;
            byte[] sendBytes = PackCommandToTCP(BuildReadModbusCommand(station, function, tempaddress, length));
            if (GetResultBytes(SendAndReceive(sendBytes), out byte[] resultBytes))
            {
                contentBytes = resultBytes;
                return true;
            }
        }
        return false;
    }

    public bool WriteBool(string address, bool value)
    {
        if (ushort.TryParse(address, out ushort tempaddress))
        {
            tempaddress = isStartWithZero ? (ushort)(tempaddress - 1) : tempaddress;
            byte[] sendBytes = PackCommandToTCP(BuildWriteBoolModbusCommand(station, tempaddress, value));
            if (GetResultBytes(SendAndReceive(sendBytes), out byte[] resultBytes))
            {
                return true;
            }
        }
        return false;
    }

    public bool WriteBool(string address, bool[] value)
    {
        if (ushort.TryParse(address, out ushort tempaddress))
        {
            tempaddress = isStartWithZero ? (ushort)(tempaddress - 1) : tempaddress;
            byte[] sendBytes = PackCommandToTCP(BuildWriteBoolModbusCommand(station, tempaddress, value));
            if (GetResultBytes(SendAndReceive(sendBytes), out byte[] resultBytes))
            {
                return true;
            }
        }
        return false;
    }

    public bool WriteUshort(string address, ushort value)
    {
        if (ushort.TryParse(address, out ushort tempaddress))
        {
            tempaddress = isStartWithZero ? (ushort)(tempaddress - 1) : tempaddress;
            byte[] sendBytes = PackCommandToTCP(BuildWirteOneRegisterModbusCommand(station, tempaddress, value));
            if (GetResultBytes(SendAndReceive(sendBytes), out byte[] resultBytes))
            {
                return true;
            }
        }
        return false;
    }

    public bool WriteShort(string address, short value)
    {
        if (ushort.TryParse(address, out ushort tempaddress))
        {
            tempaddress = isStartWithZero ? (ushort)(tempaddress - 1) : tempaddress;
            byte[] sendBytes = PackCommandToTCP(BuildWirteOneRegisterModbusCommand(station, tempaddress, value));
            if (GetResultBytes(SendAndReceive(sendBytes), out byte[] resultBytes))
            {
                return true;
            }
        }
        return false;
    }

    public bool WriteUshort(string address, ushort[] value)
    {
        return CommonWrite(address, ByteTransform.TransByte(value));
    }

    public bool WriteShort(string address, short[] value)
    {
        return CommonWrite(address, ByteTransform.TransByte(value));
    }

    public bool WriteInt(string address, int value)
    {
        return CommonWrite(address, ByteTransform.TransByte(format, value));
    }

    public bool WriteInt(string address, int[] value)
    {
        return CommonWrite(address, ByteTransform.TransByte(format, value));
    }

    public bool WriteFloat(string address, float value)
    {
        return CommonWrite(address, ByteTransform.TransByte(format, value));
    }

    public bool WriteFloat(string address, float[] value)
    {
        return CommonWrite(address, ByteTransform.TransByte(format, value));
    }

    public bool CommonWrite(string address, byte[] value)
    {
        if (ushort.TryParse(address, out ushort tempaddress))
        {
            tempaddress = isStartWithZero ? (ushort)(tempaddress - 1) : tempaddress;
            byte[] sendBytes = PackCommandToTCP(BuildWriteWordModbusCommand(station, tempaddress, value));
            if (GetResultBytes(SendAndReceive(sendBytes), out byte[] resultBytes))
            {
                return true;
            }
        }
        return false;
    }

    private byte[] SendAndReceive(byte[] send)
    {
        var info = _pool.GetClient();
        try
        {
            var client = info.Client;
            info.Connect();
            NetworkStream ns = client.GetStream();
            ns.Write(send);
            byte[] receive = new byte[2048];
            int length = ns.Read(receive, 0, receive.Length);
            _pool.ReturnClient(info);
            return receive.Take(length).ToArray();
        }
        catch(Exception ex)
        {
            info.Reset();
            _pool.ReturnClient(info);
            throw new Exception($"IP:{info.IP},Port:{info.Port}PLC通讯异常,{ex.Message}", ex);
        }
    }

    private bool GetResultBytes(byte[] receive, out byte[] resultBytes)
    {
        resultBytes = new byte[128];
        if (receive == null || receive.Length < 10) return false;
        resultBytes = receive.Skip(9).ToArray();
        return true;
    }

    private static byte[] BuildReadModbusCommand(byte station, byte function, ushort address, ushort length)
    {
        byte[] array = new byte[6]
        {
                station,
                function,
                BitConverter.GetBytes(address)[1],
                BitConverter.GetBytes(address)[0],
                BitConverter.GetBytes(length)[1],
                BitConverter.GetBytes(length)[0]
        };
        return array;
    }

    /// <summary>
    /// 写单个线圈
    /// </summary>
    /// <param name="station"></param>
    /// <param name="address"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte[] BuildWriteBoolModbusCommand(byte station, ushort address, bool value)
    {
        byte[] array = new byte[6]
        {
                station,
                5,
                BitConverter.GetBytes(address)[1],
                BitConverter.GetBytes(address)[0],
                value ? byte.MaxValue : (byte)0,
                0
        };
        return array;
    }

    /// <summary>
    /// 写多个线圈
    /// </summary>
    /// <param name="station"></param>
    /// <param name="address"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte[] BuildWriteBoolModbusCommand(byte station, ushort address, bool[] value)
    {
        byte[] array1 = ByteTransform.BoolArrayToByte(value);
        byte[] array = new byte[array1.Length + 7];
        array[1] = station;
        array[1] = (byte)15;
        array[2] = BitConverter.GetBytes(address)[1];
        array[3] = BitConverter.GetBytes(address)[0];
        array[4] = (byte)(value.Length / 256);
        array[5] = (byte)(value.Length % 256);
        array[6] = (byte)(array1.Length);
        for (int i = 0; i < array1.Length; i++) array[i + 7] = array1[i];
        return array;
    }

    /// <summary>
    /// 写单个寄存器
    /// </summary>
    /// <param name="station"></param>
    /// <param name="address"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte[] BuildWirteOneRegisterModbusCommand(byte station, ushort address, ushort value)
    {
        byte[] array = new byte[6]
        {
                station,
                6,
                BitConverter.GetBytes(address)[1],
                BitConverter.GetBytes(address)[0],
                BitConverter.GetBytes(value)[1],
                BitConverter.GetBytes(value)[0]
        };
        return array;
    }

    /// <summary>
    /// 写单个寄存器
    /// </summary>
    /// <param name="station"></param>
    /// <param name="address"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte[] BuildWirteOneRegisterModbusCommand(byte station, ushort address, short value)
    {
        byte[] array = new byte[6]
        {
                station,
                6,
                BitConverter.GetBytes(address)[1],
                BitConverter.GetBytes(address)[0],
                BitConverter.GetBytes(value)[1],
                BitConverter.GetBytes(value)[0]
        };
        return array;
    }

    /// <summary>
    /// 写多个寄存器
    /// </summary>
    /// <param name="station"></param>
    /// <param name="address"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte[] BuildWriteWordModbusCommand(byte station, ushort address, byte[] value)
    {
        byte[] array = new byte[value.Length + 7];
        array[1] = station;
        array[1] = (byte)16;
        array[2] = BitConverter.GetBytes(address)[1];
        array[3] = BitConverter.GetBytes(address)[0];
        array[4] = (byte)(value.Length / 2 / 256);
        array[5] = (byte)(value.Length / 2 % 256);
        array[6] = (byte)(value.Length);
        for (int i = 0; i < value.Length; i++) array[i + 7] = value[i];
        return array;
    }

    private static byte[] PackCommandToTCP(byte[] modbusCommand)
    {
        ushort id = 888;
        byte[] array = new byte[modbusCommand.Length + 6];
        array[0] = BitConverter.GetBytes(id)[1];
        array[1] = BitConverter.GetBytes(id)[0];
        array[4] = BitConverter.GetBytes(modbusCommand.Length)[1];
        array[5] = BitConverter.GetBytes(modbusCommand.Length)[0];
        for (int i = 0; i < modbusCommand.Length; i++)
        {
            array[6 + i] = modbusCommand[i];
        }
        return array;
    }

    public void Dispose()
    {
        
    }
}

public class Result<T>
{
    public bool isSuccess = false;
    public T result;
}

public class ByteTransform
{
    public static byte[] TransByte(bool value)
    {
        return TransByte(new bool[1] { value });
    }

    public static byte[] TransByte(bool[] values)
    {
        return (values == null) ? null : BoolArrayToByte(values);
    }

    public static byte[] TransByte(short value)
    {
        return TransByte(new short[1] { value });
    }

    public static byte[] TransByte(short[] values)
    {
        if (values == null)
        {
            return null;
        }
        byte[] array = new byte[values.Length * 2];
        for (int i = 0; i < values.Length; i++)
        {
            BitConverter.GetBytes(values[i]).CopyTo(array, 2 * i);
        }
        return array;
    }

    public static byte[] TransByte(ushort value)
    {
        return TransByte(new ushort[1] { value });
    }

    public static byte[] TransByte(ushort[] values)
    {
        if (values == null)
        {
            return null;
        }
        byte[] array = new byte[values.Length * 2];
        for (int i = 0; i < values.Length; i++)
        {
            BitConverter.GetBytes(values[i]).CopyTo(array, 2 * i);
        }
        return array;
    }

    public static byte[] TransByte(DataFormat format, int value)
    {
        return TransByte(format, new int[1] { value });
    }

    public static byte[] TransByte(DataFormat format, int[] values)
    {
        if (values == null)
        {
            return null;
        }
        byte[] array = new byte[values.Length * 4];
        for (int i = 0; i < values.Length; i++)
        {
            ByteTransDataFormat4(format, BitConverter.GetBytes(values[i])).CopyTo(array, 4 * i);
        }
        return array;
    }

    public static byte[] TransByte(DataFormat format, uint value)
    {
        return TransByte(format, new uint[1] { value });
    }

    public static byte[] TransByte(DataFormat format, uint[] values)
    {
        if (values == null)
        {
            return null;
        }
        byte[] array = new byte[values.Length * 4];
        for (int i = 0; i < values.Length; i++)
        {
            ByteTransDataFormat4(format, BitConverter.GetBytes(values[i])).CopyTo(array, 4 * i);
        }
        return array;
    }

    public static byte[] TransByte(DataFormat format, float value)
    {
        return TransByte(format, new float[1] { value });
    }

    public static byte[] TransByte(DataFormat format, float[] values)
    {
        if (values == null)
        {
            return null;
        }
        byte[] array = new byte[values.Length * 4];
        for (int i = 0; i < values.Length; i++)
        {
            ByteTransDataFormat4(format, BitConverter.GetBytes(values[i])).CopyTo(array, 4 * i);
        }
        return array;
    }

    public static short TransInt16(byte[] buffer, int index)
    {
        return BitConverter.ToInt16(buffer, index);
    }

    public static ushort TransUInt16(byte[] buffer, int index)
    {
        return BitConverter.ToUInt16(buffer, index);
    }

    public static short[] TransInt16(byte[] buffer, int index, int length)
    {
        short[] array = new short[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = TransInt16(buffer, index + 2 * i);
        }
        return array;
    }

    public static ushort[] TransUInt16(byte[] buffer, int index, int length)
    {
        ushort[] array = new ushort[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = TransUInt16(buffer, index + 2 * i);
        }
        return array;
    }

    public static int TransInt32(DataFormat format, byte[] buffer, int index)
    {
        return BitConverter.ToInt32(ByteTransDataFormat4(format, buffer, index), 0);
    }

    public static uint TransUInt32(DataFormat format, byte[] buffer, int index)
    {
        return BitConverter.ToUInt32(ByteTransDataFormat4(format, buffer, index), 0);
    }

    public static int[] TransInt32(DataFormat format, byte[] buffer, int index, int length)
    {
        int[] array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = TransInt32(format, buffer, index + 4 * i);
        }
        return array;
    }

    public static uint[] TransUInt32(DataFormat format, byte[] buffer, int index, int length)
    {
        uint[] array = new uint[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = TransUInt32(format, buffer, index + 4 * i);
        }
        return array;
    }

    public static float TransSingle(DataFormat format, byte[] buffer, int index)
    {
        return BitConverter.ToSingle(ByteTransDataFormat4(format, buffer, index), 0);
    }

    public static float[] TransSingle(DataFormat format, byte[] buffer, int index, int length)
    {
        float[] array = new float[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = TransSingle(format, buffer, index + 4 * i);
        }
        return array;
    }

    protected static byte[] ByteTransDataFormat4(DataFormat df, byte[] value, int index = 0)
    {
        byte[] array = new byte[4];
        switch (df)
        {
            case DataFormat.ABCD:
                array[0] = value[index + 3];
                array[1] = value[index + 2];
                array[2] = value[index + 1];
                array[3] = value[index];
                break;
            case DataFormat.BADC:
                array[0] = value[index + 2];
                array[1] = value[index + 3];
                array[2] = value[index];
                array[3] = value[index + 1];
                break;
            case DataFormat.CDAB:
                array[0] = value[index + 1];
                array[1] = value[index];
                array[2] = value[index + 3];
                array[3] = value[index + 2];
                break;
            case DataFormat.DCBA:
                array[0] = value[index];
                array[1] = value[index + 1];
                array[2] = value[index + 2];
                array[3] = value[index + 3];
                break;
        }
        return array;
    }

    public static byte[] BoolArrayToByte(bool[] array)
    {
        if (array == null)
        {
            return null;
        }
        int num = ((array.Length % 8 == 0) ? (array.Length / 8) : (array.Length / 8 + 1));
        byte[] array2 = new byte[num];
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i])
            {
                array2[i / 8] += GetDataByBitIndex(i % 8);
            }
        }
        return array2;
    }

    public static bool BoolOnByteIndex(byte value, int offset)
    {
        byte dataByBitIndex = GetDataByBitIndex(offset);
        return (value & dataByBitIndex) == dataByBitIndex;
    }

    public static byte GetDataByBitIndex(int offset)
    {
        return offset switch
        {
            0 => 1,
            1 => 2,
            2 => 4,
            3 => 8,
            4 => 16,
            5 => 32,
            6 => 64,
            7 => 128,
            _ => 0,
        };
    }

    public static bool[] ByteToBoolArray(byte[] inBytes, int length)
    {
        if (inBytes == null)
        {
            return null;
        }
        if (length > inBytes.Length * 8)
        {
            length = inBytes.Length * 8;
        }
        bool[] array = new bool[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = BoolOnByteIndex(inBytes[i / 8], i % 8);
        }
        return array;
    }
}

public enum DataFormat
{
    ABCD,
    BADC,
    CDAB,
    DCBA
}
