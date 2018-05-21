using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using ttrssclientgui.dto;
using ttrssclientgui.requests;
using ttrssclientgui.responses;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ttrssclientgui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            mainWorkAsync();
        }

        private async void mainWorkAsync()
        {
            Uri server = new Uri("http://example/tt-rss/api/");
            
            string username = "user";
            string password = "password";
            

            HttpStringContent httpLogin = new HttpStringContent("{\"op\":\"login\",\"user\":\"" + username + "\",\"password\":\"" + password + "\"}");
            
            HttpClient client = new HttpClient();
            string jsonString = "booo";
            HttpResponseMessage response = new HttpResponseMessage();

            JsonObject tempJsonArray = new JsonObject();
            JsonObject tempJsonArray2 = new JsonObject();
            Response<LoginResponse> loginResponse = new Response<LoginResponse>();
            loginResponse.content = new LoginResponse();
            try
            {
                response = await client.PostAsync(server, httpLogin);
                jsonString = await response.Content.ReadAsStringAsync();
                tempJsonArray = JsonObject.Parse(jsonString);
                tempJsonArray2 = tempJsonArray.GetNamedObject("content");
                try
                {
                    loginResponse.content.error = (string)tempJsonArray2.GetNamedString("error");
                    MessageDialog msg4 = new MessageDialog(loginResponse.content.error);
                    await msg4.ShowAsync();
                }
                catch
                {
                    loginResponse.seq = (int)tempJsonArray.GetNamedNumber("seq");
                    loginResponse.status = (int)tempJsonArray.GetNamedNumber("status");

                    loginResponse.content.api_level = (int)tempJsonArray2.GetNamedNumber("api_level");
                    loginResponse.content.session_id = (string)tempJsonArray2.GetNamedString("session_id");

                    string session_id = loginResponse.content.session_id;

                    MessageDialog msg = new MessageDialog(jsonString.ToString());
                    await msg.ShowAsync();

                    LogoutRequest logoutObject = new LogoutRequest(session_id);

                    HttpStringContent httpLogout = new HttpStringContent("{\"op\":\"logout\",\"sid\":\"" + session_id + "\"\"}");
                    HttpResponseMessage logoutResponse = await client.PostAsync(server, httpLogin);
                    jsonString = await response.Content.ReadAsStringAsync();
                    MessageDialog msg2 = new MessageDialog("logout" + jsonString.ToString());
                    await msg2.ShowAsync();
                }
            }
            catch
            {
                MessageDialog msg3 = new MessageDialog("cannot connect");
                await msg3.ShowAsync();
            }
            
        }

        static LoginRequest GetLoginRequest(string userName, string password)
        {
            return new LoginRequest() { user = userName, password = password };
        }
    }
}
