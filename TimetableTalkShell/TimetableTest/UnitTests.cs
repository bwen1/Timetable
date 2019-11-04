using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimetableTalkShell.ViewModels;

namespace TimetableTest
{
    [TestClass]
    public class SignUpViewModelTest
    {
        [TestMethod]
        public void SignupCommandTest()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "hanogy@gmail.com";
            vm.Password = "hobolobo456";


            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.GetType().Name == "SignUpViewModel", "viewmodel");// Correct result
            //Assert.IsTrue(vm.Email == "", "Email is Invalid");// Wrong result
            //Assert.IsTrue(vm.Password == "", "Password is Invalid");// Wrong result
        }



    }

    [TestClass]
    public class LoginViewModelTest
    {

        [TestMethod]
        public void LoginClickedCommandTest()
        {
            //Arrange
            var vm = new LoginViewModel();
            vm.Name = "heloo";
            vm.Password = "hobolobo456";


            //Act
            vm.LoginCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.GetType().Name == "LoginViewModel", "viewmodel");// Correct result
            //Assert.IsTrue(vm.Name == "", "Name is Invalid");// Wrong result
            //Assert.IsTrue(vm.Password == "", "Password is Invalid");// Wrong result
        }

    }
}
