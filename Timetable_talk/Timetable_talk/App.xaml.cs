using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Timetable_talk;
using Timetable_talk.ViewModels;


namespace Timetable_talk
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new NavigatingPage());
        }

        protected override void OnStart()
        {
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
