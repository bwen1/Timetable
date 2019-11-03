using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using System;

namespace TimetableTalkShell.Views
{
    /// <summary>
    /// Page to get review from customer
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewPage
    {
        public ReviewPage()
        {
            this.InitializeComponent();
            //this.ProductImage.Source = App.BaseImageUrl + "Image1.png";
        }
        private async void ToTimetable(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//timetable");
        }
    }
}