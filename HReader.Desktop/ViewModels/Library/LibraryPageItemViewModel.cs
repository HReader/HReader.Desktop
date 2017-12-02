using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using HReader.Base.Data;
using HReader.Core.Sources;
using HReader.ViewModels.Tabs;

namespace HReader.ViewModels.Library
{
    internal class LibraryPageItemViewModel : Screen
    {
        public sealed class Factory
        {
            private readonly ISourceManager sourceManager;
            private readonly ReaderViewModel.Factory readerFactory;

            public Factory(ISourceManager sourceManager, ReaderViewModel.Factory readerFactory)
            {
                this.sourceManager = sourceManager;
                this.readerFactory = readerFactory;
            }

            public LibraryPageItemViewModel Create(IMetadata data, Conductor<TabViewModelBase>.Collection.OneActive conductor)
            {
                return new LibraryPageItemViewModel(data, sourceManager, readerFactory, conductor);
            }
        }

        private readonly IMetadata data;
        private readonly ReaderViewModel.Factory readerFactory;
        private readonly Conductor<TabViewModelBase>.Collection.OneActive conductor;

        public LibraryPageItemViewModel(IMetadata data, ISourceManager sourceManager, ReaderViewModel.Factory readerFactory, Conductor<TabViewModelBase>.Collection.OneActive conductor)
        {
            this.data = data;
            this.readerFactory = readerFactory;
            this.conductor = conductor;
            sourceManager.ConsumeAsync(data.Cover, SetCover);
        }

        private async Task SetCover(Stream stream)
        {
            var mem = new MemoryStream();
            await stream.CopyToAsync(mem);

            OnUIThread(() =>
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = mem;
                img.EndInit();
                Cover = img;
            });

        }

        public ImageSource Cover { get; private set; }
        
        public int PageCount => data.Pages.Count;

        public string Title => data.Title;

        public void Open(MouseButtonEventArgs e)
        {
            var reader = readerFactory.Create(data);
            conductor.Items.Add(reader);
            reader.ConductWith(conductor);
            e.Handled = true;
        }
    }
}
