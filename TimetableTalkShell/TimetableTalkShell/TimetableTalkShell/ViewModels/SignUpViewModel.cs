using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TimetableTalkShell.ViewModels
{
    /// <summary>
    /// ViewModel for sign-up page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class SignUpViewModel : LoginBaseViewModel
    {
        #region Fields

        private string name;

        private string password;

        private string confirmPassword;
        private string color;
        private string invalidmessage = "To begin, tell us about yourself :)";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="SignUpViewModel" /> class.
        /// </summary>
        public SignUpViewModel()
        {
            this.LoginCommand = new Command(this.LoginClicked);
            this.SignUpCommand = new Command(this.SignUpClicked);
        }

        #endregion

        #region Property

        

        public string Subcolor
        {
            get
            {
                return this.color;
            }

            set
            {
                if (this.color == value)
                {
                    return;
                }

                this.color = value;
                this.OnPropertyChanged();
            }
        }

        public string InvalidMessage
        {
            get
            {
                return this.invalidmessage;
            }

            set
            {
                if (this.invalidmessage == value)
                {
                    return;
                }

                this.invalidmessage = value;
                this.OnPropertyChanged();
            }
        }



        /// <summary>
        /// Gets or sets the property that bounds with an entry that gets the password from users in the Sign Up page.
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (this.password == value)
                {
                    return;
                }

                this.password = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that bounds with an entry that gets the password confirmation from users in the Sign Up page.
        /// </summary>
        public string ConfirmPassword
        {
            get
            {
                return this.confirmPassword;
            }

            set
            {
                if (this.confirmPassword == value)
                {
                    return;
                }

                this.confirmPassword = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Gets or sets the command that is executed when the Log In button is clicked.
        /// </summary>
        public Command LoginCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the Sign Up button is clicked.
        /// </summary>
        public Command SignUpCommand { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Invoked when the Log in button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void LoginClicked(object obj)
        {

            await Shell.Current.GoToAsync("//login");

            //databaseConnector.Response response = App.backend.SignUp(this.Email, this.password);
            //if (response.status == databaseConnector.statuscode.OK)
            //{
            //    //navigate from here, to login
            //}
            //else if (response.status == databaseConnector.statuscode.ERROR)
            //{
            //    //the signup failed, change text / color
            //    this.Email = "";
            //    this.Password = "";
            //    this.Subcolor = "#FFD62F2F";
            //    this.InvalidMessage = response.message;
            //}
            // Do something
        }

        /// <summary>
        /// Invoked when the Sign Up button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void SignUpClicked(object obj)
        {
            databaseConnector.Response response = App.backend.LogIn("Bob", password);
            if (response.status == databaseConnector.statuscode.OK)
            {
                await Shell.Current.GoToAsync("//login");

            }
            else if (response.status == databaseConnector.statuscode.ERROR)
            {
                //the signup failed, change text / color
                this.Email = "";
                this.Password = "";
                this.Subcolor = "#FFD62F2F";
                this.InvalidMessage = response.message;
            }
        }

        #endregion
    }
}