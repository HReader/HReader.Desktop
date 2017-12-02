using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HReader.Wpf.Shortcuts.Bindings;

namespace HReader.Wpf.Shortcuts
{
    public class ShortcutManager : IReadOnlyCollection<Shortcut>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IShortcutPersistenceProvider persistenceProvider;
        private readonly Dictionary<string, Shortcut> shortcuts;

        public ShortcutManager(IShortcutPersistenceProvider persistenceProvider)
        {
            this.persistenceProvider = persistenceProvider;
            shortcuts = new Dictionary<string, Shortcut>();
        }

        public void Add(string id, Shortcut shortcut, params ShortcutBinding[] defaultBindings)
        {
            Add(id, shortcut, (IEnumerable<ShortcutBinding>)defaultBindings);
        }

        public void Add(string id, Shortcut shortcut, IEnumerable<ShortcutBinding> defaultBindings)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (shortcut == null) throw new ArgumentNullException(nameof(shortcut));
            if (shortcuts.ContainsKey(id)) throw new ArgumentException("Duplicate id.", nameof(id));

            shortcuts.Add(id, shortcut);

            if (persistenceProvider.TryGetValue(id, out var savedBindings))
            {
                foreach (var binding in savedBindings)
                {
                    shortcut.Bindings.Add(binding);
                }
            }
            else
            {
                foreach (var binding in defaultBindings ?? Enumerable.Empty<ShortcutBinding>())
                {
                    shortcut.Bindings.Add(binding);
                }
            }

            OnCollectionChanged(NotifyCollectionChangedAction.Add, shortcut);
            OnPropertyChanged(nameof(Count));
        }

        public void Remove(string id)
        {
            if (!shortcuts.TryGetValue(id, out var shortcut)) return;

            shortcuts.Remove(id);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, shortcut);
            OnPropertyChanged(nameof(Count));
        }

        public T Bind<T>(string id, Func<Shortcut, T> binderFactory)
            where T: Binder
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (!shortcuts.TryGetValue(id, out var shortcut))
                throw new ArgumentException($"Id does not exist: {id}", nameof(id));

            var binder = binderFactory(shortcut);
            binder.Initialize();
            return binder;
        }

        public IDisposable Bind(string id, InputBindingCollection inputBindings, ICommand command)
        {
            if (inputBindings == null) throw new ArgumentNullException(nameof(inputBindings));
            if (command == null) throw new ArgumentNullException(nameof(command));

            return Bind(id, shortcut => new InputBinder(shortcut, inputBindings, command));
        }

        public IDisposable Bind(string id, Action<ShortcutBinding> onBind, Action<ShortcutBinding> onUnbind)
        {
            if (onBind == null) throw new ArgumentNullException(nameof(onBind));
            if (onUnbind == null) throw new ArgumentNullException(nameof(onUnbind));

            return Bind(id, shortcut => new ActionBinder(shortcut, onBind, onUnbind));
        }

        public void Persist()
        {
            persistenceProvider.Persist(
                shortcuts.ToDictionary(
                    kv => kv.Key,
                    kv => (IList<ShortcutBinding>)kv.Value.Bindings.ToList()
            ));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, object item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        }

        /// <inheritdoc />
        public IEnumerator<Shortcut> GetEnumerator()
        {
            return shortcuts.Values.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public int Count => shortcuts.Count;

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}