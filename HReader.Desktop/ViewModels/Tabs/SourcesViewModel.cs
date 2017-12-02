using System.Collections.ObjectModel;
using HReader.Base;
using HReader.Core.Sources;
using HReader.Utility;

namespace HReader.ViewModels.Tabs
{
    internal sealed class SourcesViewModel : TabViewModelBase
    {
        public SourcesViewModel(ISourceManager sourceManager) : base("sources")
        {
            DisplayName = "Sources";
            
            Metadata = MappingCollection.Create(sourceManager.Metadata, source => new MetadataSourceViewModel(source));
            Content  = MappingCollection.Create(sourceManager.Content,  source => new ContentSourceViewModel(source));
        }

        public IMappedCollection<MetadataSourceViewModel> Metadata { get; }
        public IMappedCollection<ContentSourceViewModel> Content { get; }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            (Metadata as MappingCollection<IMetadataSource, MetadataSourceViewModel, ReadOnlyObservableCollection<IMetadataSource>>)?.Dispose();
            (Content as MappingCollection<IContentSource, ContentSourceViewModel, ReadOnlyObservableCollection<IContentSource>>)?.Dispose();
            base.Dispose(disposing);
        }
    }
}
