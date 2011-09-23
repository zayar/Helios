using System.Linq;
using System.Web.Mvc;
using Helios.Web.Infrastructure;
using Helios.Web.Models;

namespace Helios.Web.Controllers {
    public class SessionsController : ApplicationController {
        IFormsAuthentication _formsAuthentication;

        public SessionsController(IUnitOfWork unitOfWork, IFormsAuthentication formsAuthentication)
            : base(unitOfWork) {

            this._formsAuthentication = formsAuthentication;
        }

        [HttpGet]
        public ActionResult New() {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SignInViewModel signInModel) {
            if (!this.ModelState.IsValid) {
                return View("New");
            }

            var dbUser = Users.FirstOrDefault(u => u.UserName.Equals(signInModel.UserName));
            if (dbUser == null || !dbUser.VerifyPassword(signInModel.Password)) {
                this.ModelState.AddModelError("*", "User Name or Password is wrong.");
                return View("New");
            }

            _formsAuthentication.SetAuthCookie(signInModel.UserName, createPersistentCookie: signInModel.RememberMe);
            var routeValues = dbUser.RequiredToChangePassword ? new { controller = "Sessions", action = "Edit" } : new { controller = "Home", action = "Index" };
            return RedirectToRoute(routeValues);
        }

        [HttpGet, Authorize]
        public ActionResult Edit() {
            return View();
        }

        [HttpPost, Authorize]
        public ActionResult Edit(ChangePasswordViewModel changePasswordViewModel) {
            if (this.ModelState.IsValid) {
                var dbUser = Users.FirstOrDefault(u => u.UserName.Equals(this.User.Identity.Name));
                dbUser.RequiredToChangePassword = false;
                dbUser.SetPassword(changePasswordViewModel.Password);

                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }

            return View();
        }

        [HttpPost, Authorize]
        public ActionResult Destroy() {
            _formsAuthentication.SignOut();

            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }
    }
}
