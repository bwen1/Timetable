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
    public partial class HanoPage : ContentPage
    {
        public HanoPage()
        {
            InitializeComponent();
        }

        private async void NavigateButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigatingPage());
        }
    }
}