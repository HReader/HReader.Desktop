using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using HReader.Wpf.Shortcuts.Bindings;

namespace HReader.Wpf.Shortcuts
{
    public class ManualBindCollector : IEnumerable<ManualBinder>, IDisposable
    {
        private readonly List<ManualBinder> binders;

        public ManualBindCollector()
        {
            binders = new List<ManualBinder>();
        }

        public bool Process(Key key, ModifierKeys modifiers)
        {
            var result = false;
            foreach (var binder in binders)
            {
                if (binder.Process(key, modifiers))
                {
                    result = true;
                }
            }
            return result;
        }

        public bool Process(KeyEventArgs eventArgs)
        {
            if (Process(eventArgs.Key, eventArgs.Modifiers()))
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
        
        public void Add(ManualBinder item)
        {
            binders.Add(item);
        }
        
        public void Clear()
        {
            binders.Clear();
        }
        
        public bool Remove(ManualBinder item)
        {
            return binders.Remove(item);
        }

        /// <inheritdoc />
        IEnumerator<ManualBinder> IEnumerable<ManualBinder>.GetEnumerator()
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
