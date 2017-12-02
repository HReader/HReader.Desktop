using System;
using System.Collections.Generic;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using HReader.AutoModules;
using HReader.ViewModels;
using IContainer = Autofac.IContainer;

namespace HReader
{
    internal class Bootstrap : BootstrapperBase
    {
        public Bootstrap()
        {
            Initialize();
        }

        private IContainer container;

        /// <inheritdoc />
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
        
        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<Mvvm>()
                   .RegisterModule<Config>() 
                   .RegisterModule<Default>();

            container = builder.Build();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (container.IsRegistered(serviceType))
                {
                    return container.Resolve(serviceType);
                }
            }
            else if (container.IsRegisteredWithKey(key, serviceType))
            {
                return container.ResolveKeyed(key, serviceType);
            }
            throw new Exception($"Could not locate any instances of contract {key ?? serviceType.Name}.");
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            container.InjectProperties(instance);
        }
    }
}
