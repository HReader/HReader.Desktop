using System;

namespace HReader.Wpf.Shortcuts.Bindings
{
    public class ActionBinder : Binder
    {
        private Action<ShortcutBinding> onBind;
        private Action<ShortcutBinding> onUnbind;

        public ActionBinder(Shortcut shortcut, Action<ShortcutBinding> onBind, Action<ShortcutBinding> onUnbind) : base(shortcut)
        {
            this.onBind = onBind;
            this.onUnbind = onUnbind;
        }

        /// <inheritdoc />
        protected override void Bind(ShortcutBinding binding)
        {
            onBind(binding);
        }

        /// <inheritdoc />
        protected override void Unbind(ShortcutBinding binding)
        {
            onUnbind(binding);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            onUnbind = null;
            onBind = null;
        }
    }
}