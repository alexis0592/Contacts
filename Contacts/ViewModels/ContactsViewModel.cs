using System;
using System.Collections.ObjectModel;
using Contacts.Services;
using Contacts.Models;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace Contacts.ViewModels
{
    public class ContactsViewModel : INotifyPropertyChanged
    {

        #region MyRegion
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private bool isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<ContactItemViewModel> MyContacts
        {
            get;
            set;
        }

        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshing"));
                }
            }
        }

        #endregion

        #region Constructs
        public ContactsViewModel()
        {
            instance = this;
            apiService = new ApiService();
            dialogService = new DialogService();

            MyContacts = new ObservableCollection<ContactItemViewModel>();

        }
        #endregion

        #region Singleton
        static ContactsViewModel instance;

        public static ContactsViewModel GetInstance(){
            if(instance == null){
                instance = new ContactsViewModel();   
            }

            return instance;
        }
        #endregion

        #region Methods
        private async void LoadContacts()
        {

            IsRefreshing = true;
            var response = await apiService.Get<Contact>("http://contactsxamarintata.azurewebsites.net",
                                                         "/api", "/Contacts");
            IsRefreshing = false;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            ReloadContacts((List<Contact>)response.Result);

        }

        private void ReloadContacts(List<Contact> contacts)
        {
            MyContacts.Clear();
            foreach (var contact in contacts.OrderBy(c => c.FirstName).ThenBy(c => c.LastName))
            {
                MyContacts.Add(new ContactItemViewModel
                {
                    ContactId = contact.ContactId,
                    EmailAddress = contact.EmailAddress,
                    FirstName = contact.FirstName,
                    Image = contact.Image,
                    LastName = contact.LastName,
                    PhoneNumber = contact.PhoneNumber,
                });
            }

        }
		#endregion

		#region Commands
		public ICommand RefreshCommand
		{
			get { return new RelayCommand(Refresh); }

		}

		private void Refresh()
		{
			LoadContacts();
		}
		#endregion
	}



}
