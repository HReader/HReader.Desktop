using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace HReader.ViewModels.Tabs
{
    internal class TabViewModelBase : Screen, IDisposable
    {
        protected TabViewModelBase(string id)
        {
            Id = id;
        }

        public string Id { get; }

        protected virtual void Dispose(bool disposing) { }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
