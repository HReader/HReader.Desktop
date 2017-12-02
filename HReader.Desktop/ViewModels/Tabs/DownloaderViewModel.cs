using System;
using System.Threading.Tasks;
using HReader.Core.Sources;
using HReader.Core.Storage;

namespace HReader.ViewModels.Tabs
{
    internal sealed class DownloaderViewModel : TabViewModelBase
    {
        private readonly IMetadataRepository repository;
        private readonly ISourceManager sourceManager;

        public DownloaderViewModel(IMetadataRepository repository, ISourceManager sourceManager) : base("downloader")
        {
            this.repository = repository;
            this.sourceManager = sourceManager;
            DisplayName = "Downloader";
        }

        public string Url { get; set; }

        public async Task AddToRepository()
        {
            var meta = await sourceManager.ResolveMetadataAsync(new Uri(Url, UriKind.Absolute));
            await repository.AddAsync(meta);
        }
    }
}
