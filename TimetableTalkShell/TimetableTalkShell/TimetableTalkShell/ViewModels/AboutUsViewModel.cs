using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using TimetableTalkShell.Models;

namespace TimetableTalkShell.ViewModels
{
    /// <summary>
    /// ViewModel of AboutUs templates.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class AboutUsViewModel : INotifyPropertyChanged
    {
        #region Fields

        // private string productDescription;

        private string productVersion;

        private string productIcon;

        private string cardsTopImage;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="T:TimetableTalkShell.ViewModels.AboutUsViewModel"/> class.
        /// </summary>
        public AboutUsViewModel()
        {
            //this.productDescription = "Test product description";
            this.productIcon = App.BaseImageUrl + "Icon.png";
            this.productVersion = "1.0 Beta";
            this.cardsTopImage = App.BaseImageUrl + "Mask.png";

            this.EmployeeDetails = new ObservableCollection<AboutUsModel>
            {
                new AboutUsModel
                {
                    EmployeeName = "Thomas",
                    Image = App.BaseImageUrl + "ProfileImage15.png",
                    Designation = "Developer"
                },
                new AboutUsModel
                {
                    EmployeeName = "Jacob",
                    Image = App.BaseImageUrl + "ProfileImage10.png",
                    Designation = "Developer"
                },
                new AboutUsModel
                {
                    EmployeeName = "Hano",
                    Image = App.BaseImageUrl + "ProfileImage11.png",
                    Designation = "Developer"
                },
                new AboutUsModel
                {
                    EmployeeName = "Bill",
                    Image = App.BaseImageUrl + "ProfileImage12.png",
                    Designation = "Developer"
                },

            };


            this.ItemSelectedCommand = new Command(this.ItemSelected);
        }

        #endregion

        #region Event handler

        /// <summary>
        /// Occurs when the property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the top image source of the About us with cards view.
        /// </summary>
        /// <value>Image source location.</value>
        public string CardsTopImage
        {
            get { return this.cardsTopImage; }

            set
            {
                this.cardsTopImage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the description of a product or a company.
        /// </summary>
        /// <value>The product description.</value>
        /*public string ProductDescription
        {
            get { return this.productDescription; }
            set
            {
                this.productDescription = value;
                this.OnPropertyChanged();
            }
        }*/

        /// <summary>
        /// Gets or sets the product icon.
        /// </summary>
        /// <value>The product icon.</value>
        public string ProductIcon
        {
            get { return this.productIcon; }

            set
            {
                this.productIcon = value;
                this.OnPropertyChanged();
            }
        }



        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public string ProductVersion
        {
            get { return this.productVersion; }

            set
            {
                this.productVersion = value;
                this.OnPropertyChanged();
            }

        }


        /// <summary>
        /// Gets or sets the employee details.
        /// </summary>
        /// <value>The employee details.</value>
        public ObservableCollection<AboutUsModel> EmployeeDetails { get; set; }

        /// <summary>
        /// Gets or sets the command that will be executed when an item is selected.
        /// </summary>
        public Command ItemSelectedCommand { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The PropertyChanged event occurs when changing the value of property.
        /// </summary>
        /// <param name="propertyName">The PropertyName</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Invoked when an item is selected.
        /// </summary>
        private void ItemSelected(object selectedItem)
        {
            // Do something
        }

        #endregion
    }
}