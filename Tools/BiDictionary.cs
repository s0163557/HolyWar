using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Tools
{
    /// <summary>
    /// Создан для хранени уникальных пар и быстрого поиска в обе стороны, принуждает все хранимые значения к уникальности.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class BiDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> forward = new();
        private readonly Dictionary<TValue, TKey> reverse = new();

        public IEnumerable<TKey> GetKeys()
        {
            return forward.Keys;
        }
        public IEnumerable<TValue> GetValues()
        {
            return reverse.Keys;
        }

        public bool TryGetValueByKey(TKey key, out TValue value)
        {
            if (key == null)
            {
                value = default;
                return false;
            }

            return forward.TryGetValue(key, out value);
        }

        public bool TryGetKeyByValue(TValue value, out TKey key)
        {
            if (value == null)
            {
                key = default;
                return false;
            }

            return reverse.TryGetValue(value, out key);
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                return;

            if (forward.ContainsKey(key) || reverse.ContainsKey(value))
                throw new ArgumentException("BiDictionary can't contain a duplicate key or value");

            forward[key] = value;
            reverse[value] = key;
        }

        public void ChangeKey(TKey oldKey, TKey newKey)
        {
            if (forward.ContainsKey(oldKey) && !forward.ContainsKey(newKey))
            {
                TValue value = forward[oldKey];
                forward.Remove(oldKey);
                forward[newKey] = value;
                reverse[value] = newKey;
            }
            else
            {
                throw new ArgumentException("BiDictionary must contain the old key to change and can't contain the new key to add it");
            }
        }

        public void ChangeValue(TValue oldValue, TValue newValue)
        {
            if (reverse.ContainsKey(oldValue) && !reverse.ContainsKey(newValue))
            {
                TKey key = reverse[oldValue];
                reverse.Remove(oldValue);
                reverse[newValue] = key;
                forward[key] = newValue;
            }
            else
            {
                throw new ArgumentException("BiDictionary must contain the old value to change and can't contain the new value to add it");
            }
        }

        public void RemoveByKey(TKey key)
        {
            if (forward.ContainsKey(key))
            {
                TValue value = forward[key];
                forward.Remove(key);
                reverse.Remove(value);
            }
        }

        public void RemoveByValue(TValue value)
        {
            if (reverse.ContainsKey(value))
            {
                TKey key = reverse[value];
                forward.Remove(key);
                reverse.Remove(value);
            }
        }
    }
}
