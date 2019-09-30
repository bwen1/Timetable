using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TimetableTalkShell.ViewModels.ReviewsandRatings
{
    /// <summary>
    /// ViewModel for review page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ReviewPageViewModel
    {
        #region Constructor

        public ReviewPageViewModel()
        {
            this.UploadCommand = new Command<object>(this.OnUploadTapped);
            this.SubmitCommand = new Command<object>(this.OnSubmitTapped);
        }

        #endregion

        #region Command

        /// <summary>
        /// Gets or sets the value for upload command.
        /// </summary>
        public Command<object> UploadCommand { get; set; }

        /// <summary>
        /// Gets or sets the value for submit command.
        /// </summary>
        public Command<object> SubmitCommand { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Invoked when the Upload button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void OnUploadTapped(object obj)
        {
            // Do something
        }

        /// <summary>
        /// Invoked when the Submit button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void OnSubmitTapped(object obj)
        {
            // Do something
            databaseConnector.Response response = App.backend.AddEvent(new databaseConnector.Event());
            // add the varibles from the page to the event object like Event(name, shared, start, end, day, location, [notes])
            if(response.status == databaseConnector.statuscode.OK)
            {
                //navigate to whereever when sucess.
            }
            else
            {
                //find further info in response.stats, and response.message, then do things as appropriate.
            }
        }

        #endregion
    }
}
