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
    public partial class NavigatingPage : ContentPage
    {
        public NavigatingPage()
        {
            InitializeComponent();
        }

        private async void BillButton(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BillPage());
        }
        private async void HanoButton(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HanoPage());
        }
        private async void JacobButton(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new JacobPage());
        }
        private async void ThomasButton(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ThomasPage());
        }
    }
}