using System.Collections.Generic;
using System.Windows.Input;

namespace HReader.Wpf.Shortcuts.Bindings
{
    public class InputBinder : Binder
    {
        private InputBindingCollection inputBindings;

        private Dictionary<ShortcutBinding, InputBinding> mapping;

        private ICommand command;
        /// <inheritdoc />
        public InputBinder(Shortcut shortcut, InputBindingCollection inputBindings, ICommand command) : base(shortcut)
        {
            this.command = command;
            this.inputBindings = inputBindings;
            mapping = new Dictionary<ShortcutBinding, InputBinding>();
        }

        /// <inheritdoc />
        protected override void Bind(ShortcutBinding binding)
        {
            var input = new KeyBinding(command, binding.Key, binding.Modifiers);
            inputBindings.Add(input);
            mapping.Add(binding, input);
        }

        /// <inheritdoc />
        protected override void Unbind(ShortcutBinding binding)
        {
            if (!mapping.TryGetValue(binding, out var input)) return;
            mapping.Remove(binding);
            inputBindings.Remove(input);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            inputBindings = null;
            command = null;
            mapping = null;
        }
    }
}