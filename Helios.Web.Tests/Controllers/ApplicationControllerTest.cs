using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Moq;
using Helios.Web.Controllers;
using Helios.Web.Infrastructure;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests.Controllers {
    public class ApplicationControllerTest {
        [Fact]
        public void Test_all_controllers_should_inherits_from_ApplicationController() {
            var controllerTypes = from t in typeof(ApplicationController).Assembly.GetTypes()
                                  where t.IsSubclassOf(typeof(Controller))
                                  && !t.IsAbstract
                                  select t;

            foreach (var controllerType in controllerTypes) {
                Assert.Equal(true, controllerType.IsSubclassOf(typeof(ApplicationController)));
            }
        }

        private class StubController : ApplicationController {
            public StubController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

            public void CallOnAuthorization(AuthorizationContext filterContext) {
                this.OnAuthorization(filterContext);
            }            
        }

        [Fact]
        [InitDatabase]
        public void Test_should_set_UserPrinciple_if_User_is_authenticated() {
            var mockHttpContext = new Mock<HttpContextBase>();
            var genericPrincipal = new GenericPrincipal(new GenericIdentity("username"), new string[]{});
            mockHttpContext.SetupProperty(c => c.User, genericPrincipal);

            var controllerContext = new ControllerContext() {
                HttpContext = mockHttpContext.Object
            };

            using (var dbContext = new HeliosDbContext()) {
                var dbUser = new User() { 
                    UserName = "username",
                    IsAdmin = true
                };
                dbUser.SetPassword("secret");
                dbContext.Users.Add(dbUser);
                dbContext.Commit();

                var stubController = new StubController(dbContext) {
                    ControllerContext = controllerContext
                };

                stubController.CallOnAuthorization(new AuthorizationContext() { HttpContext = controllerContext.HttpContext });

                Assert.Equal(true, stubController.User is UserPrincipal);
                Assert.Equal("username", stubController.User.Identity.Name);
                Assert.Equal(true, stubController.User.IsInRole("admin"));
            }
        }

        [Fact]
        public void Test_should_not_set_UserPrinciple_if_User_is_not_authenticated() {
            var mockHttpContext = new Mock<HttpContextBase>();
            var genericPrincipal = new GenericPrincipal(new GenericIdentity("") /* empty user name means un-authenticated*/, new string[] { });            
            mockHttpContext.SetupProperty(c => c.User, genericPrincipal);

            var controllerContext = new ControllerContext() {
                HttpContext = mockHttpContext.Object
            };

            var stubController = new StubController(new HeliosDbContext()) {
                ControllerContext = controllerContext
            };

            stubController.CallOnAuthorization(new AuthorizationContext() { HttpContext = controllerContext.HttpContext });

            Assert.Equal(false, stubController.User is UserPrincipal);
            Assert.Equal(false, stubController.User.Identity.IsAuthenticated);
            Assert.Equal(false, stubController.User.IsInRole("admin"));
        }
    }
}
