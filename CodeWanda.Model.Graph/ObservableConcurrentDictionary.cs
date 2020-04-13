//--------------------------------------------------------------------------
// 
//  Copyright (c) Microsoft Corporation.  All rights reserved. 
// 
//  File: ObservableConcurrentDictionary.cs
//
//--------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace CodeWanda.Model.Graph
{
    /// <summary>
    /// Provides a thread-safe dictionary for use with data binding.
    /// </summary>
    /// <typeparam name="TKey">Specifies the type of the keys in this collection.</typeparam>
    /// <typeparam name="TValue">Specifies the type of the values in this collection.</typeparam>
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public sealed class ObservableConcurrentDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SynchronizationContext _context;
        private readonly ConcurrentDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// class that is empty, has the default concurrency level, has the default initial capacity, and
        /// uses the default comparer for the key type.
        /// </summary>
        public ObservableConcurrentDictionary()
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _dictionary = new ConcurrentDictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// class that is empty, has the specified concurrency level and capacity, and uses the default
        /// comparer for the key type.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the
        /// <see cref="ObservableConcurrentDictionary{TKey,TValue}"/> concurrently.</param>
        /// <param name="capacity">The initial number of elements that the <see
        /// cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// can contain.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is
        /// less than 1.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"> <paramref name="capacity"/> is less than
        /// 0.</exception>
        public ObservableConcurrentDictionary(int concurrencyLevel, int capacity)
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// class that contains elements copied from the specified <see
        /// cref="System.Collections.Generic.IEnumerable{T}"/>, has the default concurrency
        /// level, has the default initial capacity, and uses the default comparer for the key type.
        /// </summary>
        /// <param name="collection">The <see
        /// cref="System.Collections.Generic.IEnumerable{T}"/> whose elements are copied to
        /// the new
        /// <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException"><paramref name="collection"/> contains one or more
        /// duplicate keys.</exception>
        public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _dictionary = new ConcurrentDictionary<TKey, TValue>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// class that is empty, has the specified concurrency level and capacity, and uses the specified
        /// <see cref="System.Collections.Generic.IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IEqualityComparer{TKey}"/>
        /// implementation to use when comparing keys.</param>
        public ObservableConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _dictionary = new ConcurrentDictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// class that contains elements copied from the specified <see
        /// cref="System.Collections.IEnumerable"/>, has the default concurrency level, has the default
        /// initial capacity, and uses the specified
        /// <see cref="System.Collections.Generic.IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="collection">The <see
        /// cref="System.Collections.Generic.IEnumerable{T}"/> whose elements are copied to
        /// the new
        /// <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IEqualityComparer{TKey}"/>
        /// implementation to use when comparing keys.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection,
            IEqualityComparer<TKey> comparer)
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _dictionary = new ConcurrentDictionary<TKey, TValue>(collection, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// class that contains elements copied from the specified <see cref="System.Collections.IEnumerable"/>,
        /// has the specified concurrency level, has the specified initial capacity, and uses the specified
        /// <see cref="System.Collections.Generic.IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the
        /// <see cref="ObservableConcurrentDictionary{TKey,TValue}"/> concurrently.</param>
        /// <param name="collection">The <see cref="System.Collections.Generic.IEnumerable{T}"/> whose elements are copied to the new
        /// <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IEqualityComparer{TKey}"/> implementation to use
        /// when comparing keys.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="collection"/> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel"/> is less than 1.
        /// </exception>
        /// <exception cref="System.ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        public ObservableConcurrentDictionary(
            int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, collection, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// class that is empty, has the specified concurrency level, has the specified initial capacity, and
        /// uses the specified <see cref="System.Collections.Generic.IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the
        /// <see cref="ObservableConcurrentDictionary{TKey,TValue}"/> concurrently.</param>
        /// <param name="capacity">The initial number of elements that the <see
        /// cref="ObservableConcurrentDictionary{TKey,TValue}"/>
        /// can contain.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IEqualityComparer{TKey}"/>
        /// implementation to use when comparing keys.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel"/> is less than 1. -or-
        /// <paramref name="capacity"/> is less than 0.
        /// </exception>
        public ObservableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey>? comparer)
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity, comparer);
        }

        /// <summary>Event raised when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>Event raised when a property on the collection changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary.
        /// </summary>
        private void NotifyObserversOfChange(NotifyCollectionChangedEventArgs args)
        {
            var collectionHandler = CollectionChanged;
            var propertyHandler = PropertyChanged;
            if (collectionHandler != null || propertyHandler != null)
            {
                _context.Send(s =>
                {
                    collectionHandler?.Invoke(this, args);
                    propertyHandler?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    propertyHandler?.Invoke(this, new PropertyChangedEventArgs(nameof(Keys)));
                    propertyHandler?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
                }, null);
            }
        }

        /// <summary>
        /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary.
        /// </summary>
        private void NotifyObserversOfChange()
        {
            NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary.
        /// </summary>
        /// <param name="actionType">Add or Update action</param>
        /// <param name="changedItem">The item involved with the change</param>
        private void NotifyObserversOfChange(NotifyCollectionChangedAction actionType, object changedItem)
        {
            NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(actionType, changedItem));
        }

        /// <summary>
        /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary.
        /// </summary>
        /// <param name="actionType">Remove action or optionally an Add action</param>
        /// <param name="item">The item in question</param>
        /// <param name="index">The position of the item in the collection</param>
        private void NotifyObserversOfChange(NotifyCollectionChangedAction actionType, object item, int index)
        {
            NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(actionType, item, index));
        }

        /// <summary>Attempts to add an item to the dictionary, notifying observers of any changes.</summary>
        /// <param name="item">The item to be added.</param>
        /// <returns>Whether the add was successful.</returns>
        private bool TryAddWithNotification(KeyValuePair<TKey, TValue> item)
            => TryAddWithNotification(item.Key, item.Value);

        /// <summary>Attempts to add an item to the dictionary, notifying observers of any changes.</summary>
        /// <param name="key">The key of the item to be added.</param>
        /// <param name="value">The value of the item to be added.</param>
        /// <returns>Whether the add was successful.</returns>
        private bool TryAddWithNotification(TKey key, TValue value)
        {
            var result = _dictionary.TryAdd(key, value);
            if (result)
            {
                var index = IndexOf(key);
                NotifyObserversOfChange(NotifyCollectionChangedAction.Add, value, index);
            }

            return result;
        }

        /// <summary>Attempts to remove an item from the dictionary, notifying observers of any changes.</summary>
        /// <param name="key">The key of the item to be removed.</param>
        /// <param name="value">The value of the item removed.</param>
        /// <returns>Whether the removal was successful.</returns>
        private bool TryRemoveWithNotification(TKey key, out TValue value)
        {
            var index = IndexOf(key);
            var result = _dictionary.TryRemove(key, out value);
            if (result) NotifyObserversOfChange(NotifyCollectionChangedAction.Remove, value, index);
            return result;
        }

        /// <summary>Attempts to add or update an item in the dictionary, notifying observers of any changes.</summary>
        /// <param name="key">The key of the item to be updated.</param>
        /// <param name="value">The new value to set for the item.</param>
        /// <returns>Whether the update was successful.</returns>
        private void UpdateWithNotification(TKey key, TValue value)
        {
            bool added = false, updated = false;
            NotifyCollectionChangedEventArgs args = null;
            
            _dictionary.AddOrUpdate(
                key,
                (_, arg) =>
                {
                    added = true;
                    return arg;
                },
                (_, old, arg) =>
                {
                    updated = true;
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, arg, old);
                    return arg;
                },
                value
            );

            if (added)
            {
                NotifyObserversOfChange(NotifyCollectionChangedAction.Add, value, IndexOf(key));
            }

            if (updated)
            {
                NotifyObserversOfChange(args);
            }
        }

        /// <summary>
        /// WPF requires that the reported index for Add/Remove events are correct/reliable. With a dictionary there
        /// is no choice but to brute-force search through the key list. Ugly.
        /// </summary>
        private int IndexOf(TKey key)
        {
            var keys = _dictionary.Keys;
            var index = -1;
            foreach (var k in keys)
            {
                index++;
                if (k.Equals(key)) return index;
            }

            return -1;
        }


        // ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
            => TryAddWithNotification(item);

        public void Clear()
        {
            _dictionary.Clear();
            NotifyObserversOfChange();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).CopyTo(array, arrayIndex);

        public int Count => _dictionary.Count;

        public bool IsReadOnly =>
            ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).IsReadOnly;

        public bool Remove(KeyValuePair<TKey, TValue> item)
            => TryRemoveWithNotification(item.Key, out var temp);


        // IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _dictionary.GetEnumerator();


        // IReadOnlyDictionary<TKey, TValue> Members

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => _dictionary.Count;


        // IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
            => TryAddWithNotification(key, value);

        public bool ContainsKey(TKey key)
            => _dictionary.ContainsKey(key);

        public ICollection<TKey> Keys => _dictionary.Keys;

        public bool Remove(TKey key)
            => TryRemoveWithNotification(key, out _);

        public bool TryGetValue(TKey key, out TValue value)
            => _dictionary.TryGetValue(key, out value);

        public ICollection<TValue> Values => _dictionary.Values;

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => UpdateWithNotification(key, value);
        }
    }
}