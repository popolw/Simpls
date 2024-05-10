using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Simpls
{
    public class Log4NetTest
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        [SetUp]
        public void Setup()
        {
            _services.AddLogging(builder =>
            {
                builder.AddLog4Net();
            });
        }

        [Test]
        public void Run()
        {
            var provider = _services.BuildServiceProvider();
            var factory = provider.GetRequiredService<ILoggerFactory>();
            var logger = factory.CreateLogger("System.Net.Http.HttpClient.MIR.ClientHandler");
            logger.LogInformation("System.Net.Http.HttpClient.MIR.ClientHandler");
        }

        [Test]
        public void Count()
        {
             var x = 0;
            var counter = new Counter(ref x);
            counter.Add();
        }
    }

    public class Counter
    {
        private  int _count = 0;
        public Counter(ref int count)
        {
            this._count =  count;
        }

        public  int Count =>  _count;

        public void Add()
        {
            for (int i = 0; i < 10; i++)
            {
                this._count++;
            }
        }
    }
}
