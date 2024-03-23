using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Simpls
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddStackExchangeRedisCache(options => {
				options.Configuration = "localhost";
				options.InstanceName = "TI_PROBE_F5";
			});

			services.AddSingleton(provider =>
			{
				var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>();
				var connection = ConnectionMultiplexer.Connect(options.Value.Configuration);
				return connection.GetDatabase();
			});

			return services;
		}
	}
}
