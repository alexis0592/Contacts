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
		public EditContactViewModel EditContact 
        { 
            get;
            set; 
        }

		#endregion

		#region Constructors
		public MainViewModel()
        {
            instance = this;
            Contacts = new ContactsViewModel();
            navigationService = new NavigationService();
        }
		#endregion

		#region Singleton
        static MainViewModel instance;

        public static MainViewModel GetInstance()
		{
			if (instance == null)
			{
                instance = new MainViewModel();
			}

			return instance;
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
