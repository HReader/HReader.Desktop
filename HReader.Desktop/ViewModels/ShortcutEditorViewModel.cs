using System;
using HReader.Wpf.Shortcuts;
using PropertyChanged;

namespace HReader.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class ShortcutEditorViewModel
    {
        public ShortcutEditorViewModel(Shortcut shortcut)
        {
            this.shortcut = shortcut ?? throw new ArgumentNullException(nameof(shortcut));
        }

        private readonly Shortcut shortcut;

        private void SetBinding(ShortcutBinding binding, int index)
        {
            if (index < shortcut.Bindings.Count)
            {
                if (binding == null)
                {
                    shortcut.Bindings.RemoveAt(index);
                }
                else
                {
                    shortcut.Bindings[index] = binding;
                }
            }
            else if (index == shortcut.Bindings.Count)
            {
                if (binding != null)
                {
                    shortcut.Bindings.Add(binding);
                }
            }
            else
            {
                throw new InvalidOperationException("Index out of valid range.");
            }
        }

        private ShortcutBinding GetBinding(int index)
        {
            return index < shortcut.Bindings.Count 
                ? shortcut.Bindings[index] 
                : null;
        }

        public ShortcutBinding PrimaryShortcut
        {
            get => GetBinding(0);
            set => SetBinding(value, 0);
        }

        [DependsOn(nameof(PrimaryShortcut))]
        public ShortcutBinding SecondaryShortcut
        {
            get => GetBinding(1);
            set => SetBinding(value, 1);
        }

        public string Name => shortcut.Name;
        public string Description => shortcut.Description;

        [DependsOn(nameof(PrimaryShortcut))]
        public bool IsSecondaryEnabled => PrimaryShortcut != null;
    }
}
