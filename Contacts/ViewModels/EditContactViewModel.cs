﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Contacts.Helpers;
using Contacts.Interfaces;
using Contacts.Models;
using Contacts.Services;
using GalaSoft.MvvmLight.Command;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Contacts.ViewModels
{
    public class EditContactViewModel : Contact, INotifyPropertyChanged
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
        private Stream stream;
		private MediaFile file;
        private bool isFromCamera;
        private bool isFromGallery;
        private byte[] imageArray = null;
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


		#region Constructors
		public EditContactViewModel(Contact contact)
		{
			dialogService = new DialogService();
			apiService = new ApiService();
			navigationService = new NavigationService();

			ContactId = contact.ContactId;
			FirstName = contact.FirstName;
            LastName = contact.LastName;
            Image = contact.Image;
            EmailAddress = contact.EmailAddress;
            PhoneNumber = contact.PhoneNumber;

            IsEnabled = true;

		}
		#endregion

		#region Commands
		public ICommand DeleteContactCommand { get { return new RelayCommand(DeleteContact); } }

		private async void DeleteContact()
		{
			var answer = await dialogService.ShowConfirm("Confirm", "Are you sure to delete this record?");
			if (!answer)
			{
				return;
			}

			var contact = new Contact
			{
				EmailAddress = EmailAddress,
				FirstName = FirstName,
				LastName = LastName,
				PhoneNumber = PhoneNumber,
				Image = Image,
				ContactId = ContactId,
			};

			IsRunning = true;
			IsEnabled = false;
			var response = await apiService.Delete("http://contactsxamarintata.azurewebsites.net",
                                                   "/api", "/Contacts", contact);
			IsRunning = false;
			IsEnabled = true;

			if (!response.IsSuccess)
			{
				await dialogService.ShowMessage("Error", response.Message);
				return;
			}

			await navigationService.Back();
		}

		public ICommand SaveContactCommand { get { return new RelayCommand(SaveContact); } }

		private async void SaveContact()
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

            if (isFromCamera && file != null){

	            imageArray = FilesHelper.ReadFully(file.GetStream());
	            file.Dispose();
            }

			var contact = new Contact
			{
				EmailAddress = EmailAddress,
				FirstName = FirstName,
				ImageArray = imageArray,
				LastName = LastName,
				PhoneNumber = PhoneNumber,
				Image = Image,
				ContactId = ContactId,
			};

			IsRunning = true;
			IsEnabled = false;
			var response = await apiService.Put("http://contactsxamarintata.azurewebsites.net", 
                                                "/api", "/Contacts", contact);
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
                return;
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

            isFromCamera = true;
            isFromGallery = false;

			IsRunning = false;
		}

        public ICommand SelectPictureCommand
        {
            get { return new RelayCommand(SelectPicture); }
        }

        private async void SelectPicture()
        {
            Stream myStream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

            if(myStream != null){
                
                var ImageS = new Image
                {
                    Source = ImageSource.FromStream(() => myStream)
                };

                ImageSource = ImageS.Source;
                stream = myStream;

				imageArray = FilesHelper.ReadFully(stream);

                isFromCamera = false;
                isFromGallery = true;
            }
        }

        #endregion


    }
}
