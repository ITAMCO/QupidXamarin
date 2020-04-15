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
    }
}