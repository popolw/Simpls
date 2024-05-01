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
    }
}
