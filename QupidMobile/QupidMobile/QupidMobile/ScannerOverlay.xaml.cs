using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QupidMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    
    public partial class ScannerOverlay : Grid
    {
        ScannerView parentScanner;
        public ScannerOverlay()
        {
            InitializeComponent();
        }
        public void refreshText(String txt)
        {
            //qrLabel.Text = "QRData: " + txt;
            var txtArray = txt.Split(';');
            if (txtArray.Length == 4 && user.sendingScreen == "transactions")
            {
                qrLabelTitle0.Text = "Part#: ";
                qrLabelTitle1.Text = "OP ID: ";
                qrLabelValue0.Text = txtArray[0] + "\n";
                qrLabelValue1.Text = txtArray[3];
                user.qrData = txt;
            }

            
        }


        private void selBtn_Clicked(object sender, EventArgs e)
        {
            user.sendingScreen = "";
            Navigation.PopAsync();
        }


     //   private void scanBtn_Clicked(object sender, EventArgs e)
     //   {
       //     parentScanner.Scan();
      //  }

        public void SetParent(ScannerView parent)
        {
            parentScanner = parent;
        }
        
    }
}