using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using TimetableTalkShell.Services;
using TimetableTalkShell.Views;
using databaseConnector;

namespace TimetableTalkShell
{
    public partial class App : Application
    {
        public static string BaseImageUrl { get; } = "https://cdn.syncfusion.com/essential-ui-kit-for-xamarin.forms/common/uikitimages/";
        public static Backend backend { get; private set; } 
        public App()
        {
            InitializeComponent();
            backend = new Backend();
            DependencyService.Register<MockDataStore>();
            MainPage = new LoginPage();
            
            
            
            //MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            if (Preferences.ContainsKey("Saved_Login"))
            {
                Response re = await backend.LogIn(Preferences.Get("Saved_User", ""), Preferences.Get("Saved_Pass", "No"));
                if (re.status == statuscode.OK)
                {
                    MainPage = new TimetablePage();
                }
                else if (re.status == statuscode.NOT_THESE_DROIDS)
                {
                    Preferences.Clear();
                    Device.BeginInvokeOnMainThread(async () => {
                        await MainPage.DisplayAlert("Saved login fail", "We coulden't log you in with your saved credentials", "OK");
                        MainPage = new LoginPage();
                    });
                }
                else
                {
                    MainPage = new LoginPage();
                }
            }
            else
            {
                MainPage = new LoginPage();
            }
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
