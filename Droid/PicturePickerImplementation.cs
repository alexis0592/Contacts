﻿using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Contacts.Droid;
using Contacts.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(PicturePickerImplementation))]

namespace Contacts.Droid
{
    public class PicturePickerImplementation : IPicturePicker
    {
        public Task<Stream> GetImageStreamAsync()
        {
			Intent intent = new Intent();
			intent.SetType("image/*");
			intent.SetAction(Intent.ActionGetContent);

            MainActivity activity = Forms.Context as MainActivity;

			activity.StartActivityForResult(
			   Intent.CreateChooser(intent, "Select Picture"),
			   MainActivity.PickImageId);

            activity.PickImageTaskCompletionSource = new TaskCompletionSource<Stream>();

            return activity.PickImageTaskCompletionSource.Task;
        }
    }
}
