using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Simpls
{

	public class SMCollection :IDictionary<string, SMItem>
	{
		private  Dictionary<string, SMItem> _dic = new Dictionary<string, SMItem>();
		private ICollection<KeyValuePair<string, SMItem>> _collection=> _dic;

		public Action? OnChanging { get; set; }

		public SMItem this[string key]
		{
			get => _dic[key];
			set
			{
				_dic[key] = value;
				this.OnChanging?.Invoke();
			}
		}

		public ICollection<string> Keys => _dic.Keys;

		public ICollection<SMItem> Values => _dic.Values;

		public int Count => _dic.Count;

		public bool IsReadOnly => false;

		public void Add(string key, SMItem value)
		{
			this.OnChanging?.Invoke();
			_dic.Add(key, value);
		}

		public void Add(KeyValuePair<string, SMItem> item)
		{
			this.OnChanging?.Invoke();
			_collection.Add(item);
		}

		public void Clear()
		{
			this.OnChanging?.Invoke();
			_dic.Clear();
		}

		public bool Contains(KeyValuePair<string, SMItem> item)
		{
			return _collection.Contains(item);
		}

		public bool ContainsKey(string key)
		{
			return _dic.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<string, SMItem>[] array, int arrayIndex)
		{
			 _collection.CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<string, SMItem>> GetEnumerator()
		{
			return _dic.GetEnumerator();
		}

		public bool Remove(string key)
		{
			this.OnChanging?.Invoke();
			return _dic.Remove(key);
		}

		public bool Remove(KeyValuePair<string, SMItem> item)
		{
			this.OnChanging?.Invoke();
			return _collection.Remove(item);
		}

		public bool TryGetValue(string key, [MaybeNullWhen(false)] out SMItem value)
		{
			return _dic.TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dic.GetEnumerator();
		}
	}

	public class SMItem
	{
		public string Name { get;  set; }
		public PositionState State { get; set; }
		public string Shelf { get; set; } = string.Empty;
	}

		public enum PositionState
		{
			Null,
			Empty,
			Full
		}
	}

