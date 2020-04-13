using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QupidMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class transactions : ContentPage
    {
        public transactions()
        {
            InitializeComponent();
        }

        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            SecureStorage.Remove("user_id");
            Navigation.PushModalAsync(new MainPage());
        }

        private void ScanQRCode_Clicked(object sender, EventArgs e)
        {
            scanQRCode();
        }
        private void scanQRCode()
        {
#if __ANDROID__
	// Initialize the scanner first so it can track the current context
	MobileBarcodeScanner.Initialize (Application);
#endif
            string barcodeMessage;
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            //Tell our scanner to use the default overlay
            scanner.UseCustomOverlay = false;
            
            //We can customize the top and bottom text of our default overlay
            scanner.TopText = "Hold camera up to barcode";
            scanner.BottomText = "Camera will automatically scan barcode\r\n\r\nPress the 'Back' button to Cancel";

            //Start scanning
            scanner.ScanContinuously(result =>
            {
                barcodeMessage = result.Text;
                Debug.WriteLine("QRCode: " + barcodeMessage);
                DisplayAlert("QRCode", barcodeMessage, "ok");
                
            });
        }
        private async void takePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            //image.Source = ImageSource.FromStream(() =>
            //{
            //    var stream = file.GetStream();
            //    return stream;
            //});
            var stream = file.GetStream();
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            string base64 = System.Convert.ToBase64String(bytes);
            DisplayAlert("Image info", base64, "YEET");
        }

        private void takePic_Clicked(object sender, EventArgs e)
        {
            takePicture();
        }
    }
}