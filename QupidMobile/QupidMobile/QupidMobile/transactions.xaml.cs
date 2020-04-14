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
            txtEid.Text = user.eid;
            txtMachine.Text = "M";
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
        private async void scanQRCode()
        {
            #if __ANDROID__
	            // Initialize the scanner first so it can track the current context
	            MobileBarcodeScanner.Initialize (Application);
            #endif      

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            string[] barcodeInfo;
            var result = await scanner.Scan();
            if(result != null)
            {
                barcodeInfo = result.Text.Split(';');
                txtItemID.Text = barcodeInfo[0];
                txtSerialLot.Text = barcodeInfo[1];
                txtMoID.Text = barcodeInfo[2];
                txtOPID.Text = barcodeInfo[3];
                txtItemID.IsEnabled = txtSerialLot.IsEnabled = txtMoID.IsEnabled = txtOPID.IsEnabled = false;
                txtItemID.BackgroundColor = txtSerialLot.BackgroundColor = txtMoID.BackgroundColor = txtOPID.BackgroundColor = Color.FromHex("#DFDFDF");
            }
            
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

        private void btnTransactions_Clicked(object sender, EventArgs e)
        {

        }

        private void btnViewOpenTransactions_Clicked(object sender, EventArgs e)
        {

        }
    }
}