using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Text;
using MvvmHelpers;
using Xamarin.Forms;

namespace TimetableTalkShell.ViewModels
{
    public class BillViewModel : ContentPage
    {
        public ICommand Test_Backend { get; private set; }
        public string sometext { get; private set; }
        public BillViewModel()
        {
            Title = "Bill";
            Test_Backend = new Command(test_backend);
            sometext = "0";
            OnPropertyChanged(sometext);
        }

        public async void test_backend()
        {
            
            await DisplayAlert("Alert", "testing Backend connection", "cool");
            sometext = "1";
            OnPropertyChanged(sometext);
            Console.WriteLine("woop woop");
        }
    }
}