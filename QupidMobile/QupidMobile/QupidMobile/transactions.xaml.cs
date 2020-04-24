using Newtonsoft.Json;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

        protected override void OnAppearing()
        {
            if (user.qrData != null)
            {
                var barcodeInfo = user.qrData.Split(';');
                if (barcodeInfo.Length == 4)
                {

                    txtItemID.Text = barcodeInfo[0];
                    txtSerialLot.Text = barcodeInfo[1];
                    txtMoID.Text = barcodeInfo[2];
                    txtOPID.Text = barcodeInfo[3];
                    disableTextFields();
                }
            }
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
        
            user.sendingScreen = "transactions";
           await Navigation.PushAsync(new ScannerView());
        }
        protected void disableTextFields()
        {
            txtItemID.IsEnabled = txtSerialLot.IsEnabled = txtMoID.IsEnabled = txtOPID.IsEnabled = false;
            txtItemID.BackgroundColor = txtSerialLot.BackgroundColor = txtMoID.BackgroundColor = txtOPID.BackgroundColor = Color.FromHex("#DFDFDF");
        }
        protected void enableTextFields()
        {
            txtMachine.Text = "M";
            txtItemID.IsEnabled = txtSerialLot.IsEnabled = txtMoID.IsEnabled = txtOPID.IsEnabled = true;
            txtItemID.BackgroundColor = txtSerialLot.BackgroundColor = txtMoID.BackgroundColor = txtOPID.BackgroundColor = Color.White;
        }
        private async void takePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                DisplayAlert("No Camera", ":( No camera available.", "OK");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            DisplayAlert("Image info", base64, "YEET");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

        private void takePic_Clicked(object sender, EventArgs e)
        {
            takePicture();
        }

        private void btnTransactions_Clicked(object sender, EventArgs e)
        {
            submitTransaction();
        }
        protected void submitTransaction()
        {
            transactionData transaction = new transactionData();
            transaction.moID = txtMoID.Text.Trim();
            transaction.opID = txtOPID.Text.Trim();
            transaction.eid = user.eid.Trim();
            transaction.machine = txtMachine.Text.Trim();
            transaction.serialNo = txtSerialLot.Text.Trim();
            transaction.setup = swSetup.IsEnabled;
            transaction.rework = swRework.IsEnabled;
            transaction.quality = swQuality.IsEnabled;
            var request = HttpWebRequest.Create("http://vsv-qupidweb-01:8410/api/transactions/");
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(transaction, Formatting.Indented);

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

            }
            DisplayAlert("", "Submitted Transaction", "ok");
            clearFields();
        }
        protected void clearFields()
        {
            txtItemID.Text = txtMachine.Text = txtMoID.Text = txtOPID.Text = txtSerialLot.Text = "";
        }

        private void btnViewOpenTransactions_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new openTransactions());
        }
        public class transactionData
        {
            public int transID { get; set; }
            public string moID { get; set; }
            public string opID { get; set; }
            public string eid { get; set; }
            public string machine { get; set; }
            public int qty { get; set; }
            public bool setup { get; set; }
            public bool quality { get; set; }
            public bool rework { get; set; }
            public byte? DoneFlag { get; set; }
            public string serialNo { get; set; }
            public int overrideSid { get; set; }
            public string note { get; set; }
        }

        private void scanBtn_Clicked(object sender, EventArgs e)
        {
            txtScannedValue.Focus();
        }

        private void txtScannedValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            var scannedValue = txtScannedValue.Text.Split(';');
            if(scannedValue.Length == 4)
            {
                txtItemID.Text = scannedValue[0];
                txtSerialLot.Text = scannedValue[1];
                txtMoID.Text = scannedValue[2];
                txtOPID.Text = scannedValue[3];
                disableTextFields();
                txtScannedValue.Text = "";
            }


        }

        private void clearFields_Clicked(object sender, EventArgs e)
        {
            clearFields();
            enableTextFields();
        }
    }
}