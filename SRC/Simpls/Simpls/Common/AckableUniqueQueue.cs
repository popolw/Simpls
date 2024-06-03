using System.Collections.Concurrent;
using System.Drawing;

namespace Simpls
{
    public interface IQueueAck
    {
        void Ack();
    }

    public class AckableUniqueQueue<T>
    {
        private readonly ConcurrentDictionary<T, bool> _set = new ConcurrentDictionary<T, bool>();
        private readonly ConcurrentQueue<AckableItem<T>> _queue = new ConcurrentQueue<AckableItem<T>>();
        private Semaphore _locker = new Semaphore(1, 1);

        public void Enqueue(T value)
        {
            _set.GetOrAdd(value, (key) =>
            {
                _queue.Enqueue(new AckableItem<T>(value, (item) =>
                {
                    _set.TryRemove(item.Value, out _);
                    _queue.TryDequeue(out _);
                    _locker.Release();
                }));
                return true;
            });
        }

        public bool Dequeue(out AckableItem<T> value)
        {
            _locker.WaitOne();
            return _queue.TryPeek(out value);

        }

        public class AckableItem<TItem> : IQueueAck
        {
            private readonly Action<AckableItem<TItem>> _onAck;

            public AckableItem(TItem value, Action<AckableItem<TItem>> onAck)
            {
                this.Value = value;
                this._onAck = onAck;
            }

            public TItem Value { get; }
            public bool IsAcked { get; private set; }

            public void Ack()
            {
                if (!this.IsAcked)
                {
                    IsAcked = true;
                    this._onAck(this);
                }
            }
        }
    }



    public struct NotifyMessage<TBody> 
    {
        public NotifyMessage(string channel, TBody body)
        {
            this.Channel = channel;
            this.Body = body;
        }

        public string Channel { get; private set; }

        public TBody Body { get; private set; }


        public override string ToString()
        {
            return $"Channel={this.Channel},Body={this.Body}";
        }
    }

    public class MessageChannelNotify
    {
        private readonly AckableUniqueQueue<KeyValuePair<string, string>> _queue = new AckableUniqueQueue<KeyValuePair<string, string>>();

        public MessageChannelNotify()
        {
            Task.Factory.StartNew(Revice, TaskCreationOptions.LongRunning);
        }

        public Func<string, string, IQueueAck, Task> OnRevice { get; set; }

        private void Revice()
        {
            while (true)
            {
                if (_queue.Dequeue(out var item))
                {
                    this.OnRevice?.Invoke(item.Value.Key, item.Value.Value, item);
                }
                Task.Delay(TimeSpan.FromSeconds(1));
            }
        }


        /// <summary>
        /// 发送一条消息弹窗消息
        /// </summary>
        /// <param name="channel">Channel</param>
        /// <param name="message">
        /// 需要弹窗显示的消息
        /// 如需关闭可以发送CLOSE字符串
        /// </param>
        /// <returns></returns>
        public Task SendMessageAsync(string channel, string message)
        {
            _queue.Enqueue(new KeyValuePair<string, string>(channel, message));
            return Task.CompletedTask;
        }
    }



}


