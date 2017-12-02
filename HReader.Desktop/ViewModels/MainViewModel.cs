using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Dragablz;
using HReader.Persistence;
using HReader.Utility;
using HReader.ViewModels.Library;
using HReader.ViewModels.Tabs;
using HReader.Wpf.Shortcuts;
using HReader.Wpf.Shortcuts.Bindings;

namespace HReader.ViewModels
{
    internal sealed class MainViewModel : Conductor<TabViewModelBase>.Collection.OneActive, IDisposable
    {
        private const string WindowTitleBase = "HReader";

        private readonly Configuration config;
        private readonly ShortcutManager shortcutManager;

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]// needed for DI
        public MainViewModel(
            Configuration config,
            ShortcutManager shortcutManager, // manually injecting all tabs for now,
            SourcesViewModel sourcesVm,      // might want to replace this if more tabs get added
            ConfigurationViewModel configuration,
            DownloaderViewModel downloader,
            LibraryViewModel library)
        {
            DisplayName = WindowTitleBase;
            TabClosing = OnTabClosing;
            this.config = config;
            this.shortcutManager = shortcutManager;
            CreateShortcuts();
            Items.AddRange(new TabViewModelBase[]
            {
                library,
                downloader,
                sourcesVm,
                configuration,
            });

            library.ConductWith(this);
            downloader.ConductWith(this);
            sourcesVm.ConductWith(this);
            configuration.ConductWith(this);

            var tabId = config.State.ActiveTab ?? string.Empty;
            var tab = Items.FirstOrDefault(t => t.Id == tabId);
            if (tab != null)
            {
                ActivateItem(tab);
            }
        }

        private void CreateShortcuts()
        {
            var closeShortcut = new Shortcut("Close Application", "Saves everything and closes the application. Never displays a dialog.");
            shortcutManager.Add("application.close", closeShortcut, new ShortcutBinding(Key.Escape));

            // reader shortcuts
            shortcutManager.Add(
                "reader.next-page", new Shortcut(
                    "Next Page",
                    "Navigates to the next page in the reader if there is one."),
                new ShortcutBinding(Key.D),
                new ShortcutBinding(Key.Right));

            shortcutManager.Add(
                "reader.previous-page", new Shortcut(
                    "Previous Page",
                    "Navigates to the previous page in the reader if there is one."),
                new ShortcutBinding(Key.A),
                new ShortcutBinding(Key.Left));

            shortcutManager.Add(
                "reader.fit-vertically",
                new Shortcut(
                    "Fit Vertically",
                    "Resizes the page to fill the vertical space completely."),
                new ShortcutBinding(Key.V));

            shortcutManager.Add(
                "reader.fit-horizonally",
                new Shortcut(
                    "Fit Horizontally",
                    "Resizes the page to fill the horizontal space completely."),
                new ShortcutBinding(Key.H));

            shortcutManager.Add(
                "reader.fit-original",
                new Shortcut(
                    "Original Size",
                    "Shows the page in its original size."),
                new ShortcutBinding(Key.B));





            applicationCloseBinding = shortcutManager.Bind("application.close",
                                                           s => new ManualBinder(s, () => TryClose()));

        }

        private ManualBinder applicationCloseBinding;
        
        public void KeyPress(KeyEventArgs e)
        {
            if (applicationCloseBinding.Process(e.Key, e.KeyboardDevice.Modifiers)) e.Handled = true;
        }

        private void Serialize()
        {
            config.State.ActiveTab = ActiveItem?.Id;
            shortcutManager.Persist();
            config.Save();
        }

        /// <inheritdoc />
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                Serialize();
                Dispose();
            }

            base.OnDeactivate(close);
        }
        
        private void UpdateDisplayName(IHaveDisplayName item)
        {
            DisplayName = !string.IsNullOrWhiteSpace(item.DisplayName)
                ? $"{item.DisplayName} - {WindowTitleBase}"
                : WindowTitleBase;
        }

        /// <inheritdoc />
        protected override void OnActivationProcessed(TabViewModelBase item, bool success)
        {
            if (success && item != null)
            {
                UpdateDisplayName(item);
            }

            base.OnActivationProcessed(item, success);
        }

        public void OnTabClosing(ItemActionCallbackArgs<TabablzControl> args)
        {
            (args.DragablzItem.DataContext as TabViewModelBase)?.Dispose();
        }

        public ItemActionCallback TabClosing { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var tab in Items)
            {
                tab?.Dispose();
            }

            applicationCloseBinding?.Dispose();
        }
    }
}
