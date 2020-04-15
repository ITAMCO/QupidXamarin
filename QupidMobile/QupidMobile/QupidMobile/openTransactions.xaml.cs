using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QupidMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class openTransactions : ContentPage
    {
        public openTransactions()
        {
            InitializeComponent();
            getOpenTransactions();
        }
        public class submittedTransaction
        {
            public int sid { get; set; }
            public string mo_id { get; set; }
            public string Serial { get; set; }
            public string sequence_id { get; set; }
            public string wc_id { get; set; }
            public byte? setup_flag { get; set; }
            public byte? rework_flag { get; set; }
            public byte? lot_flag { get; set; }
            public DateTime startDate { get; set; }
            public string startDateView { get; set; }
            public DateTime startTime { get; set; }
            public DateTime Eta { get; set; }
            public string EtaView { get; set; }
        }
        public void getOpenTransactions()
        {
            TransactionsFactory trans = new TransactionsFactory();
            trans.openTrans = new List<submittedTransaction>();
            string result;
            var request = HttpWebRequest.Create("http://vsv-qupidweb-01:8410/api/transactions/" + user.eid);
            request.ContentType = "application/json";
            request.Method = "GET";
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();

            }
            if(result == "")
            {

            }
            else
            {
                JArray a = JArray.Parse(result);
                for(int i = 0; i< a.Count; i++)
                {
                    JObject jo = JObject.Parse(a[i].ToString());
                     
                    submittedTransaction transactionData = jo.ToObject<submittedTransaction>();
                    transactionData.startDateView = transactionData.startDate.ToString("MM/dd");
                    transactionData.startDateView += " " + transactionData.startTime.ToString("hh:mm");
                    transactionData.EtaView = transactionData.Eta.ToString("MM/dd HH:mm");
                    trans.openTrans.Add(transactionData);
                }
                openTransactionsView.ItemsSource = trans.openTrans;
            }
        }
        public class TransactionsFactory
        {
            public List<submittedTransaction> openTrans { get; set; }
            
        }

        private void openTransactionsView_Refreshing(object sender, EventArgs e)
        {
            getOpenTransactions();
            openTransactionsView.IsRefreshing = false;
        }

        private void openTransactionsView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var trans = (submittedTransaction)e.SelectedItem;
            getAnswer(trans);
        }
        protected async void getAnswer(submittedTransaction trans)
        {
            var ans = await DisplayAlert("Finish Transaction", "Are you sure you would like finish serial: " + trans.Serial, "Finish", "Cancel");
            if( ans)
            {
                closeTransaction(trans);
            }
            else
            {
                getOpenTransactions();
            }
            //popupLoadingView.IsVisible = true;
        }
        protected void closeTransaction(submittedTransaction finishtransaction)
        {
            transactions.transactionData transaction = new transactions.transactionData();
            transaction.moID = finishtransaction.mo_id;
            transaction.opID = finishtransaction.sequence_id;
            transaction.DoneFlag = 0;
            transaction.eid = user.eid;
            transaction.machine = finishtransaction.wc_id;
            transaction.serialNo = finishtransaction.Serial;
            transaction.transID = finishtransaction.sid;
            transaction.setup = Convert.ToBoolean(finishtransaction.setup_flag);
            transaction.rework = Convert.ToBoolean(finishtransaction.rework_flag);
            transaction.qty = 0;
            transaction.note = "";
            var request = HttpWebRequest.Create("http://vsv-qupidweb-01:8410/api/finishTransaction/");
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
            getOpenTransactions();
        }
    }
}