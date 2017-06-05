using System;
using System.Collections.Generic;
using Contacts.ViewModels;
using Xamarin.Forms;

namespace Contacts.Views
{
    public partial class ContactsPage : ContentPage
    {
        public ContactsPage()
        {
            InitializeComponent();

            var viewModel = ContactsViewModel.GetInstance();
            base.Appearing += (object sender, EventArgs e) => {
                viewModel.RefreshCommand.Execute(this);
            };
        }
    }
}
