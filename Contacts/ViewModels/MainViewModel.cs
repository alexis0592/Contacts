using System;
namespace Contacts.ViewModels
{
    public class MainViewModel
    {

        public ContactsViewModel Contacts
        {
            get;
            set;
        }

        public MainViewModel()
        {
            Contacts = new ContactsViewModel();
        }
    }
}
