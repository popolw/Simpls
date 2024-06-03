using NUnit.Framework;
using System.Collections.Concurrent;

namespace Simpls
{
    [TestFixture]
    public class QueueTest
    {
        [Test]
        public async Task Queue()
        {
            //MessageChannelNotify notify = new MessageChannelNotify();
            //await notify.SendMessageAsync("ABC", "HELLO");
            //await notify.SendMessageAsync("ABCD", "HELLO");
            //notify.OnRevice = (channe, messgge,ack) => {
            //   return Task.Run(() => {
            //       //ack.Ack();
            //   });
            //};

            var x = new NotifyMessage<string>("AA", "BB");
            var y = new NotifyMessage< string>("AA", "ZZ");
            var z = new NotifyMessage<string>("AA", "BB");

            ConcurrentDictionary<NotifyMessage<string>, bool> set = new ConcurrentDictionary<NotifyMessage<string>, bool>();
            set.GetOrAdd(x, (key) => true);
            set.GetOrAdd(y, (key) => true);
            set.GetOrAdd(z, (key) => true);
            var b = false;
            IEquatable<bool> eq = b;
            eq.Equals(true);
            // item.Ack();
        }
    }
}
