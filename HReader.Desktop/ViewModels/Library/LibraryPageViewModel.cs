using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using HReader.Base.Data;
using HReader.Core.Pagination;
using HReader.ViewModels.Tabs;

namespace HReader.ViewModels.Library
{
    internal class LibraryPageViewModel : Screen
    {
        public LibraryPageViewModel(
            IPage<IMetadata> page,
            LibraryPageItemViewModel.Factory factory,
            Conductor<TabViewModelBase>.Collection.OneActive conductor)
        {
            if (page == null)
            {
                PageVisibility = Visibility.Collapsed;
                EmptyVisibility = Visibility.Visible;
                return;
            }

            PageVisibility = Visibility.Visible;
            EmptyVisibility = Visibility.Hidden;

            Page = page.Select(m => factory.Create(m, conductor)).ToImmutableList();
        }

        public IReadOnlyList<LibraryPageItemViewModel> Page { get; }

        public Visibility PageVisibility { get; }
        public Visibility EmptyVisibility { get; }
    }
}
