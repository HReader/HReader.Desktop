using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using HReader.Utility;
using HReader.Wpf.Shortcuts;

namespace HReader.ViewModels.Tabs
{
    internal sealed class ConfigurationViewModel : TabViewModelBase
    {
        public ConfigurationViewModel(ShortcutManager shortcutManager) : base("configuration")
        {
            DisplayName = "Configuration";

            Shortcuts = MappingCollection.Create<Shortcut, ShortcutEditorViewModel, ShortcutManager>(shortcutManager, source => new ShortcutEditorViewModel(source));
        }

        public IMappedCollection<ShortcutEditorViewModel> Shortcuts { get; }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            (Shortcuts as MappingCollection<Shortcut, ShortcutEditorViewModel, ShortcutManager>)?.Dispose();
            base.Dispose(disposing);
        }
    }
}
