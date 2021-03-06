﻿using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TimetableTalkShell.Views
{
    /// <summary>
    /// Class helps to reduce repetitive markup and allows to change the appearance of apps more easily.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCommitmentStyles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCommitmentStyles" /> class.
        /// </summary>
        public AddCommitmentStyles()
        {
            this.InitializeComponent();
        }
    }
}