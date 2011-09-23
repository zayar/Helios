
namespace Helios.Web.Infrastructure {
    public interface IFormsAuthentication {
        void SetAuthCookie(string userName, bool createPersistentCookie);
        void SignOut();
    }
}
