using System.Net;
using System.Text;
using System.Text.Json;

namespace Simpls
{

	public delegate void RequestDelegate(HttpListenerContext context);


	public static class HttpListenerResponseExtensions
	{
		public static void Write(this HttpListenerResponse response, string text, Encoding encoding)
		{
			var buffer = encoding.GetBytes(text);
			response.ContentEncoding = encoding;
			response.OutputStream.Write(buffer, 0, buffer.Length);
		}

		public static void WriteJson<T>(this HttpListenerResponse response, T value)
		{
			response.WriteJson(value, Encoding.UTF8, null);
		}

		public static void WriteJson<T>(this HttpListenerResponse response, T value,Encoding encoding,JsonSerializerOptions? options=null)
		{
			response.ContentType = "application/json";
			var json = JsonSerializer.Serialize(value,options);
			response.Write(json, encoding);
		}
	}

	internal class _ExecutorWrapper
	{
		private int _index = 0;
		private readonly RequestDelegate _next;
		private readonly HttpListenerContext _context;
		private readonly IList<Action<HttpListenerContext, RequestDelegate>> _middlewares;

		public _ExecutorWrapper(HttpListenerContext context,IList<Action<HttpListenerContext, RequestDelegate>> middlewares)
		{
			this._next = Next;
			this._context = context;
			this._middlewares = middlewares;
		}

		public void Excute()
		{
			this.Next(_context);
		}

		private void Next(HttpListenerContext context)
		{
			if (_index < _middlewares.Count && _middlewares.Count > 0)
			{
				var current = _middlewares[_index];
				_index++;
				current.Invoke(context, _next);
			}
		}
	}

	/// <summary>
	/// Http服务类，提供Get/Post请求处理逻辑
	/// </summary>
	public class HttpListenerServer:IDisposable
	{
		Action<string> _errorActionHandle = null;
		private readonly  HttpListener _httpListener;
		private readonly IList<Action<HttpListenerContext, RequestDelegate>> _middlewares = new List<Action<HttpListenerContext, RequestDelegate>>();
		public HttpListenerServer(int port)
		{
			_httpListener = new HttpListener();
			this._httpListener.Prefixes.Add($"http://+:{port}/");
		}


		/// <summary>
		/// 添加出错处理
		/// </summary>
		/// <param name="errorAction"></param>
		public void AddErrorListener(Action<string> errorAction)
		{
			_errorActionHandle = errorAction;
		}


		public bool Start()
		{
			bool isStart = false;
			try
			{
				_httpListener.Start();
				_httpListener.BeginGetContext(Revieve, _httpListener);
				isStart = true;
			}
			catch (Exception ex)
			{
				_errorActionHandle?.Invoke($"StartListen Error!Msg:{ex.Message}");
			}
			return isStart;
		}



		public void Use(Action<HttpListenerContext, RequestDelegate> middleware)
		{
			_middlewares.Add(middleware);
		}


		/// <summary>
		/// 处理请求
		/// </summary>
		/// <param name="ar"></param>
		private void Revieve(IAsyncResult ar)
		{
			try
			{
				if (!_httpListener.IsListening) { return; }
				var context = _httpListener.EndGetContext(ar);//获得Context对象
				var request = context.Request;
				var response = context.Response;

				using (response)
				{
					using (response.OutputStream)
					{
						try
						{
							var executor = new _ExecutorWrapper(context, _middlewares);
							executor.Excute();
						}
						catch (Exception ex)
						{
							response.StatusCode = 500;
							response.ContentType = "text/plain";
							response.Write(ex.Message, Encoding.UTF8);
						}

					}
				}

			}
			catch (Exception ex)
			{
				_errorActionHandle?.Invoke($"Revieve Error!Msg:{ex.Message}");
			}
			finally
			{
				_httpListener.BeginGetContext(Revieve, _httpListener);
			}
		}


		public void Dispose()
		{
			_httpListener.Stop();
			_httpListener.Close();
		}
	}
}
