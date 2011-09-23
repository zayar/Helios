using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using Helios.Web.Models;
using Helios.Web.Ninject;

namespace Helios.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            MapSignInRoute(routes);
            MapSignUpRoute(routes);
            MapSignOutRoute(routes);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        private static void MapSignOutRoute(RouteCollection routes) {

            routes.MapRoute(
                "SignOut",
                "SignOut",
                new { controller = "Sessions", action = "Destroy" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );
        }

        private static void MapSignUpRoute(RouteCollection routes) {
            routes.MapRoute(
                "NewUser",
                "SignUp",
                new { controller = "Users", action = "New" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            routes.MapRoute(
                "CreateUser",
                "SignUp",
                new { controller = "Users", action = "Create" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );
        }

        private static void MapSignInRoute(RouteCollection routes) {
            routes.MapRoute(
                "NewSession",
                "SignIn",
                new { controller = "Sessions", action = "New" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );

            routes.MapRoute(
                "CreateSession",
                "SignIn",
                new { controller = "Sessions", action = "Create" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // initialize dependency injection.
            NinjectBootstrapper.Init();

            Database.SetInitializer(new DatabaseInitializer());
        }
    }
}