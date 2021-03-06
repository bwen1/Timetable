﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using TimetableTalkShell.Views;

namespace TimetableTalkShell
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        Dictionary<string, Type> routes = new Dictionary<string, Type>();
        public Dictionary<string, Type> Routes { get { return routes; } }

        public AppShell()
        {
            InitializeComponent();
            this.LogoutCommand = new Command(this.LogoutClicked);
            Shell.SetTabBarIsVisible(this, false);
        }

        void RegisterRoutes()
        {
            routes.Add("items", typeof(ItemsPage));
            routes.Add("signup", typeof(SignUpPage));
            routes.Add("login", typeof(LoginPage));
            routes.Add("resetpassword", typeof(ResetPasswordPage));
            routes.Add("addcommitment", typeof(AddCommitmentPage));
            routes.Add("about", typeof(AboutPage));
            routes.Add("bill", typeof(BillPage));
            routes.Add("thomas", typeof(ThomasPage));
            routes.Add("apppages", typeof(AppPagesPage));
            routes.Add("timetable", typeof(TimetablePage));

            foreach (var item in routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }

        void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {

        }

        void OnNavigated(object sender, ShellNavigatedEventArgs e)
        {

        }
        /// <summary>
        /// Gets or sets the command that is executed when the Log out button is clicked.
        /// </summary>
        public Command LogoutCommand { get; set; }
        private async void LogoutClicked()
        {
            Preferences.Clear();
            App.backend.LogOut();
            await Shell.Current.GoToAsync("//timetable");
        }

    }
}
