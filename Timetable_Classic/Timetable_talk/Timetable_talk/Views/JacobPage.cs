using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Timetable_talk.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JacobPage : ContentPage
    {
        public JacobPage()
        {
            InitializeComponent();
        }

        private async void NavigateButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigatingPage());
        }
    }
}