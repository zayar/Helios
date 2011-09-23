using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Helios.Web.Infrastructure;
using Helios.Web.Models;

namespace Helios.Web.Controllers {
    [UnitOfWork]
    public abstract class ApplicationController : Controller {
        private readonly HeliosDbContext _dbContext;

        public ApplicationController(IUnitOfWork unitOfWork) {
            _dbContext = (HeliosDbContext)unitOfWork;
        }

        public DbSet<User> Users {
            get {
                return _dbContext.Users;
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext) {
            var currentUser = filterContext.HttpContext.User;

            if (currentUser.Identity.IsAuthenticated) {
                var dbUser = Users.FirstOrDefault(u => u.UserName == filterContext.HttpContext.User.Identity.Name);
                filterContext.HttpContext.User = new UserPrincipal(filterContext.HttpContext.User, dbUser);
            }

            base.OnAuthorization(filterContext);
        }
    }
}
