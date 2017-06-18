using System;
using System.IO;
using System.Threading.Tasks;
using Contacts.Interfaces;
using Contacts.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PicturePickerImplementation))]

namespace Contacts.iOS
{
    public class PicturePickerImplementation : IPicturePicker
    {
        TaskCompletionSource<Stream> taskCompletionSource;
        UIImagePickerController imagePicker;

        public PicturePickerImplementation()
        {
        }

        public Task<Stream> GetImageStreamAsync()
        {
            imagePicker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
                MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)
            };

            // Set event handlers
            imagePicker.FinishedPickingMedia += OnImagePickerFinishedPickingMedia;
            imagePicker.Canceled += OnImagePickerCancelled;

            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentModalViewController(imagePicker, true);

            taskCompletionSource = new TaskCompletionSource<Stream>();
            return taskCompletionSource.Task;
		}

        private void OnImagePickerCancelled(object sender, EventArgs e)
        {
            taskCompletionSource.SetResult(null);
            imagePicker.DismissModalViewController(true);
        }

        private void OnImagePickerFinishedPickingMedia(object sender, 
                                                       UIImagePickerMediaPickedEventArgs args)
        {
            UIImage image = args.EditedImage ?? args.OriginalImage;

            if(image != null){
                NSData data = image.AsJPEG(1);
                Stream stream = data.AsStream();

                taskCompletionSource.SetResult(stream);
            }else{
                taskCompletionSource.SetResult(null);
            }

            imagePicker.DismissModalViewController(true);
        }
    }
}
