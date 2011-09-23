using System.Linq;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests.Models {
    public class SignUpViewModelTest {
        [Fact]
        public void Test_UserName_cannot_be_null_or_empty() {
            var userNames = new string[] { null, "" };
            foreach (var userName in userNames) {
                var viewModel = CreateSignUpViewModel(userName);
                var validationResults = ModelTestHelper.ValidateModel(viewModel);
                Assert.Equal(1, validationResults.Count());
                Assert.True(validationResults.All(vr => vr.MemberNames.First() == "UserName"));
            }
        }

        [Fact]
        public void Test_UserName_cannot_contains_whitespace_and_special_characters() {
            var userNames = new string[] { "  user name", "user23!name@", "+0fl)" };
            foreach (var userName in userNames) {
                var viewModel = CreateSignUpViewModel(userName);
                var validationResults = ModelTestHelper.ValidateModel(viewModel);
                Assert.Equal(1, validationResults.Count());
                Assert.True(validationResults.All(vr => vr.MemberNames.First() == "UserName"));
            }
        }

        [Fact]
        public void Test_UserName_should_be_valid_for_alphanumeric_underscore_hyphen() {
            var userNames = new string[] { "username", "userName", "user_name", "user-Name" };
            foreach (var userName in userNames) {
                var viewModel = CreateSignUpViewModel(userName);
                Assert.Empty(ModelTestHelper.ValidateModel(viewModel));
            }
        }

        [Fact]
        public void Test_Password_cannot_be_null_or_empty() {
            var passwords = new string[] { null, "" };
            foreach (var password in passwords) {
                var viewModel = CreateSignUpViewModel(password: password);
                var validationResults = ModelTestHelper.ValidateModel(viewModel);                
                Assert.True(validationResults.Any(vr => vr.MemberNames.First() == "Password"));
            }
        }

        [Fact]
        public void Test_ConfirmPassword_must_match_with_Password() {
            var viewModel = CreateSignUpViewModel(password: "password", confirmPassword: "differentPassword");
            var validationResults = ModelTestHelper.ValidateModel(viewModel);
            Assert.Equal(1, validationResults.Count());
        }

        public static SignUpViewModel CreateSignUpViewModel(string userName = "username", string password = "mySecret", string confirmPassword = "mySecret") {
            return new SignUpViewModel() { 
                UserName = userName,
                Password = password,
                ConfirmPassword = confirmPassword
            };
        }
    }
}
