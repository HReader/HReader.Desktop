using System.ComponentModel;
using System.Windows;
using Autofac;
using HReader.Core.Sources;
using HReader.Core.Storage;
using HReader.Utility;
using HReader.Wpf.Shortcuts;

namespace HReader.AutoModules
{
    internal class Default : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new SynchronizeInvokeDispatcher(Application.Current.Dispatcher))
                   .As<ISynchronizeInvoke>()
                   .InstancePerLifetimeScope();

            // instances generated from configuration
            builder.Register(context =>
                   {
                       var repo = new MetadataRepository(Files.MetadataRepository);
                       repo.InitializeAsync().GetAwaiter().GetResult();
                       return repo;
                   })
                   .As<IMetadataRepository>()
                   .InstancePerLifetimeScope();

            // business domain classes
            builder.Register(context => new DefaultSourceManager(
                        Directories.Sources,
                        Directories.NativateData,
                        context.Resolve<ISynchronizeInvoke>()
                   ))
                   .As<ISourceManager>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<ShortcutManager>()
                   .AsSelf()
                   .InstancePerLifetimeScope();
        }
    }
}
