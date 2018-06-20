using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IconMaker
{
    public class IntrusiveDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<TValue>
    {
        private readonly Func<TValue, TKey> _keyAccessor;
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public IntrusiveDictionary(Func<TValue, TKey> keyAccessor) => _keyAccessor = keyAccessor;

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => _dictionary.GetEnumerator();

        public IEnumerator<TValue> GetEnumerator() => _dictionary.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);

        public void Clear() => _dictionary.Clear();

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => _dictionary.Add(key, value);

        public void Add(TValue value) => _dictionary.Add(_keyAccessor(value), value);

        public bool Remove(TKey key) => _dictionary.Remove(key);

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
    }
}
