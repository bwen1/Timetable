using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TimetableTalkShell
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Shell.SetTabBarIsVisible(this, false);
        }
    }
}
