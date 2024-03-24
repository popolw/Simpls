using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace Simpls
{
    public class APIResult
    {
        /// <summary>
        /// Success，Error
        /// </summary>
        public string Statue = "";
        /// <summary>
        /// 
        /// </summary>
        public string Msg = "";
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddServices();
            var provider = services.BuildServiceProvider();

            //HttpListenerServer server = new HttpListenerServer(54320);
            //server.AddPrefixes("/", (method, request, response) => {
            //	var positionName = request.QueryString["PositionName"];
            //	var shelfID = request.QueryString["ShelfID"];
            //	APIResult result = new APIResult();
            //	result.Statue = "Success";
            //	result.Msg = $"PositionName={positionName},ShelfID={shelfID}";
            //});


            //server.Use((context, next) =>
            //{
            //	if("/PositionCheckOut".Equals(context.Request?.Url?.AbsolutePath, StringComparison.CurrentCultureIgnoreCase))
            //	{
            //		var positionName = context.Request.QueryString["PositionName"];
            //		var shelfID = context.Request.QueryString["ShelfID"];
            //		var result = new APIResult { Statue = "Success", Msg = $"PositionName={positionName},ShelfID={shelfID}" };
            //		context.Response.WriteJson(result, Encoding.UTF8, new JsonSerializerOptions { IncludeFields = true });
            //	}
            //	else
            //	{
            //		next(context);
            //	}
            //});

            //server.Use((context, next) =>
            //{
            //	context.Response.Write("abcd", Encoding.UTF8);
            //	next(context);
            //});

            //server.Start();
            //services.AddServices();

            //var redis = provider.GetRequiredService<IDatabase>();
            //var manager = new MachineAvailableCheckerManager(redis,(machine)=>machine.StartsWith("TE") ,machine => true, (unabailable) =>
            //{
            //	Console.WriteLine($"{DateTime.Now}机台:{unabailable.Machine}机台不可用");
            //	return true;
            //});
            //manager.Set("TE1001");
            //manager.Clear();

            EchoClient client = new EchoClient();
            client.Start();
            

            Console.Read();
        }
    }
}
