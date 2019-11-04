using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
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
        private string name;
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

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name == value)
                {
                    return;
                }

                this.name = value;
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

        

        #endregion

        #region methods

        /// <summary>
        /// Invoked when the Log In button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void LoginClicked(object obj)
        {

            databaseConnector.Response response = App.backend.LogIn(name, password);
            if (response.status == databaseConnector.statuscode.OK)
            {
                Preferences.Set("Saved_Login", "");
                Preferences.Set("Saved_User", name);
                Preferences.Set("Saved_Pass", password);
                await Shell.Current.GoToAsync("//timetable");
                
            }
            else if (response.status == databaseConnector.statuscode.ERROR)
            {
                //the login failed, change text / color
                this.Name = "";
                this.Password = "";
                this.Subcolor = "#FFD62F2F";
                this.InvalidMessage = response.message;
            }




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
            // navigate to signup page
            await Shell.Current.GoToAsync("//reset");

        }

       

        #endregion
    }
}