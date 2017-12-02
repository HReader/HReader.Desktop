using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HReader.Utility
{
    internal interface IMapper<in TSource, out T>
    {
        T Map(TSource source);
    }

    internal interface IMappedCollection<out T>  : IReadOnlyList<T>,
                                                   INotifyCollectionChanged,
                                                   INotifyPropertyChanged { }

    internal static class MappingCollection
    {
        public static MappingCollection<TSource, T, ObservableCollection<TSource>> Create<TSource, T>(ObservableCollection<TSource> source, Func<TSource, T> mapper)
        {
            return new MappingCollection<TSource, T, ObservableCollection<TSource>>(source, mapper);
        }

        public static MappingCollection<TSource, T, ObservableCollection<TSource>> Create<TSource, T>(ObservableCollection<TSource> source, IMapper<TSource, T> mapper)
        {
            return new MappingCollection<TSource, T, ObservableCollection<TSource>>(source, mapper);
        }

        public static MappingCollection<TSource, T, ReadOnlyObservableCollection<TSource>> Create<TSource, T>(ReadOnlyObservableCollection<TSource> source, Func<TSource, T> mapper)
        {
            return new MappingCollection<TSource, T, ReadOnlyObservableCollection<TSource>>(source, mapper);
        }

        public static MappingCollection<TSource, T, ReadOnlyObservableCollection<TSource>> Create<TSource, T>(ReadOnlyObservableCollection<TSource> source, IMapper<TSource, T> mapper)
        {
            return new MappingCollection<TSource, T, ReadOnlyObservableCollection<TSource>>(source, mapper);
        }

        public static MappingCollection<TSource, T, TCollection> Create<TSource, T, TCollection>(TCollection source, IMapper<TSource, T> mapper)
            where TCollection : IReadOnlyCollection<TSource>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new MappingCollection<TSource, T, TCollection>(source, mapper);
        }

        public static MappingCollection<TSource, T, TCollection> Create<TSource, T, TCollection>(TCollection source, Func<TSource, T> mapper)
            where TCollection : IReadOnlyCollection<TSource>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new MappingCollection<TSource, T, TCollection>(source, mapper);
        }
    }

    internal class MappingCollection<TSource, T, TCollection> : IMappedCollection<T>, IDisposable
        where TCollection: IReadOnlyCollection<TSource>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private class DelegateMapper : IMapper<TSource, T>
        {
            private readonly Func<TSource, T> mapper;

            public DelegateMapper(Func<TSource, T> mapper)
            {
                this.mapper = mapper;
            }

            /// <inheritdoc />
            public T Map(TSource source)
            {
                return mapper(source);
            }
        }

        private readonly IMapper<TSource, T> mapper;
        private readonly TCollection source;
        private List<T> items;
        
        public MappingCollection(TCollection source, Func<TSource, T> mapper)
            : this(source, new DelegateMapper(mapper)) { }

        public MappingCollection(TCollection source, IMapper<TSource, T> mapper)
        {
            this.mapper = mapper;
            this.source = source;
            RemapItems();

            this.source.CollectionChanged += OnSourceCollectionChanged;
            (this.source as INotifyPropertyChanged).PropertyChanged += OnSourcePropertyChanged;
        }
        
        private void RemapItems()
        {
            items = source.Select(s => mapper.Map(s))
                          .ToList();
        }

        #region CollectionChanged handler
        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnSourceAdd(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnSourceRemove(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    OnSourceReplace(e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    OnSourceMove(e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnSourceReset(e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void OnSourceAdd(NotifyCollectionChangedEventArgs e)
        {
            var list = e.NewItems
                        .Cast<TSource>()
                        .Select(s => mapper.Map(s))
                        .ToList();

            if (e.NewStartingIndex >= 0)
            {
                items.InsertRange(e.NewStartingIndex, list);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, list, e.NewStartingIndex));
            }
            else
            {
                items.AddRange(list);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, list));
            }
        }

        protected virtual void OnSourceRemove(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldStartingIndex >= 0)
            {
                var list = items.GetRange(e.OldStartingIndex, e.OldItems.Count);
                items.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, list, e.OldStartingIndex));
            }
            else
            {
                //TODO: replace with proper implementation
                RemapItems();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected virtual void OnSourceReplace(NotifyCollectionChangedEventArgs e)
        {
            //TODO: replace with proper implementation
            RemapItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void OnSourceMove(NotifyCollectionChangedEventArgs e)
        {
            //TODO: replace with proper implementation
            RemapItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void OnSourceReset(NotifyCollectionChangedEventArgs e)
        {
            RemapItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region PropertyChanged delegation
        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Count))
            {
                OnPropertyChanged(e.PropertyName);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IReadOnlyList delegation
        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public int Count => items.Count;
        /// <inheritdoc />
        public T this[int index] => items[index];
        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            source.CollectionChanged -= OnSourceCollectionChanged;
            source.PropertyChanged -= OnSourcePropertyChanged;
        }
    }
}
