using System;
using System.Threading.Tasks;
using Contacts.Views;

namespace Contacts.Services
{
    public class NavigationService
    {
		public async Task Navigate(string pageName)
		{
			switch (pageName)
			{
				case "NewContactPage":
					await App.Current.MainPage.Navigation.PushAsync(new NewContactPage());
					break;
				default:
					break;
			}
		}

		public async Task Back()
		{
			await App.Current.MainPage.Navigation.PopAsync();
		}

	}
}
