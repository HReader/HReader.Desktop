using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HReader.Wpf.Shortcuts.Bindings;

namespace HReader.Wpf.Shortcuts
{
    public class ManualAsyncBindCollector : IEnumerable<ManualAsyncBinder>, IDisposable
    {
        private readonly List<ManualAsyncBinder> binders;

        public ManualAsyncBindCollector()
        {
            binders = new List<ManualAsyncBinder>();
        }

        public async Task<bool> ProcessAsync(Key key, ModifierKeys modifiers)
        {
            var tasks = binders.Select(binder => binder.ProcessAsync(key, modifiers));
            var res = await Task.WhenAll(tasks);
            return res.Any(b => b);
        }

        public async Task<bool> ProcessAsync(KeyEventArgs eventArgs)
        {
            if (await ProcessAsync(eventArgs.Key, eventArgs.Modifiers()))
            {
                eventArgs.Handled = true;
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var binder in binders)
            {
                binder.Dispose();
            }
        }

        public void Add(ManualAsyncBinder item)
        {
            binders.Add(item);
        }

        public void Clear()
        {
            binders.Clear();
        }

        public bool Remove(ManualAsyncBinder item)
        {
            return binders.Remove(item);
        }

        /// <inheritdoc />
        IEnumerator<ManualAsyncBinder> IEnumerable<ManualAsyncBinder>.GetEnumerator()
        {
            return binders.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) binders).GetEnumerator();
        }
    }
}