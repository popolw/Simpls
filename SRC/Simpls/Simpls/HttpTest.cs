using NUnit.Framework;
using System.Text.Json;
using System.Text;

namespace Simpls
{
    [TestFixture]
    public class HttpTest
    {
        public HttpTest()
        {

        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Run()
        {
            HttpListenerServer server = new HttpListenerServer(54320);

            server.Use((context, next) =>
            {
                if ("/PositionCheckOut".Equals(context.Request?.Url?.AbsolutePath, StringComparison.CurrentCultureIgnoreCase))
                {
                    var positionName = context.Request.QueryString["PositionName"];
                    var shelfID = context.Request.QueryString["ShelfID"];
                    var result = new APIResult { Statue = "Success", Msg = $"PositionName={positionName},ShelfID={shelfID}" };
                    context.Response.WriteJson(result, Encoding.UTF8, new JsonSerializerOptions { IncludeFields = true });
                }
                else
                {
                    next(context);
                }
            });

            server.Use((context, next) =>
            {
                context.Response.Write("abcd", Encoding.UTF8);
                next(context);
            });

            server.Start();
        }
    }
}
