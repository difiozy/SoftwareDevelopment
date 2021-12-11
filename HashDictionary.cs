using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SoftwareDevelopment
{
    class HashDictionary<K, V> : IDictionary<K, V>
    {
        private class Node<K, V>
        {
            public readonly int hash;
            public readonly K key;
            public V value;
            public Node<K, V> next = null;

            public void Add(Node<K,V> node)
            {                
                Node<K, V> cur = this;
                while(true)
                {
                    if (cur.next == null)
                    {
                        cur.next = node;
                        break;
                    }

                    cur = cur.next;
                }
            }

            public Node(int hash, K key, V value)
            {
                this.hash = hash;
                this.key = key;
                this.value = value;
            }

            public Node(KeyValuePair<K,V> kv)
            {
                hash = kv.Key.GetHashCode();
                key = kv.Key;
                value = kv.Value;
            }

            public override String ToString()
            {
                return $"hash={hash}  key={key}  value={value}";
            }
        }

        private Node<K, V>[] hashTable;
        private int size;//count Node<k,V>

        readonly float loadFactor;

        readonly int MAX_CAPACITY = 1 << 30;
        readonly int DEFAULT_CAPACITY = 1 << 4;
        readonly float DEFAULT_LOADfACTOR = 0.75f;

        public HashDictionary()
        {
            size = 0;
            loadFactor = DEFAULT_LOADfACTOR;
            hashTable = new Node<K, V>[DEFAULT_CAPACITY];
        }

        public HashDictionary(HashDictionary<K,V> d)
        {
            this.hashTable = d.hashTable;
            size = d.size;
            loadFactor = d.loadFactor;
        }
        public V this[K key]
        { 
            get
            {
                int hc = key.GetHashCode();
                int index = (hashTable.Length - 1) & hc;
                Node<K, V> node = hashTable[index];
                while(true)
                {
                    if (node == null)
                        break;
                    if (node.hash == hc)
                    {
                        if (node.key.Equals(key))
                        {
                            break;
                        }
                    }
                    node = node.next;
                }
                if (node!=null)
                {
                    return node.value;
                }                
                else
                {
                    throw new Exception($"There's no element with such key: {key}");//TODO: Стоит ли возвращать defaul(V)?
                }
                
            }
            set
            {
                int hc = key.GetHashCode();
                int index = (hashTable.Length - 1) & hc;
                Node<K, V> node = hashTable[index];
                
                if (node==null)
                {
                    hashTable[index] = new Node<K, V>(hc, key, value);
                    size++;
                }
                else
                {
                    while(true)
                    {
                        if (node.hash == hc)
                        {
                            if (node.key.Equals(key))
                            {
                                node.value = value;
                                return;
                            }
                        }

                        if (node.next == null)
                            break;

                        node = node.next;
                    }
                    node.next = new Node<K, V>(hc, key, value);
                    size++;
                    RebuildTable();
                    //TODO: Check resize table
                }
            }
        }

        public ICollection<K> Keys
        {
            get
            {
                ICollection<K> list = new LinkedList<K>();
                foreach(var element in hashTable)
                {
                    var temp = element;
                    while(temp != null)
                    {
                        list.Add(temp.key);
                        temp = temp.next;
                    }
                }
                return list;
            }
        }
        public ICollection<V> Values
        {
            get
            {
                ICollection<V> list = new LinkedList<V>();
                foreach (var element in hashTable)
                {
                    var temp = element;
                    while (temp != null)
                    {
                        list.Add(temp.value);
                        temp = temp.next;
                    }
                }
                return list;
            }
        }

        public int Count
        {
            get => size;
        }

        public bool IsReadOnly
        {
            get => false;
        }

        private void RebuildTable()
        {
            if (size >= hashTable.Length * loadFactor && hashTable.Length * 2 <= MAX_CAPACITY)
            {
                int newLen = hashTable.Length * 2;
                Node<K, V>[] hasT = new Node<K, V>[newLen];

                for (int i = 0; i < hashTable.Length; i++)
                {
                    Node<K, V> node = hashTable[i];
                    while (node != null)
                    {
                        int newIndex = (newLen - 1) & node.hash;
                        if (hasT[newIndex] == null)
                        {
                            hasT[newIndex] = node;
                        }
                        else
                        {
                            hasT[newIndex].Add(node);
                        }
                        node = node.next;
                    }
                }
                this.hashTable = hasT;
            }
        }
        public void Add(K key, V value)
        {
            size++;
            this[key] = value;
            RebuildTable();
        }

        public void Add(KeyValuePair<K, V> item)
        {
            this[item.Key] = item.Value;
            size++;
            RebuildTable();
            //TODO: Check resize table
        }

        public void Clear()
        {
            size = 0;
            hashTable = new Node<K, V>[DEFAULT_CAPACITY];
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            K key = item.Key;
            int hc = key.GetHashCode();
            int index = (hashTable.Length - 1) & hc;
            Node<K, V> node = hashTable[index];

            while (node != null)
            {
                if (node.hash == hc)
                {
                    if (node.key.Equals(key))
                    {
                        if (node.value.Equals(item.Value))
                            return true;
                        else
                            return false;
                    }
                }
            }
            return false;



        }

        public bool ContainsKey(K key)
        {
            int hc = key.GetHashCode();
            int index = (hashTable.Length - 1) & hc;
            Node<K, V> node = hashTable[index];

            while (node != null)
            {
                if (node.hash == hc)
                {
                    if (node.key.Equals(key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentException("ArrayIndex is not valid");
            }
            if (arrayIndex + size - 1 > array.Length)
            {
                throw new Exception("Array out of range");
            }
            if (array == null)
            {
                throw new NullReferenceException("Array is null");
            }

            for (int i = 0; i < hashTable.Length; i++)
            {
                Node<K, V> node = hashTable[i];
                while(node!=null)
                {
                    array[arrayIndex] = new KeyValuePair<K, V>(node.key, node.value);
                    arrayIndex++;
                }
            }

        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            for (int i = 0; i < hashTable.Length; i++)
            {
                Node<K, V> node = hashTable[i];
                while (node != null)
                    yield return new KeyValuePair<K,V> (node.key, node.value);
            }
        }

        public bool Remove(K key)
        {
            int hc = key.GetHashCode();
            int index = (hashTable.Length - 1) & hc;
            Node<K, V> node = hashTable[index];
            
            if (node.key.Equals(key))
            {
                hashTable[index] = null;
                return true;
            }

            while(node.next != null)
            {
                if (node.next.key.Equals(key))
                {
                    node.next = node.next.next;
                    return true;
                }
            }
            return false;
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            K key = item.Key;
            V value = item.Value;
            int hc = key.GetHashCode();
            int index = (hashTable.Length - 1) & hc;
            Node<K, V> node = hashTable[index];

            if (node.key.Equals(key))
            {
                hashTable[index] = null;
                return true;
            }

            while (node.next != null)
            {
                if (node.next.key.Equals(key))
                {
                    if (node.next.value.Equals(value))
                    {
                        node.next = node.next.next;
                        return true;
                    }
                    else return false;
                }
            }
            return false;
        }

        public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
        {
            int hc = key.GetHashCode();
            int index = (hashTable.Length - 1) & hc;

            Node<K, V> node = hashTable[index];

            while(node!=null)
            {
                if (node.hash == hc)
                {
                    if (node.key.Equals(key))
                    {
                        value = node.value;
                        return true;
                    }
                }
            }
            value = default(V);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public override String ToString()
        {
            StringBuilder sb = new StringBuilder($"capasity={hashTable.Length}  size={size}\n");
            
            for (int i=0; i<hashTable.Length; i++)
            {
                Node<K, V> node = hashTable[i];
                while(node!=null)
                {
                    sb.Append(node.ToString() + "\n");
                    node = node.next;
                }
            }
            return sb.ToString();

        }
    }
}
