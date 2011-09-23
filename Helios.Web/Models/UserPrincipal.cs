using System;
using System.Linq;
using System.Security.Principal;

namespace Helios.Web.Models {
    public class UserPrincipal : IPrincipal {
        IPrincipal _principal;
        User _user;
        string[] _roles;

        public UserPrincipal(IPrincipal principal, User user) {
            if (principal == null) {
                throw new ArgumentNullException("principal");
            }

            if (user == null) {
                throw new ArgumentNullException("user");
            }

            this._principal = principal;
            this._user = user;
            this._roles = new string[] { "admin" };
        }

        public IIdentity Identity {
            get { return this._principal.Identity; }
        }

        public bool IsInRole(string role) {
            return this._roles.Contains((role ?? "").ToLowerInvariant()) && _user.IsAdmin;
        }
    }
}