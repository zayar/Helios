using System.Web.Mvc;
using Helios.Web.Infrastructure;

namespace Helios.Web.Controllers {
    public class HomeController : ApplicationController {

        public HomeController(IUnitOfWork unitOfWork)
            : base(unitOfWork) {
        }

        public ActionResult Index() {
            return View();
        }
    }
}
