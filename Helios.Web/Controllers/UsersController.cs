using System.Web.Mvc;
using Helios.Web.Infrastructure;
using Helios.Web.Models;

namespace Helios.Web.Controllers {
    public class UsersController : ApplicationController {
        IFormsAuthentication _formsAuthentication;

        public UsersController(IUnitOfWork unitOfWork, IFormsAuthentication formsAuthentication)
            : base(unitOfWork) {
            
            this._formsAuthentication = formsAuthentication;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public ActionResult New() {
            return View();
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult Create(SignUpViewModel signUpViewModel) {
            if (this.ModelState.IsValid) {
                var newUser = new User() { UserName = signUpViewModel.UserName, RequiredToChangePassword = true };
                newUser.SetPassword(signUpViewModel.Password);
                Users.Add(newUser);
                
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }

            return View("New");
        }
    }
}
