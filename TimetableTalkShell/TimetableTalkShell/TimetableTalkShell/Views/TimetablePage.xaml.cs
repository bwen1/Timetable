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
    public partial class TimetablePage : ContentPage
    {
        public TimetablePage()
        {
            InitializeComponent();
        }

        private async void OnTap(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//addcommitment");
        }

    }
}