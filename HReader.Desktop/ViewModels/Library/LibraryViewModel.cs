using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using HReader.Base.Data;
using HReader.Core.Pagination;
using HReader.Core.Storage;
using HReader.ViewModels.Tabs;
using PropertyChanged;

namespace HReader.ViewModels.Library
{
    internal sealed class LibraryViewModel : TabViewModelBase
    {
        private readonly IMetadataRepository repository;
        private readonly LibraryPageItemViewModel.Factory itemFactory;

        private readonly IPagination<IPage<IMetadata>> pagination;
        
        public LibraryViewModel(IMetadataRepository repository, LibraryPageItemViewModel.Factory itemFactory) : base("library")
        {
            this.repository = repository;
            this.itemFactory = itemFactory;
            DisplayName = "Library";
            //TODO: find a way to fix this getawaiter
            pagination = repository.GetAllEntriesAsync().GetAwaiter().GetResult();
            PageSelection = Enumerable.Range(1, PageCount).ToList();
        }

        /// <inheritdoc />
        protected override void OnActivate()
        {
            if(Page == null) Page = new LibraryPageViewModel(pagination.Current, itemFactory, IoC.Get<MainViewModel>());
            base.OnActivate();
        }

        public int SelectedPageSelection
        {
            set => SelectIndex(value - 1);
        }

        public IReadOnlyList<int> PageSelection { get; private set; }

        public LibraryPageViewModel Page { get; private set; }

        [DependsOn(nameof(Page))]
        public int PageIndex => pagination.CurrentIndex + 1;
        public int PageCount => pagination.Count;

        private async void SelectIndex(int index)
        {
            if (index == pagination.CurrentIndex) return;
            await pagination.NavigateToAsync(index);
            Page = new LibraryPageViewModel(pagination.Current, itemFactory, IoC.Get<MainViewModel>());
        }

        public Task SelectNextAsync()
        {
            return pagination?.NavigateToNextAsync();
        }

        public Task SelectPreviousAsync()
        {
            return pagination?.NavigateToPreviousAsync();
        }

        public Task SelectFirstAsync()
        {
            return pagination?.NavigateToStartAsync();
        }

        public Task SelectLastAsync()
        {
            return pagination?.NavigateToEndAsync();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            pagination?.Dispose();
            base.Dispose(disposing);
        }
    }
}
