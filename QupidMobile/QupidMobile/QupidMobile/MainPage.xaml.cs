﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.AppCenter.Crashes;
using System.Net;
using Xamarin.Essentials;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QupidMobile
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            checkifLoggedIn();
            InitializeComponent();
        }
        private async void checkifLoggedIn()
        {
            var userID = await SecureStorage.GetAsync("user_id");
            var eid = await SecureStorage.GetAsync("eid");
            user.fullName = userID;
            user.eid = eid;
            if (userID != "" && userID != null)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                Navigation.PushModalAsync(new MasterDetailPage1());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            }
        }
        private void Login_Clicked(object sender, EventArgs e)
        {
            string result;
            loginInfo pass = new loginInfo();
            pass.username = UserName.Text;
            pass.password = Password.Text;
            var request = HttpWebRequest.Create("http://vsv-qupidweb-01:8410/api/values/");
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(pass, Formatting.Indented);

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();

            }
            if (result == "0")
                DisplayAlert("Login Incorrect", "Please enter a correct username and password", "OK");
            else
            {

                JArray a = JArray.Parse(result);
                JObject o = JObject.Parse(a[0].ToString());
                user.eid = o["Eid"].ToString();

                user.fullName = o["FullName"].ToString();
                SecureStorage.SetAsync("user_id", o["FullName"].ToString());
                SecureStorage.SetAsync("eid", o["Eid"].ToString());
                Application.Current.SavePropertiesAsync();
                Navigation.PushModalAsync(new MasterDetailPage1());
            }

        }
        public class loginInfo
        {
            public string username { get; set; }
            public string password { get; set; }
        }

    }
}
