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
            public submittedTransaction(string mo, string serial, string op)
            {
                moID = mo;
                serialID = serial;
                opID = op;
            }
            public string moID { get; set; }
            public string serialID { get; set; }
            public string opID { get; set; }
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
                    trans.openTrans.Add(new submittedTransaction(jo["mo_id"].ToString(), jo["Serial"].ToString(), jo["sequence_id"].ToString()));
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
            return;
        }
    }
}