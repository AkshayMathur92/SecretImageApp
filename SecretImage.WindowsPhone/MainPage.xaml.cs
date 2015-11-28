
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SecretImage
{
    public sealed partial class MainPage : PageBase
    {
        Stream image = new MemoryStream();
        IRandomAccessStream bitmapstream;
        BitmapImage bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
        private string filename;
        HttpResponseMessage response;

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
                this.GetSecretImage.IsEnabled = true;
            }
        }

        private async void GetSecretImage_Click(object sender, RoutedEventArgs e)
        {
            HttpContent imageContent = new ByteArrayContent(ReadFully(this.image));
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/Png");
            imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            imageContent.Headers.ContentDisposition.FileName = this.filename;

            HttpContent messageContent = new StringContent(this.TextInput.Text);

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(imageContent);
            form.Add(messageContent, "message");

            try
            {
                response = await App.MobileService.InvokeApiAsync("/api/Encode", form, HttpMethod.Post, null, null);
                Debug.WriteLine("response: " + response);
                byte[] bytearray = await response.Content.ReadAsByteArrayAsync();
                Stream imagestream = new MemoryStream(bytearray);
                bitmapstream = imagestream.AsRandomAccessStream();
                await bitmapImage.SetSourceAsync(bitmapstream);
                this.SecretImage.Source = bitmapImage;
                this.SaveSecretImage.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            //String secretImageStream = await secretImageResponse.Content.ReadAsStringAsync();
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

        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ButtonImageSelect.IsEnabled = (((TextBox)sender).Text.Length > 0) ? true : false;
        }

        private async void SaveSecretImage_Click(object sender, RoutedEventArgs e)
        {
            StorageFile myfile = await KnownFolders.PicturesLibrary.CreateFileAsync("Secret_"+filename);
            BitmapDecoder bmpDecoder = await BitmapDecoder.CreateAsync(bitmapstream);
            PixelDataProvider pixelData = await bmpDecoder.GetPixelDataAsync(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, new BitmapTransform(), ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);
            using (var destFileStream = await myfile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder bmpEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destFileStream);
                uint width = (uint)bitmapImage.PixelWidth;
                uint height = (uint)bitmapImage.PixelHeight;
                bmpEncoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, width, height, 300, 300, pixelData.DetachPixelData());
                await bmpEncoder.FlushAsync();
            }
        }
    }
}

