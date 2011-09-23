namespace Helios.Web.Infrastructure {

    using System.IO;
    using System.Web.Mvc;

    public class ExcelResult : FileStreamResult {
        public ExcelResult(Stream fileStream, string fileName)
            : base(fileStream, "application/vnd.ms-excel") {
            FileDownloadName = fileName;
        }
    }
}