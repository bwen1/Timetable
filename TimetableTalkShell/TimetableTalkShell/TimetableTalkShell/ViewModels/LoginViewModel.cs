using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TimetableTalkShell.ViewModels
{
    /// <summary>
    /// ViewModel for login page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class LoginViewModel : LoginBaseViewModel
    {
        #region Fields

        private string password;
        private string color;
        private string invalidmessage = "Your Timetable is waiting for you :)";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="LoginViewModel" /> class.
        /// </summary>
        public LoginViewModel()
        {
            this.LoginCommand = new Command(this.LoginClicked);
            this.SignUpCommand = new Command(this.SignUpClicked);
            this.ForgotPasswordCommand = new Command(this.ForgotPasswordClicked);
            this.SocialMediaLoginCommand = new Command(this.SocialLoggedIn);
        }

        #endregion

        #region property

        /// <summary>
        /// Gets or sets the property that is bound with an entry that gets the password from user in the login page.
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

        /// <summary>
        /// Gets or sets the command that is executed when the Forgot Password button is clicked.
        /// </summary>
        public Command ForgotPasswordCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the social media login button is clicked.
        /// </summary>
        public Command SocialMediaLoginCommand { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Invoked when the Log In button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void LoginClicked(object obj)
        {

            databaseConnector.Response response = App.backend.LogIn("Bob", password);
            if (response.status == databaseConnector.statuscode.OK)
            {
                Application.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync("//timetable");
                
            }



            // Do something
            //databaseConnector.Response response = App.backend.LogIn(this.Email, this.password);
            //if (response.status == databaseConnector.statuscode.OK)
            //{
            //    App.backend.GetFriends();
            //    App.backend.GetEvents();
            //    //navigate from here, to home
            //}
            //else if (response.status == databaseConnector.statuscode.ERROR)
            //{
            //    //the signup failed, change text / color
            //    this.Email = "";
            //    this.Password = "";
            //    this.Subcolor = "#FFD62F2F";
            //    this.InvalidMessage = "Invalid Email or Password!";
            //}
        }

        /// <summary>
        /// Invoked when the Sign Up button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void SignUpClicked(object obj)
        {
            // navigate to signup page
            await Shell.Current.GoToAsync("//signup");
        }

        /// <summary>
        /// Invoked when the Forgot Password button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void ForgotPasswordClicked(object obj)
        {
            var label = obj as Label;
            label.BackgroundColor = Color.FromHex("#70FFFFFF");
            await Task.Delay(100);
            label.BackgroundColor = Color.Transparent;
            
        }

        /// <summary>
        /// Invoked when social media login button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void SocialLoggedIn(object obj)
        {
            // Do something
        }

        #endregion
    }
}