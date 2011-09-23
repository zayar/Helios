using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Helios.Web.Controllers;
using Helios.Web.Infrastructure;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests.Controllers {
    public class UsersControllerTest {
        [Fact]
        public void Test_SignUp_should_route_to_users_new() {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            RouteTestHelper.AssertRoute(routes, "~/SignUp", new { controller = "users", action = "new" }, httpMethod: "GET");
        }

        [Fact]
        public void Test_users_new_should_return_ViewResult() {
            using (var context = new HeliosDbContext()) {
                var mockFormsAuth = new Mock<IFormsAuthentication>();
                var controller = new UsersController(context, mockFormsAuth.Object);
                var result = controller.New();

                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public void Test_POST_SignUp_should_route_to_users_create() {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            RouteTestHelper.AssertRoute(routes, "~/SignUp", new { controller = "users", action = "create" }, httpMethod: "POST");
        }

        [Fact]
        [InitDatabase]
        public void Test_POST_SignUp_should_redirect_to_home_index() {
            var validSignUpModel = new SignUpViewModel() {
                    UserName = "username",
                    Password = "secret",
                    ConfirmPassword = "secret"
                };

            using (var context = new HeliosDbContext()) {
                var mockFormsAuth = new Mock<IFormsAuthentication>();                
                var controller = new UsersController(context, mockFormsAuth.Object);
                var result = controller.Create(validSignUpModel);

                Assert.IsType<RedirectToRouteResult>(result);

                var redirectResult = result as RedirectToRouteResult;
                Assert.Equal("Home", redirectResult.RouteValues["controller"] as string);
                Assert.Equal("Index", redirectResult.RouteValues["action"] as string);
            }
        }

        [Fact]
        [InitDatabase]
        public void Test_POST_SignUp_should_create_new_user_with_RequiredToChangePassword() {
            var validSignUpModel = new SignUpViewModel() {
                    UserName = "username",
                    Password = "secret",
                    ConfirmPassword = "secret"
                };

            using (var context = new HeliosDbContext()) {
                var mockFormsAuth = new Mock<IFormsAuthentication>();

                var controller = new UsersController(context, mockFormsAuth.Object);
                controller.Create(validSignUpModel);

                // need to commit since we used UnitOfWorkAttribute in production
                context.Commit(); 

                var dbUser = context.Users.FirstOrDefault(u => u.UserName == validSignUpModel.UserName);

                Assert.NotNull(dbUser);
                Assert.Equal(true, dbUser.RequiredToChangePassword);
            }
        }

        [Fact]
        [InitDatabase]
        public void Test_POST_SignUp_with_invalid_view_model_should_return_view_with_Errors() {
            var invalidSignUpModel = new SignUpViewModel() {
                UserName = "",
                Password = "secret",
                ConfirmPassword = "wrongSecrect"
            };

            using (var context = new HeliosDbContext()) {
                var mockFormsAuth = new Mock<IFormsAuthentication>();
                var isAuthenticated = false;
                mockFormsAuth
                    .Setup(f => f.SetAuthCookie(invalidSignUpModel.UserName, /* createPersistentCookie */ false))
                    .Callback(() => isAuthenticated = true);

                var controller = new UsersController(context, mockFormsAuth.Object);

                var validationResults = ModelTestHelper.ValidateModel<SignUpViewModel>(invalidSignUpModel);
                foreach (var validationResult in validationResults) {
                    controller.ModelState.AddModelError("*", validationResult.ErrorMessage);
                }

                var result = controller.Create(invalidSignUpModel);

                Assert.IsType<ViewResult>(result);

                var viewResult = result as ViewResult;
                Assert.Equal("New", viewResult.ViewName);
                Assert.Equal(false, controller.ModelState.IsValid);
                Assert.Equal(false, isAuthenticated);
            }
        }
    }
}
