using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HReader.Base.Data;
using HReader.Core.Sources;
using HReader.Core.Caching;
using HReader.Wpf.Shortcuts;
using HReader.Wpf.Shortcuts.Bindings;
using PropertyChanged;

namespace HReader.ViewModels.Tabs
{
    internal sealed class ReaderViewModel : TabViewModelBase
    {
        public class Factory
        {
            private readonly ISourceManager sourceManager;
            private readonly ShortcutManager shortcutManager;

            public Factory(ISourceManager sourceManager, ShortcutManager shortcutManager)
            {
                this.sourceManager = sourceManager;
                this.shortcutManager = shortcutManager;
            }

            public ReaderViewModel Create(IMetadata metadata)
            {
                return new ReaderViewModel(shortcutManager, sourceManager, metadata); //TODO: add parameters
            }
        }

        private readonly ShortcutManager shortcutManager;
        private readonly ISourceManager sourceManager;
        private readonly IMetadata metadata;
        private ManualAsyncBindCollector binds;

        private readonly IReaderCache cache;
        private bool isNavigating;

        public ReaderViewModel(ShortcutManager shortcutManager, ISourceManager sourceManager, IMetadata metadata) : base("reader")
        {
            DisplayName = "Read " + metadata.Title;
            this.shortcutManager = shortcutManager;
            this.sourceManager = sourceManager;
            this.metadata = metadata;
            CreateShortcuts();
            cache = new MemoryReaderCache(metadata.Pages, sourceManager);

            cache.NavigateToAsync(0).ContinueWith(async t =>
            {
                SetImage(await t);
            });
        }

        private void CreateShortcuts()
        {
            binds = new ManualAsyncBindCollector
            {
                shortcutManager.Bind("reader.next-page", s => new ManualAsyncBinder(s, NextPageAsync)),
                shortcutManager.Bind("reader.previous-page", s => new ManualAsyncBinder(s, PreviousPageAsync))
            };
        }

        public ImageSource CurrentImage { get; private set; }

        [DependsOn(nameof(CurrentImage))]
        public Visibility LoadingVisible => (CurrentImage == null) ? Visibility.Visible : Visibility.Collapsed;

        private void SetImage(Stream stream)
        {
            if (stream == null) return;

            OnUIThread(() =>
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = stream;
                img.CacheOption = BitmapCacheOption.None;
                img.EndInit();

                CurrentImage = img;
            });
        }

        private async Task NextPageAsync()
        {
            if (isNavigating || !cache.HasNext) return;

            isNavigating = true;
            CurrentImage = null;
            SetImage(await cache.NavigateNextAsync());
            isNavigating = false;
        }

        private async Task PreviousPageAsync()
        {
            if (isNavigating || !cache.HasPrevious) return;

            isNavigating = true;
            CurrentImage = null;
            SetImage(await cache.NavigatePreviousAsync());
            isNavigating = false;
        }

        public async Task KeyPressAsync(KeyEventArgs e)
        {
            await binds.ProcessAsync(e);
        }
        
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            binds?.Dispose();
            cache?.Dispose();
            base.Dispose(disposing);
        }
    }
}
