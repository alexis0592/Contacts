using System;
using System.ComponentModel;
using System.Windows.Input;
using Contacts.Models;
using Contacts.Services;
using GalaSoft.MvvmLight.Command;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Contacts.ViewModels
{
    public class NewContactViewModel : Contact, INotifyPropertyChanged
    {

		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Attributes
		private DialogService dialogService;
		private ApiService apiService;
		private NavigationService navigationService;
		private bool isRunning;
		private bool isEnabled;
		private ImageSource imageSource;
		private MediaFile file;
		#endregion

		#region Properties
		public bool IsRunning
		{
			set
			{
				if (isRunning != value)
				{
					isRunning = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
				}
			}
			get
			{
				return isRunning;
			}
		}

		public bool IsEnabled
		{
			set
			{
				if (isEnabled != value)
				{
					isEnabled = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
				}
			}
			get
			{
				return isEnabled;
			}
		}

		public ImageSource ImageSource
		{
			set
			{
				if (imageSource != value)
				{
					imageSource = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
				}
			}
			get
			{
				return imageSource;
			}
		}

		#endregion

		#region Contructor
		public NewContactViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();

            IsEnabled = true;
        }
		#endregion

		#region Commands
		public ICommand NewContactCommand { get { return new RelayCommand(NewContact); } }

		private async void NewContact()
		{
			if (string.IsNullOrEmpty(FirstName))
			{
				await dialogService.ShowMessage("Error", "You must enter a first name");
				return;
			}

			if (string.IsNullOrEmpty(LastName))
			{
				await dialogService.ShowMessage("Error", "You must enter a last name");
				return;
			}

			IsRunning = true;
			IsEnabled = false;
			var response = await apiService.Post("http://contactsxamarintata.azurewebsites.net", 
                                                 "/api", "/Contacts", this);
			IsRunning = false;
			IsEnabled = true;

			if (!response.IsSuccess)
			{
				await dialogService.ShowMessage("Error", response.Message);
				return;
			}

			await navigationService.Back();
		}

		public ICommand TakePictureCommand { get { return new RelayCommand(TakePicture); } }

		private async void TakePicture()
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await dialogService.ShowMessage("No Camera", ":( No camera available.");
			}

			file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				Directory = "Sample",
				Name = "test.jpg",
				PhotoSize = PhotoSize.Small,
			});

			IsRunning = true;

			if (file != null)
			{
				ImageSource = ImageSource.FromStream(() =>
				{
					var stream = file.GetStream();
					return stream;
				});
			}

			IsRunning = false;
		}

		#endregion


	}
}
