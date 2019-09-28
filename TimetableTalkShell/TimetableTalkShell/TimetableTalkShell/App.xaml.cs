using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TimetableTalkShell.Services;
using TimetableTalkShell.Views;
using databaseConnector;

namespace TimetableTalkShell
{
    public partial class App : Application
    {
        public static string BaseImageUrl { get; } = "https://cdn.syncfusion.com/essential-ui-kit-for-xamarin.forms/common/uikitimages/";
        public static Backend backend { get; } = new Backend();
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new BillPage();
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
