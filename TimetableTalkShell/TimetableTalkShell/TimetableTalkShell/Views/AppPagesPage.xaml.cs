using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TimetableTalkShell.ViewModels;

namespace TimetableTalkShell.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppPagesPage : ContentPage
    {
        public AppPagesPage()
        {
            InitializeComponent();

        }



        async void OnSignup(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//signup");
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//login");
        }

        private async void OnResetPassword(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//resetpassword");
        }
        private async void OnReview(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//review");
        }
        private async void OnAbout(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//about");
        }
        private async void OnBill(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//bill");
        }
        private async void OnThomas(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//thomas");
        }
    }
}