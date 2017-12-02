using System.Text;
using System.Windows.Input;

namespace HReader.Wpf.Shortcuts
{
    public sealed class ShortcutBinding
    {
        public ShortcutBinding(Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            Key = key;
            Modifiers = modifiers;
        }
        
        public Key Key { get; }
        public ModifierKeys Modifiers { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Key == Key.None) return string.Empty;
            if (Modifiers == ModifierKeys.None) return Key.ToString();

            var sb = new StringBuilder();

            if ((Modifiers & ModifierKeys.Control) > 0) sb.Append("Ctrl + ");
            if ((Modifiers & ModifierKeys.Alt)     > 0) sb.Append("Alt + ");
            if ((Modifiers & ModifierKeys.Shift)   > 0) sb.Append("Shift + ");
            if ((Modifiers & ModifierKeys.Windows) > 0) sb.Append("Win + ");
            sb.Append(Key);

            return sb.ToString();
        }
    }
}