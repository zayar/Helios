using System;
using System.Security.Principal;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests.Models {
    public class UserPrincipalTest {
        [Fact]
        public void Test_should_throws_ArgumentNullException_if_IPrincipal_or_user_is_null() {
            var exception = Assert.Throws<ArgumentNullException>(() => {
                new UserPrincipal(null, new User());
            });

            Assert.Equal("principal", exception.ParamName);

            exception = Assert.Throws<ArgumentNullException>(() => {
                var genericPrincipal = new GenericPrincipal(new GenericIdentity("username"), roles: new string[] { });
                new UserPrincipal(genericPrincipal, user: null);
            });

            Assert.Equal("user", exception.ParamName);
        }

        [Fact]
        public void Test_Identity_should_be_the_same_as_from_provided_IPrincipal() {
            var identity = new GenericIdentity("username");
            var userPrincipal = new UserPrincipal(new GenericPrincipal(identity, roles: new string[] { }), new User());

            Assert.Equal(identity, userPrincipal.Identity);
        }

        [Fact]
        public void Test_Admin_should_have_admin_role() {
            var genericPrincipal = new GenericPrincipal(new GenericIdentity("username"), roles: new string[] { });
            var admin = new User() { IsAdmin = true };
            var userPrincipal = new UserPrincipal(genericPrincipal, admin);

            Assert.Equal(true, userPrincipal.IsInRole("admin"));
            Assert.Equal(true, userPrincipal.IsInRole("Admin"));
        }

        [Fact]
        public void Test_NonAdmin_should_not_have_admin_role() {
            var genericPrincipal = new GenericPrincipal(new GenericIdentity("username"), roles: new string[] { });
            var admin = new User() { IsAdmin = false };
            var userPrincipal = new UserPrincipal(genericPrincipal, admin);

            Assert.Equal(false, userPrincipal.IsInRole(""));
            Assert.Equal(false, userPrincipal.IsInRole(null));
            Assert.Equal(false, userPrincipal.IsInRole("admin"));
            Assert.Equal(false, userPrincipal.IsInRole("Admin"));
        }
    }
}
