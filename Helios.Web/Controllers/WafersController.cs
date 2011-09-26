using System;
using System.IO;
using System.Web.Mvc;
using Helios.Web.Infrastructure;

namespace Helios.Web.Controllers {
    public class WafersController : ApplicationController {

        public WafersController(IUnitOfWork unitOfWork)
            : base(unitOfWork) {
        }

        [HttpGet]
        public ActionResult Marker() {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage() {
            SaveInputStreamAsPNG();

            return new EmptyResult();
        }

        private static string UploadsPath {
            get {
                return AppDomain.CurrentDomain.BaseDirectory + "Uploads/";
            }
        }

        private void SaveInputStreamAsPNG() {
            var fileName = Guid.NewGuid().ToString("N") + ".png";

            using (var stream = System.IO.File.OpenWrite(Path.Combine(UploadsPath, fileName))) {
                Request.InputStream.CopyTo(stream);
            }
        }
    }
}
