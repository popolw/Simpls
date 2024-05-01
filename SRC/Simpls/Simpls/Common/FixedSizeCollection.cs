using System.Collections;
using System.Collections.Specialized;

namespace Simpls
{

    public delegate void NotifyCollectionChangedEventHandler<T>(IEnumerable<T> sender, NotifyCollectionChangedEventArgs<T> e);

    public class NotifyCollectionChangedEventArgs<T> : EventArgs
    {
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            this.Action = action;
        }

        public NotifyCollectionChangedAction Action { get; }
    }

    public interface INotifyCollectionChanged<T>
    {
       public event NotifyCollectionChangedEventHandler<T>? CollectionChanged;
    }

    public class FixedSizeCollection<T>:IEnumerable<T>, INotifyCollectionChanged<T>
    {
        private readonly T[] _source;

        public event NotifyCollectionChangedEventHandler<T>? CollectionChanged;

        public FixedSizeCollection(int length)
        {
            _source=new T[length];
        }

        public void Push(T value)
        {
            for (int i = _source.Length-1; i >= 0; i--)
            {
                var next = i - 1;
                if(next>=0) _source[i] = _source[next];
            }
            _source[0] = value;
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add));
        }


        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in this._source)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this._source)
            {
                yield return item;
            }
        }
    }
}
