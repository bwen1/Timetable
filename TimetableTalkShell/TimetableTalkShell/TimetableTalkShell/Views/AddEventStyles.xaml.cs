using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TimetableTalkShell.Views
{
    /// <summary>
    /// Class helps to reduce repetitive markup and allows to change the appearance of apps more easily.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEventStyles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddEventStyles" /> class.
        /// </summary>
        public AddEventStyles()
        {
            this.InitializeComponent();
        }
    }
}