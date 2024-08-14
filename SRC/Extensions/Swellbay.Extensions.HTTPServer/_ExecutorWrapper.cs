using System.Net;

namespace Swellbay.Extensions.HTTPServer
{
    internal class _ExecutorWrapper
    {
        private int _index = 0;
        private readonly RequestDelegate _next;
        private readonly HttpListenerContext _context;
        private readonly IList<Action<HttpListenerContext, RequestDelegate>> _middlewares;

        public _ExecutorWrapper(HttpListenerContext context, IList<Action<HttpListenerContext, RequestDelegate>> middlewares)
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
}
