using System.Linq;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests.Models {
    public class UserTest {
        [Fact]
        public void Test_UserName_cannot_be_null_or_empty() {
            var userNames = new string[] { null, "" };
            foreach (var userName in userNames) {
                var user = CreateUser(userName);
                var validationResults = ModelTestHelper.ValidateModel(user);
                Assert.Equal(1, validationResults.Count());
                Assert.True(validationResults.All(vr => vr.MemberNames.First() == "UserName"));
            }
        }

        [Fact]
        public void Test_UserName_cannot_contains_whitespace_and_special_characters() {
            var userNames = new string[] { "  user name", "user23!name@", "+0fl)" };
            foreach (var userName in userNames) {
                var user = CreateUser(userName);
                var validationResults = ModelTestHelper.ValidateModel(user);
                Assert.Equal(1, validationResults.Count());
                Assert.True(validationResults.All(vr => vr.MemberNames.First() == "UserName"));
            }
        }

        [Fact]
        public void Test_UserName_should_be_valid_for_alphanumeric_underscore_hyphen() {
            var userNames = new string[] { "username", "userName", "user_name", "user-Name" };
            foreach (var userName in userNames) {
                var user = CreateUser(userName);
                Assert.Empty(ModelTestHelper.ValidateModel(user));
            }
        }

        [Fact]
        public void Test_PasswordDigest_should_not_be_null_or_the_same_as_password() {            
            var passwords = new string[] { null, "", "myLittle_secret!!" };

            foreach (var password in passwords) {
                var user = CreateUser(password: password);
                Assert.NotNull(user.PasswordDigest);
                Assert.NotEqual("", user.PasswordDigest);
                Assert.NotEqual(password, user.PasswordDigest);
            }
        }

        [Fact]
        public void Test_Salt_should_not_be_null_or_the_same_as_password_or_PasswordDigest() {
            var passwords = new string[] { null, "", "myLittle_secret!!" };
            foreach (var password in passwords) {
                var user = CreateUser(password: password);

                Assert.NotNull(user.Salt);
                Assert.NotEqual("", user.Salt);
                Assert.NotEqual(password, user.Salt);
                Assert.NotEqual(user.PasswordDigest, user.Salt);
            }
        }

        [Fact]
        public void Test_VerifyPassword_should_return_true_for_correct_password() {
            var password = "myLittle_secret!!";
            var user = CreateUser(password: password);

            Assert.Equal(true, user.VerifyPassword(password));
        }

        [Fact]
        public void Test_VerifyPassword_should_return_false_for_wrong_password() {
            var password = "myLittle_secret!!";
            var user = CreateUser(password: password);

            Assert.Equal(false, user.VerifyPassword("wrong_password"));
        }

        private User CreateUser(string userName = "username", string password = "myLittle_secret!!") {
            var user = new User { UserName = userName };
            user.SetPassword(password);

            return user;
        }
    }
}
