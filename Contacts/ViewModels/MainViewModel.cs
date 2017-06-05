using System;
using System.Windows.Input;
using Contacts.Services;
using GalaSoft.MvvmLight.Command;

namespace Contacts.ViewModels
{
    public class MainViewModel
    {
        #region Attributes
        private NavigationService navigationService;
        #endregion

        #region Properties
        public ContactsViewModel Contacts
        {
            get;
            set;
        }

        public NewContactViewModel NewContact
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            Contacts = new ContactsViewModel();
            navigationService = new NavigationService();
        }
        #endregion

        #region Commands
        public ICommand AddContactCommand
        {
            get{
                return new RelayCommand(AddContact);
            }

        }

        private async void AddContact()
        {
            NewContact = new NewContactViewModel();
            await navigationService.Navigate("NewContactPage");
        }
        #endregion
    }
}
