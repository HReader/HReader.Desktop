using System.Threading.Tasks;
using HReader.Core.Storage;
using Nito.AsyncEx;

namespace HReader.Utility
{
    internal class MetadataRepositoryFactory
    {
        public MetadataRepositoryFactory(MetadataRepository repository)
        {
            data = new AsyncLazy<IMetadataRepository>(async () =>
            {
                await repository.InitializeAsync();
                return repository;
            });
        }

        private readonly AsyncLazy<IMetadataRepository> data;

        public async Task<IMetadataRepository> GetInstance()
        {
            return await data.Task;
        }
    }
}
