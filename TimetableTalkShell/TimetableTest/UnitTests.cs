using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimetableTalkShell.ViewModels;

namespace TimetableTest
{
    [TestClass]
    public class SignUpViewModelTest
    {
        [TestMethod]
        public void SignupCommandTestwithemailandpassword()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "hanogy@gmail.com";
            vm.Password = "hobolobo456";


            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Email == "hanogy@gmail.com", "Valid Email");//Correct result
            Assert.IsTrue(vm.Password == "hobolobo456", "Valid Password");//Correct result
            
        }


        [TestMethod]
        public void SignupCommandTestwithnoemailandpassword()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "";
            vm.Password = "";


            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Email == "", "Invalid Email");//Wrong result
            Assert.IsTrue(vm.Password == "", "Invalid Password");//Wrong result

        }

        [TestMethod]
        public void SignupCommandTestwithemailandnopassword()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "hanogy@gmail.com";
            vm.Password = "";


            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Email == "hanogy@gmail.com", "Valid Email");//Correct result
            Assert.IsTrue(vm.Password == "", "Invalid Password");//Wrong result

        }

        [TestMethod]
        public void SignupCommandTestwithpasswordandnoemail()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "";
            vm.Password = "Xlriggde";


            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Email == "", "Invalid Email");//Wrong result
            Assert.IsTrue(vm.Password == "Xlriggde", "Valid Password");//Correct result

        }

        public void SignupCommandTestwithincorrectemailandpassword()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "alexmercer.com";
            vm.Password = "Xlriggde";


            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Email == "alexmercer.com", "Invalid Email");//Wrong result
            Assert.IsTrue(vm.Password == "Xlriggde", "Valid Password");//Correct result

        }


        public void SignupCommandTestwithspecialcharemailandpassword()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "-/[][;[@gmail.com";
            vm.Password = ":():";


            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Email == "-/[][;[@gmail.com", "Invalid Email");//Wrong result
            Assert.IsTrue(vm.Password == ":():", "Invalid Password");//Wrong result

        }


        public void SignupCommandTestwithalreadytakenemailandnewpassword()
        {
            //Arrange
            var vm = new SignUpViewModel();
            vm.Email = "billwen23@gmail.com";//existing email
            vm.Password = "snogeojoe";


            //Act
            vm.SignUpCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Email == "billwen23@gmail.com", "Email taken! Try a new one");//Wrong result
            Assert.IsTrue(vm.Password == "snogeojoe", "Valid Password");//Correct result

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
            
        }


        [TestMethod]
        public void LoginClickedCommandTestwithnousernameandpassword()
        {
            //Arrange
            var vm = new LoginViewModel();
            vm.Name = "";
            vm.Password = "";


            //Act
            vm.LoginCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Name == "", "Invalid Username");//Wrong result
            Assert.IsTrue(vm.Password == "", "Invalid Password");//Wrong result

        }

        [TestMethod]
        public void LoginClickedCommandTestwithusernameandnopassword()
        {
            //Arrange
            var vm = new LoginViewModel();
            vm.Name = "Shampoo345";
            vm.Password = "";


            //Act
            vm.LoginCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Name == "Shampoo345", "Valid Username");//Correct result
            Assert.IsTrue(vm.Password == "", "Invalid Password");//Wrong result

        }

        [TestMethod]
        public void LoginClickedCommandTestwithpasswordandnousername()
        {
            //Arrange
            var vm = new LoginViewModel();
            vm.Name = "";
            vm.Password = "Xlriggde";


            //Act
            vm.LoginCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Name == "", "Invalid username");//Wrong result
            Assert.IsTrue(vm.Password == "Xlriggde", "Valid Password");//Correct result

        }

        public void LoginClickedCommandTestwithspecialcharusernameandpassword()
        {
            //Arrange
            var vm = new LoginViewModel();
            vm.Name = "-/[][;[??.";
            vm.Password = "}:():}";


            //Act
            vm.LoginCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Email == "-/[][;[??.", "Invalid Username");//Wrong result
            Assert.IsTrue(vm.Password == "}:():}", "Invalid Password");//Wrong result

        }

        public void LoginCommandTestwithalreadytakenusernameandnewpassword()
        {
            //Arrange
            var vm = new LoginViewModel();
            vm.Name = "JacobFreeman34";//existing username
            vm.Password = "snogeojoe";


            //Act
            vm.LoginCommand.Execute(null);

            //Assert
            Assert.IsTrue(vm.Name == "JacobFreeman34", "Username taken! Try a new one");//Wrong result
            Assert.IsTrue(vm.Password == "snogeojoe", "Valid Password");//Correct result

        }

    }


    [TestClass]
    public class ChangeUsernameViewModelTest
    {

        [TestMethod]
        public void ChangeUsernameClickedCommandTest()
        {
            //Arrange
            var vm = new ChangeUsernameViewModel();
            vm.newUsername = "heya";
            


            //Act
            vm.ChangeUsername.Execute(null);

            //Assert
            Assert.IsTrue(vm.GetType().Name == "ChangeUsernameViewModel", "viewmodel");// Correct result

        }


       

    }
}
