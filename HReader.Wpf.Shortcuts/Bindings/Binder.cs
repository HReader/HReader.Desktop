using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace HReader.Wpf.Shortcuts.Bindings
{
    public abstract class Binder : IDisposable
    {
        private Shortcut shortcut;
        protected readonly List<ShortcutBinding> Bindings;

        protected Binder(Shortcut shortcut)
        {
            this.shortcut = shortcut;
            Bindings = new List<ShortcutBinding>();
            shortcut.Bindings.CollectionChanged += BindingsOnCollectionChanged;
        }

        internal void Initialize()
        {
            UpdateBindings();
        }

        private void BindingsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateBindings();
        }

        protected abstract void Bind(ShortcutBinding binding);
        protected abstract void Unbind(ShortcutBinding binding);

        private void UpdateBindings()
        {
            var newItems = shortcut.Bindings.Except(Bindings).ToList();
            var oldItems = Bindings.Except(shortcut.Bindings).ToList();

            foreach (var binding in oldItems)
            {
                if (binding != null)
                {
                    Unbind(binding);
                }
                Bindings.Remove(binding);
            }

            foreach (var binding in newItems)
            {
                if (binding != null)
                {
                    Bind(binding);
                }
                Bindings.Add(binding);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            shortcut.Bindings.CollectionChanged -= BindingsOnCollectionChanged;

            foreach (var binding in Bindings)
            {
                Unbind(binding);
            }

            shortcut = null;
            Bindings.Clear();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}