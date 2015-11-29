using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SecretImage
{
    public sealed partial class DecodeView : PageBase
    {
        public DecodeView()
        {
            this.InitializeComponent();
        }

        Stream image = new MemoryStream();
        String message = "";
        BitmapImage bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
        private string filename;
        HttpResponseMessage response;
        private async void GetSecretMessage_Click(object sender, RoutedEventArgs e)
        {
            HttpContent imageContent = new ByteArrayContent(ReadFully(this.image));
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/Png");
            imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            imageContent.Headers.ContentDisposition.FileName = this.filename;

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(imageContent);

            try
            {
                response = await App.MobileService.InvokeApiAsync("/api/Decode", form, HttpMethod.Post, null, null);
                Debug.WriteLine("response: " + response);
                message = await response.Content.ReadAsStringAsync();
                this.Message.Text= message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        private void ButtonImageSelect_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types
            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");

            filePicker.PickSingleFileAndContinue();
            CoreApplication.GetCurrentView().Activated += viewActivatedafterSelect;
        }
        private async void viewActivatedafterSelect(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                sender.Activated -= viewActivatedafterSelect;
                StorageFile storageFile = args.Files[0];
                filename = storageFile.Name;
                var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                image = stream.CloneStream().AsStream();
                await bitmapImage.SetSourceAsync(stream);

                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);

                this.ImageThumbnail.Source = bitmapImage;
                this.GetSecretMessage.IsEnabled = true;
            }
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        protected override void LoadState(object navParameter)
        {
            //throw new NotImplementedException();
        }
    }
}
