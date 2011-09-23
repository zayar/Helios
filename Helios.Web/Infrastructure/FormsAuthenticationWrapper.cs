using System.Web.Security;

namespace Helios.Web.Infrastructure {
    public class FormsAuthenticationWrapper : IFormsAuthentication {
        public void SetAuthCookie(string userName, bool createPersistentCookie) {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut() {
            FormsAuthentication.SignOut();
        }
    }
}