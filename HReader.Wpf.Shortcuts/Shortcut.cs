using System.Collections.ObjectModel;

namespace HReader.Wpf.Shortcuts
{
    public sealed class Shortcut
    {
        public Shortcut(string name, string description)
        {
            Name = name;
            Description = description;
            Bindings = new ObservableCollection<ShortcutBinding>();
        }

        public string Name { get; }
        public string Description { get; }

        public ObservableCollection<ShortcutBinding> Bindings { get; }
    }
}
