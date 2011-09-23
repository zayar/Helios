using System.Linq;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests.Models {
    public class SignInViewModelTest {
        [Fact]
        public void Test_UserName_cannot_be_null_or_empty() {
            var userNames = new string[] { null, "" };
            foreach (var userName in userNames) {
                var viewModel = CreateSignInViewModel(userName);
                var validationResults = ModelTestHelper.ValidateModel(viewModel);
                Assert.Equal(1, validationResults.Count());
                Assert.True(validationResults.All(vr => vr.MemberNames.First() == "UserName"));
            }
        }

        [Fact]
        public void Test_Password_cannot_be_null_or_empty() {
            var passwords = new string[] { null, "" };
            foreach (var password in passwords) {
                var viewModel = CreateSignInViewModel(password: password);
                var validationResults = ModelTestHelper.ValidateModel(viewModel);
                Assert.True(validationResults.Any(vr => vr.MemberNames.First() == "Password"));
            }
        }

        public static SignInViewModel CreateSignInViewModel(string userName = "username", string password = "mySecret") {
            return new SignInViewModel() {
                UserName = userName,
                Password = password
            };
        }
    }
}
