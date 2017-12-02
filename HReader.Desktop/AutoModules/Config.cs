using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using HReader.Builtin.Native;
using HReader.Persistence;
using HReader.Wpf.Shortcuts;

namespace HReader.AutoModules
{
    internal class Config : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            //configuration and subclasses
            builder.Register(context => Configuration.Load())
                   .AsSelf()
                   .InstancePerLifetimeScope();

            builder.Register(context => context.Resolve<Configuration>().Sources)
                   .AsSelf()
                   .ExternallyOwned();

            builder.Register(context => context.Resolve<Configuration>().UserPreferences)
                   .AsSelf()
                   .ExternallyOwned();
            
            builder.Register(context => context.Resolve<Configuration>().UserPreferences)
                   .As<IShortcutPersistenceProvider>()
                   .ExternallyOwned();
        }
    }
}
