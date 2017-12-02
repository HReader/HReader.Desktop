using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HReader.Wpf.Shortcuts.Bindings
{
    public class ManualAsyncBinder : Binder
    {
        private Func<Task> onPressed;

        /// <inheritdoc />
        public ManualAsyncBinder(Shortcut shortcut, Func<Task> onPressed) : base(shortcut)
        {
            this.onPressed = onPressed;
        }

        // irrelevant, we check all registered bindings manually
        protected override void Bind(ShortcutBinding binding) { }
        protected override void Unbind(ShortcutBinding binding) { }

        public async Task<bool> ProcessAsync(Key key, ModifierKeys modifiers)
        {
            if (Bindings.Any(binding => binding           != null     && 
                                        binding.Key       == key      &&
                                        binding.Modifiers == modifiers))
            {
                await onPressed();
                return true;
            }
            return false;
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
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            onPressed = null;
        }
    }
}