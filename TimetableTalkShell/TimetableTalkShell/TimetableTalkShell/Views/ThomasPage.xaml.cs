using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimetableTalkShell.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThomasPage : ContentPage
    {
        public ThomasPage()
        {
            this.InitializeComponent();
        }

        private async void OnTap(object sender, EventArgs e)
        {
            (sender as Button).BackgroundColor = Color.FromHex("#009688");
            await Navigation.PushAsync(new AddEventPage());
        }

    }
}