using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HReader.Wpf.Shortcuts
{
    /// <summary>
    /// A Control that handles setting a shortcut.
    /// </summary>
    public partial class ShortcutEditorControl : UserControl
    {
        private class DummyCommand : ICommand
        {
            public static readonly ICommand Instance = new DummyCommand();
            private DummyCommand() { }
            bool ICommand.CanExecute(object parameter) => false;
            void ICommand.Execute(object parameter) { }
            #pragma warning disable 67 //event never used
            event EventHandler ICommand.CanExecuteChanged { add { } remove { } }
            #pragma warning restore 67
        }

        public static readonly DependencyProperty ShortcutProperty = DependencyProperty.Register(
            nameof(Shortcut), 
            typeof(ShortcutBinding),
            typeof(ShortcutEditorControl),
            new FrameworkPropertyMetadata(
                default(ShortcutBinding),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public static readonly DependencyProperty ValidateInputBindingProperty = DependencyProperty.Register(
            nameof(ValidateInputBinding),
            typeof(bool),
            typeof(ShortcutEditorControl),
            new PropertyMetadata(
                defaultValue: false
        ));

        public bool ValidateInputBinding
        {
            get => (bool) GetValue(ValidateInputBindingProperty);
            set => SetValue(ValidateInputBindingProperty, value);
        }

        public ShortcutBinding Shortcut
        {
            get => (ShortcutBinding) GetValue(ShortcutProperty);
            set => SetValue(ShortcutProperty, value);
        }

        public ShortcutEditorControl()
        {
            InitializeComponent();
        }

        private static bool IsValid(Key key, ModifierKeys modifiers)
        {
            try
            {
                //todo: is there a better way to do this?
                new KeyBinding(DummyCommand.Instance, key, modifiers);
            }
            catch (NotSupportedException)
            {
                return false;
            }
            return true;
        }
        
        private void ShortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Don't let the event pass further
            // because we don't want standard textbox shortcuts working
            e.Handled = true;

            // Get modifiers and key data
            var modifiers = e.Modifiers();
            var key = e.Key;

            // When Alt is pressed, SystemKey is used instead
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None && key.In(Key.Delete, Key.Back, Key.Escape))
            {
                Shortcut = null;
                return;
            }

            // If no actual key was pressed - return
            if (key.In(
                Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt,
                Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin,
                Key.Clear, Key.OemClear, Key.Apps))
            {
                return;
            }

            //todo: properly display this validation failure
            if (ValidateInputBinding && !IsValid(key, modifiers))
            {
                return;
            }

            Shortcut = new ShortcutBinding(key, modifiers);
        }
    }
}
