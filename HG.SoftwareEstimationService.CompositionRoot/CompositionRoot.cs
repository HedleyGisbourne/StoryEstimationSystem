using Castle.MicroKernel.Registration;
using Castle.Windsor;
using HG.SoftwareEstimationService.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.SoftwareEstimationService.CompositionRoot
{
    public static class CompositionRoot
    {
        private static readonly WindsorContainer Container;

        static CompositionRoot()
        {
            Container = new WindsorContainer();

            Container.Register(
                Classes.FromAssembly(typeof(ConfigurationService).Assembly)
                    .Where(type => type.IsPublic)
                    .WithService.AllInterfaces()
                    .LifestyleTransient());

            Container.Register(
                Classes.FromAssembly(typeof(Repository.ConnectionStringManager).Assembly)
                    .Where(type => type.IsPublic)
                    .WithService.AllInterfaces()
                    .LifestyleTransient());
        }

        public static void InstallAsembly(IWindsorInstaller installer)
        {
            Container.Install(installer);
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return Container.ResolveAll<T>();
        }

        public static IEnumerable<object> ResolveAll(Type type)
        {
            return Container.ResolveAll(type).Cast<object>();
        }

        public static void Release(Object obj)
        {
            Container.Release(obj);
        }
    }
}