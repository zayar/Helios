using System.Linq;
using Helios.Web.Models;
using Xunit;

namespace Helios.Web.Tests.Models {
    public class ChangePasswordViewModelTest {
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

        public static ChangePasswordViewModel CreateSignUpViewModel(string password = "mySecret", string confirmPassword = "mySecret") {
            return new ChangePasswordViewModel() {
                Password = password,
                ConfirmPassword = confirmPassword
            };
        }
    }
}
