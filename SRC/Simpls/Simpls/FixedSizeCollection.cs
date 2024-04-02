using System.Collections;
using System.Collections.Specialized;

namespace Simpls
{
    public class FixedSizeCollection<T>:IEnumerable<T>, INotifyCollectionChanged
    {
        private readonly Queue<T> _queue;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public FixedSizeCollection(int length)
        {
            _queue = new Queue<T>(new T[length]);
        }

        public void Push(T value)
        {
            _queue.TryDequeue(out _);
            _queue.Enqueue(value);
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        }


        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in this._queue)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this._queue)
            {
                yield return item;
            }
        }
    }
}
