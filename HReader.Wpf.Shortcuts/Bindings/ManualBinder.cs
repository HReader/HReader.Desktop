using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HReader.Wpf.Shortcuts.Bindings
{
    public class ManualBinder : Binder
    {
        private Action onPressed;

        /// <inheritdoc />
        public ManualBinder(Shortcut shortcut, Action onPressed) : base(shortcut)
        {
            this.onPressed = onPressed;
        }

        // irrelevant, we check all registered bindings manually
        protected override void Bind(ShortcutBinding binding) { }
        protected override void Unbind(ShortcutBinding binding) { }

        public bool Process(Key key, ModifierKeys modifiers)
        {
            if (Bindings.Any(binding => binding != null 
                                     && binding.Key == key 
                                     && binding.Modifiers == modifiers))
            {
                onPressed();
                return true;
            }
            return false;
        }

        public bool Process(KeyEventArgs eventArgs)
        {
            if(Process(eventArgs.Key, eventArgs.Modifiers()))
            {
                eventArgs.Handled = true;
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            onPressed = null;
        }
    }
}
