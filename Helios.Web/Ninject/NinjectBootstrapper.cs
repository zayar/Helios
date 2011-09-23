using System.Web.Mvc;
using Ninject;
using Helios.Web.Infrastructure;
using Helios.Web.Models;

namespace Helios.Web.Ninject {
    public static class NinjectBootstrapper {        
        public static void Init() {
            IKernel kernel = new StandardKernel();

            SetupMapping(kernel);

            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }

        private static void SetupMapping(IKernel kernel) {
            kernel.Bind<IFilterProvider>().To<NinjectFilterAttributeFilterProvider>().InSingletonScope();

            kernel.Bind<IUnitOfWork>().To<HeliosDbContext>().InRequestScope();

            kernel.Bind<IFormsAuthentication>().To<FormsAuthenticationWrapper>().InSingletonScope();
        }
    }
}