using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Helios.Web.Controllers;
using Helios.Web.Infrastructure;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests.Controllers {
    public class SessionsControllerTest {
        [Fact]
        public void Test_SignIn_should_route_to_sessions_new() {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            RouteTestHelper.AssertRoute(routes, "~/SignIn", new { controller = "sessions", action = "new" }, httpMethod: "GET");
        }

        [Fact]
        public void Test_SignOut_should_route_to_sessions_destroy() {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            RouteTestHelper.AssertRoute(routes, "~/SignOut", new { controller = "sessions", action = "destroy" }, httpMethod: "POST");
        }

        [Fact]
        public void Test_sessions_new_should_return_ViewResult() {
            using (var context = new HeliosDbContext()) {
                var controller = new SessionsController(context, new Mock<IFormsAuthentication>().Object);
                var result = controller.New();

                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public void Test_sessions_edit_should_return_ViewResult() {
            using (var context = new HeliosDbContext()) {
                var controller = new SessionsController(context, new Mock<IFormsAuthentication>().Object);
                var result = controller.Edit();

                Assert.IsType<ViewResult>(result);
            }
        }

        [Fact]
        public void Test_POST_SignIn_should_route_to_sessions_create() {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            RouteTestHelper.AssertRoute(routes, "~/SignIn", new { controller = "sessions", action = "create" }, httpMethod: "POST");
        }

        [Fact]
        [InitDatabase]
        public void Test_sessions_create_should_sign_in_and_redirect_to_home_index() {
            var validSignInModel = new SignInViewModel() {
                UserName = "username",
                Password = "secret",
                RememberMe = true
            };

            using (var context = new HeliosDbContext()) {
                SaveUserInDb(context, validSignInModel.UserName, validSignInModel.Password);

                var mockFormsAuth = new Mock<IFormsAuthentication>();
                mockFormsAuth
                    .Setup(f => f.SetAuthCookie(validSignInModel.UserName, /* createPersistentCookie */ validSignInModel.RememberMe))
                    .Verifiable();

                var controller = new SessionsController(context, mockFormsAuth.Object);
                var result = controller.Create(validSignInModel);

                Assert.IsType<RedirectToRouteResult>(result);

                var redirectResult = result as RedirectToRouteResult;
                Assert.Equal("Home", redirectResult.RouteValues["controller"] as string);
                Assert.Equal("Index", redirectResult.RouteValues["action"] as string);

                mockFormsAuth.Verify();
            }
        }

        [Fact]
        [InitDatabase]
        public void Test_sessions_create_should_sign_in_and_redirect_to_sessions_reset_for_first_time_user() {
            var validSignInModel = new SignInViewModel() {
                UserName = "username",
                Password = "secret",
                RememberMe = true
            };

            using (var context = new HeliosDbContext()) {
                SaveUserInDb(context, validSignInModel.UserName, validSignInModel.Password, requiredToChangePassword: true);

                var mockFormsAuth = new Mock<IFormsAuthentication>();
                mockFormsAuth
                    .Setup(f => f.SetAuthCookie(validSignInModel.UserName, /* createPersistentCookie */ validSignInModel.RememberMe))
                    .Verifiable();

                var controller = new SessionsController(context, mockFormsAuth.Object);
                var result = controller.Create(validSignInModel);

                Assert.IsType<RedirectToRouteResult>(result);

                var redirectResult = result as RedirectToRouteResult;
                Assert.Equal("Sessions", redirectResult.RouteValues["controller"] as string);
                Assert.Equal("Edit", redirectResult.RouteValues["action"] as string);

                mockFormsAuth.Verify();
            }
        }

        [Fact]
        [InitDatabase]
        public void Test_sessions_create_with_invalid_username_should_return_view_with_error() {
            var invalidSignInModel = new SignInViewModel() {
                UserName = "",
                Password = "secret"
            };

            using (var context = new HeliosDbContext()) {
                var mockFormsAuth = new Mock<IFormsAuthentication>();
                var isAuthenticated = false;
                mockFormsAuth
                    .Setup(f => f.SetAuthCookie(invalidSignInModel.UserName, /* createPersistentCookie */ false))
                    .Callback(() => isAuthenticated = true);

                var controller = new SessionsController(context, mockFormsAuth.Object);

                var validationResults = ModelTestHelper.ValidateModel<SignInViewModel>(invalidSignInModel);
                foreach (var validationResult in validationResults) {
                    controller.ModelState.AddModelError("*", validationResult.ErrorMessage);
                }

                var result = controller.Create(invalidSignInModel);

                Assert.IsType<ViewResult>(result);

                var viewResult = result as ViewResult;
                Assert.Equal("New", viewResult.ViewName);
                Assert.Equal(false, controller.ModelState.IsValid);
                Assert.Equal(false, isAuthenticated);
            }
        }

        [Fact]
        [InitDatabase]
        public void Test_sessions_create_with_wrong_username_should_return_view_with_error() {
            var invalidSignInModel = new SignInViewModel() {
                UserName = "wrong_user",
                Password = "secret"
            };
            Sessions_create_AssertReturnViewWithError(invalidSignInModel);
        }

        [Fact]
        [InitDatabase]
        public void Test_sessions_create_with_wrong_password_should_return_view_with_error() {
            var invalidSignInModel = new SignInViewModel() {
                UserName = "username",
                Password = "wrong_password"
            };

            Sessions_create_AssertReturnViewWithError(invalidSignInModel);
        }

        private static void Sessions_create_AssertReturnViewWithError(SignInViewModel invalidSignInModel) {
            using (var context = new HeliosDbContext()) {
                SaveUserInDb(context, userName: "username", password: "secret");

                var mockFormsAuth = new Mock<IFormsAuthentication>();
                var isAuthenticated = false;
                mockFormsAuth
                    .Setup(f => f.SetAuthCookie(invalidSignInModel.UserName, /* createPersistentCookie */ false))
                    .Callback(() => isAuthenticated = true);

                var controller = new SessionsController(context, mockFormsAuth.Object);

                var result = controller.Create(invalidSignInModel);

                Assert.IsType<ViewResult>(result);

                var viewResult = result as ViewResult;
                Assert.Equal("New", viewResult.ViewName);
                Assert.Equal(false, controller.ModelState.IsValid);
                Assert.Equal(false, isAuthenticated);
            }
        }

        [Fact]
        public void Test_sessions_destroy_should_sign_out_and_redirect_to_home_index() {
            var mockFormsAuth = new Mock<IFormsAuthentication>();
            mockFormsAuth.Setup(f => f.SignOut()).Verifiable();

            using (var context = new HeliosDbContext()) {
                var controller = new SessionsController(context, mockFormsAuth.Object);

                var result = controller.Destroy();

                Assert.IsType<RedirectToRouteResult>(result);

                var redirectResult = result as RedirectToRouteResult;
                Assert.Equal("Home", redirectResult.RouteValues["controller"] as string);
                Assert.Equal("Index", redirectResult.RouteValues["action"] as string);

                mockFormsAuth.Verify();
            }
        }

        [Fact]
        [InitDatabase]
        public void Test_POST_sessions_Edit_should_update_password_and_set_false_to_RequiredToChangePassword() {
            var changePasswordViewModel = new ChangePasswordViewModel() {
                Password = "secret",
                ConfirmPassword = "secret"
            };

            var userName = "michael";
            var controllerContext = CreateControllerContext(userName);

            using (var context = new HeliosDbContext()) {
                SaveUserInDb(context, userName, password: "old_password", requiredToChangePassword: true);

                var mockFormsAuth = new Mock<IFormsAuthentication>();

                var controller = new SessionsController(context, mockFormsAuth.Object) {
                    ControllerContext = controllerContext
                };

                controller.Edit(changePasswordViewModel);

                // need to commit since we used UnitOfWorkAttribute in production
                context.Commit();

                var dbUser = context.Users.FirstOrDefault(u => u.UserName == userName);

                Assert.NotNull(dbUser);
                Assert.Equal(true, dbUser.VerifyPassword(changePasswordViewModel.Password));
                Assert.Equal(false, dbUser.RequiredToChangePassword);
            }
        }       

        [Fact]
        [InitDatabase]
        public void Test_POST_sessions_Edit_should_rediret_to_home_index() {
            var changePasswordViewModel = new ChangePasswordViewModel() {
                Password = "secret",
                ConfirmPassword = "secret"
            };

            var userName = "michael";
            var controllerContext = CreateControllerContext(userName);

            using (var context = new HeliosDbContext()) {
                SaveUserInDb(context, userName, password: "old_password");

                var mockFormsAuth = new Mock<IFormsAuthentication>();

                var controller = new SessionsController(context, mockFormsAuth.Object) {
                    ControllerContext = controllerContext
                };

                var result = controller.Edit(changePasswordViewModel);

                // need to commit since we used UnitOfWorkAttribute in production
                context.Commit();

                Assert.IsType<RedirectToRouteResult>(result);
                var redirectResult = result as RedirectToRouteResult;
                Assert.Equal("Home", (string)redirectResult.RouteValues["controller"]);
                Assert.Equal("Index", (string)redirectResult.RouteValues["action"]);
            }
        }

        [Fact]
        [InitDatabase]
        public void Test_POST_sessions_Edit_should_not_update_password_when_provide_not_match_password() {
            var changePasswordViewModel = new ChangePasswordViewModel() {
                Password = "secret",
                ConfirmPassword = "not_match_password"
            };

            var userName = "michael";
            var controllerContext = CreateControllerContext(userName);

            using (var context = new HeliosDbContext()) {
                SaveUserInDb(context, userName, password: "old_password");

                var controller = new SessionsController(context, new Mock<IFormsAuthentication>().Object) {
                    ControllerContext = controllerContext
                };

                var validationResults = ModelTestHelper.ValidateModel<ChangePasswordViewModel>(changePasswordViewModel);
                foreach (var validationResult in validationResults) {
                    controller.ModelState.AddModelError("*", validationResult.ErrorMessage);
                }

                var result = controller.Edit(changePasswordViewModel);

                // need to commit since we used UnitOfWorkAttribute in production
                context.Commit();

                var dbUser = context.Users.FirstOrDefault(u => u.UserName == userName);

                Assert.NotNull(dbUser);
                Assert.Equal(true, dbUser.VerifyPassword("old_password"));
                Assert.IsType<ViewResult>(result);
                Assert.Equal(false, controller.ModelState.IsValid);
            }            
        }

        private static ControllerContext CreateControllerContext(string userName) {
            var mockHttpContext = new Mock<HttpContextBase>();
            var genericPrincipal = new GenericPrincipal(new GenericIdentity(userName), new string[] { });
            mockHttpContext.SetupProperty(c => c.User, genericPrincipal);

            var controllerContext = new ControllerContext() {
                HttpContext = mockHttpContext.Object
            };
            return controllerContext;
        }

        private static void SaveUserInDb(HeliosDbContext context, string userName, string password, bool requiredToChangePassword = false) {
            var dbUser = new User() { UserName = userName, RequiredToChangePassword = requiredToChangePassword };
            dbUser.SetPassword(password);
            context.Users.Add(dbUser);
            context.Commit();
        }
    }
}
