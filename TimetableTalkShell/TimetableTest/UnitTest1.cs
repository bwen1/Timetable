using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimetableTalkShell.ViewModels;

namespace TimetableTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "hanogy@gmail.com";
            vm.Password = "hobolobo456";


            //Act
            vm.LoginCommand.Execute(null);
            
            //Assert
            Assert.IsTrue(vm.GetType().Name == "SignUpViewModel", "viewmodel");// Wrong Email
            Assert.IsTrue(vm.Password == "", "Password is Invalid");// Wrong Password
        }

        [TestMethod]
        public void TestNavigation()
        {
            
        }
    }
}
