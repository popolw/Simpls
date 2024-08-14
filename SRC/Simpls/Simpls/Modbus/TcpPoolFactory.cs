using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

public class ConnectionInfo:IDisposable
{
    private IPAddress _ip;
    private int _port;
    private bool _connected = false;
    public TcpClient Client { get; private set; }
    public string ConnectionId { get; private set; }

    public ConnectionInfo(string ip, int port)
    {
        this.ConnectionId = Guid.NewGuid().ToString();
        this._ip = IPAddress.Parse(ip);
        this._port = port;
        try
        {
			this.CreateClient();
		}
        catch
        {
            this.CreateClient(false);
        }
     
    }

    public IPAddress IP => this._ip;
    public int Port => this._port;  

    private void CreateClient(bool connect=true)
    {
        this.Client = new TcpClient();
        if (connect)
        {
            this.Connect();
        }
    }

    /// <summary>
    /// 重置链接
    /// </summary>
    public void Reset()
    {
        try
        {
            this.Client.Dispose();
        }
        catch
        {

        }
        this._connected = false;
        this.CreateClient(false);
    }

    public void Connect()
    {
        if (!this._connected && !this.Client.Connected)
        {
            this.Client.Connect(this._ip, this._port);
            this._connected = true;
        }
    }

    public void Dispose()
    {
        this.Client.Dispose();
    }
}

/// <summary>
/// TCP客户端连接池
/// </summary>
public class TcpClientPool : IDisposable
{
    private readonly string _serverIp;
    private readonly int _serverPort;
    private readonly int _maxPoolSize;
    private readonly ConcurrentBag<ConnectionInfo> _pool;
    private readonly Semaphore _semaphore;


    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serverIp">链接ip</param>
    /// <param name="serverPort">链接端口</param>
    /// <param name="maxPoolSize">池大小</param>
    public TcpClientPool(string serverIp, int serverPort, int maxPoolSize)
    {
        _serverIp = serverIp;
        _serverPort = serverPort;
        _maxPoolSize = maxPoolSize;
        _pool = new ConcurrentBag<ConnectionInfo>();
        _semaphore = new Semaphore(initialCount: 0, maximumCount: maxPoolSize);

        // Pre-fill the pool  
        for (int i = 0; i < _maxPoolSize; i++)
        {
            var clientinfo = CreateConnectedClient(_serverIp, _serverPort);
            _pool.Add(clientinfo);
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 创建一个TcpClient链接并连接到服务器
    /// </summary>
    /// <param name="ip">要链接到的ip</param>
    /// <param name="port">链接端口</param>
    /// <returns><see cref="{TcpClient}"/>TcpClient</returns>
    private ConnectionInfo CreateConnectedClient(string ip, int port)
    {
        ConnectionInfo info = new ConnectionInfo(ip,port);
        return info;
    }

    /// <summary>
    /// 从连接池获取一个<see cref="ConnectionInfo">TcpClient链接</see>
    /// </summary>
    /// <returns><see cref="{ConnectionInfo}"/>TcpClient</returns>
    public ConnectionInfo GetClient()
    {
        _semaphore.WaitOne();
        return _pool.TryTake(out ConnectionInfo client) ? client : CreateConnectedClient(_serverIp, _serverPort);
    }

    /// <summary>
    /// 归还链接致连接池
    /// </summary>
    /// <param name="client"><see cref="{ConnectionInfo}"/>ConnectionInfo</param>
    public void ReturnClient(ConnectionInfo client)
    {
        _pool.Add(client);
        _semaphore.Release();
    }


    /// <summary>
    /// 释放连接池中的链接
    /// </summary>
    public void Dispose()
    {
        foreach (var client in _pool)
        {
            client.Dispose();
        }
        _semaphore.Dispose();
    }
}

/// <summary>
/// TCP连接池工厂
/// </summary>
public class TcpPoolFactory
{
    private static ConcurrentDictionary<string, TcpClientPool> _dic = new ConcurrentDictionary<string, TcpClientPool>();

    public static event EventHandler<EventArgs> OnDisponsed;

    /// <summary>
    /// 根据IP和端口创建一个指定连接数量的连接池   
    /// </summary>
    /// <param name="ip">ip</param>
    /// <param name="port">端口</param>
    /// <param name="connecctonCount">链接池生成链接的数量(plc模块一般同时支持8个链接)</param>
    /// <returns>一个连接池<see cref="{TcpClientPool}"/></returns>
    public static TcpClientPool Create(string ip, int port, int connecctonCount)
    {
        var key = $"{ip}:{port}";
        return _dic.GetOrAdd(key, k => new TcpClientPool(ip, port, connecctonCount));
    }

    /// <summary>
    /// 释放由TcpPoolFactory创建的连接池
    /// </summary>
    public static void Disponse()
    {
        foreach (var kv in _dic)
        {
            kv.Value.Dispose();
        }
        _dic.Clear();
        OnDisponsed?.Invoke(default, EventArgs.Empty);
    }
}
