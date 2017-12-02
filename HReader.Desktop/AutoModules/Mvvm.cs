using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Caliburn.Micro;
using HReader.Utility;
using HReader.ViewModels;
using HReader.ViewModels.Library;
using HReader.ViewModels.Tabs;
using Module = Autofac.Module;

namespace HReader.AutoModules
{
    internal class Mvvm : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            //views
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType()))
                   .Where(IsView)
                   .AsSelf()
                   .FindConstructorsWith(new PublicOrExplicitConstructorFinder())
                   .InstancePerDependency();

            //view models
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType()))
                   .Where(IsViewModel)
                   .AsSelf()
                   .InstancePerDependency();

            //view model factories
            builder.RegisterType<ReaderViewModel.Factory>()
                   .AsSelf()
                   .InstancePerLifetimeScope();

            builder.RegisterType<LibraryPageItemViewModel.Factory>()
                   .AsSelf()
                   .InstancePerLifetimeScope();

            //conducting view model
            builder.RegisterType<MainViewModel>()
                   .AsSelf()
                   .InstancePerLifetimeScope();

            // window manager
            builder.RegisterType<MaterialWindowManager>()
                   .As<IWindowManager>()
                   .InstancePerLifetimeScope();

            // event aggregator
            builder.RegisterType<EventAggregator>()
                   .As<IEventAggregator>()
                   .InstancePerLifetimeScope();
        }

        private static bool IsView(Type type)
        {
            if (type.Namespace is null) return false;
            if (!type.Name.EndsWith("View")) return false;

            return type.Namespace.EndsWith("Views")
                   || type.Namespace.Contains(".Views.")
                   && !ExcludedViews.Contains(type.Name); // allows for sub directories in Views directory
        }

        private static bool IsViewModel(Type type)
        {
            if (type.Namespace is null) return false;
            if (!type.Name.EndsWith("ViewModel")) return false;

            return type.Namespace.EndsWith("ViewModels")
                   || type.Namespace.Contains(".ViewModels.")
                   && !ExcludedViewModels.Contains(type.Name);
        }

        private static readonly string[] ExcludedViews = { };

        private static readonly string[] ExcludedViewModels =
        {
            nameof(ReaderViewModel),
            nameof(MainViewModel),
        };
    }
}
