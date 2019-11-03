using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimetableTalkShell.ViewModels;
using TimetableTalkShell;
using TimetableTalkShell.Views;
using Xamarin.Forms;
using Xamarin.Essentials;


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
            
            vm.Email = "hehehe@gmail.com";
            vm.Password = "hobolobo456";
            
            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            //Assert.IsTrue();
           // Assert.IsTrue(vm.Email == "", "Email is Invalid");// Wrong Email
           // Assert.IsTrue(vm.Password == "", "Password is Invalid");// Wrong Password
        }
    }
}
