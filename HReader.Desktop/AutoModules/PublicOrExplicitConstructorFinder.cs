using System;
using System.Linq;
using System.Reflection;
using Autofac.Core.Activators.Reflection;

namespace HReader.AutoModules
{
    [AttributeUsage(AttributeTargets.Constructor)]
    internal class InjectableAttribute : Attribute { }

    internal class PublicOrExplicitConstructorFinder : IConstructorFinder
    {
        /// <inheritdoc />
        public ConstructorInfo[] FindConstructors(Type targetType)
        {
            return targetType.GetTypeInfo()
                             .DeclaredConstructors
                             .Where(c => c.IsPublic || c.GetCustomAttribute<InjectableAttribute>() != null)
                             .ToArray();
        }
    }
}