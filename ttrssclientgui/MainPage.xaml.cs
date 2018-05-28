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
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ttrssclientgui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Uri server = new Uri("http://example/tt-rss/api/");

        private string username = "user";
        private string password = "password";

        HttpClient client = new HttpClient();

        // public string session_id = "test";

        public ObservableCollection<Feed> FeedList = new ObservableCollection<Feed>();

        public Feed SelectedFeed;

        public ObservableCollection<HeadLine> HeadList = new ObservableCollection<HeadLine>();

        public MainPage()
        {
            this.InitializeComponent();
            mainWorkAsync();
        }

        private async void mainWorkAsync()
        {
            Response<LoginResponse> loginResponse = new Response<LoginResponse>();
            loginResponse.content = new LoginResponse();
            
            try
            {
                JsonObject loginContent2 = await ExecuteMethod2("{\"op\":\"login\",\"user\":\"" + username + "\",\"password\":\"" + password + "\"}");
                // JsonObject loginContent2 = tempJsonArray.GetNamedObject("content");
                try
                {
                    loginResponse.content.error = (string)loginContent2.GetNamedString("error");
                    MessageDialog serverErrorMessage = new MessageDialog(loginResponse.content.error);
                    await serverErrorMessage.ShowAsync();
                }
                catch
                {
                    loginResponse.content.api_level = (int)loginContent2.GetNamedNumber("api_level");
                    loginResponse.content.session_id = (string)loginContent2.GetNamedString("session_id");

                    string session_id = loginResponse.content.session_id;
                    
                    MessageDialog checkLogin = new MessageDialog(loginContent2.ToString());
                    await checkLogin.ShowAsync();
                    
                    JsonArray feedListJson = await ExecuteMethod("{\"op\":\"getFeeds\",\"sid\":\"" + session_id + "\",\"cat_id\":\"" + "-3" + "\",\"unread_only\":\"" + "true" + "\",\"include_nested\":\"" + "true" + "\"}");
                    
                    //for (uint i = 0; i < feedListJson.Count; i++)
                    foreach (JsonValue feedJson in feedListJson)
                    {
                        JsonObject feedJson2 = JsonObject.Parse(feedJson.ToString());

                        /*
                        MessageDialog checkFeedJson = new MessageDialog(feedJson2.ToString());
                        await checkFeedJson.ShowAsync();
                        */

                        Feed tempFeed = new Feed();
                        tempFeed.headline = new ObservableCollection<HeadLine>();
                        
                        tempFeed.feed_url = feedJson2.GetNamedString("feed_url");
                        tempFeed.title = feedJson2.GetNamedString("title");
                        tempFeed.id = ((int)feedJson2.GetNamedNumber("id")).ToString();
                        tempFeed.unread = ((int)feedJson2.GetNamedNumber("unread")).ToString();
                        tempFeed.has_icon = feedJson2.GetNamedBoolean("has_icon").ToString();
                        tempFeed.cat_id = ((int)feedJson2.GetNamedNumber("cat_id")).ToString();
                        tempFeed.last_updated = ((int)feedJson2.GetNamedNumber("last_updated")).ToString();
                        tempFeed.order_id = ((int)feedJson2.GetNamedNumber("order_id")).ToString();
                        /*
                        JsonObject tempLevel = await ExecuteMethod2("getApiLevel");
                        int apiLevel = (int)tempLevel.GetNamedNumber("level");
                        string headLimit;
                        if(apiLevel < 6)
                        {
                            headLimit = "60";
                        }
                        else
                        {
                            headLimit = "200";
                        }
                        */

                        
                        JsonArray headListJson = await ExecuteMethod("{\"op\":\"getHeadlines\",\"sid\":\"" + session_id + "\",\"feed_id\":\"" + tempFeed.id + "\",\"view_mode\":\"" + "unread\"}");
                        // MessageDialog checkHeadList = new MessageDialog(headListJson.ToString());
                        // await checkHeadList.ShowAsync();
                        

                        // for (uint j = 0; j < headListJson.Count; j++)
                        
                        foreach (JsonValue headJson in headListJson)
                        {
                            JsonObject headJson2 = new JsonObject();
                            headJson2 = JsonObject.Parse(headJson.ToString());
                            // MessageDialog checkHeadJson = new MessageDialog(headJson.ToString());
                            // await checkHeadJson.ShowAsync();

                            HeadLine tempHead = new HeadLine();
                            
                            tempHead.id = (int)headJson2.GetNamedNumber("id");
                            tempHead.unread = headJson2.GetNamedBoolean("unread");
                            tempHead.marked = headJson2.GetNamedBoolean("marked");
                            tempHead.published = headJson2.GetNamedBoolean("published");
                            tempHead.updated = (int)headJson2.GetNamedNumber("updated");
                            tempHead.is_updated = headJson2.GetNamedBoolean("is_updated");
                            tempHead.title = headJson2.GetNamedString("title");
                            tempHead.link = headJson2.GetNamedString("link");
                            tempHead.feed_id = headJson2.GetNamedString("feed_id");
                            tempHead.feed_title = headJson2.GetNamedString("feed_title");
                            
                            JsonArray tempArrayTags = headJson2.GetNamedArray("tags");
                            /*
                            for (uint i = 0; i < tempArrayTags.Count; i++)
                            {
                                tempHead.tags[i] = tempArrayTags.GetObjectAt(i).GetString();
                            }
                            */
                            // MessageDialog checkTags = new MessageDialog(headJson2.GetNamedString("tags"));
                            // await checkTags.ShowAsync();
                            tempFeed.headline.Add(tempHead);
                        }
                        FeedList.Add(tempFeed);
                        HeadList = tempFeed.headline;
                    }

                    this.ArticlesListView.ItemsSource = HeadList;
                    this.FeedsListView.ItemsSource = FeedList;

                    MessageDialog checkGetFeeds = new MessageDialog(feedListJson.ToString());
                    await checkGetFeeds.ShowAsync();
                    
                    
                    JsonObject logoutContent = await ExecuteMethod2("{\"op\":\"logout\",\"sid\":\"" + session_id + "\"}");
                    // JsonObject logoutContent = logoutContent2.GetNamedObject("content");
                    MessageDialog checkLogout = new MessageDialog("logout" + logoutContent.ToString());
                    await checkLogout.ShowAsync();
                }
            }
            catch(Exception ex)
            {
                MessageDialog GeneralError = new MessageDialog(ex.ToString());
                await GeneralError.ShowAsync();
            }
            
        }

        private async Task<JsonArray> ExecuteMethod(string url)
        {
            HttpStringContent httpLogin = new HttpStringContent(url);
                        
            string jsonString = "booo";
            HttpResponseMessage response = new HttpResponseMessage();

            JsonObject tempJsonArray = new JsonObject();
            // JsonObject tempJsonArray2 = new JsonObject();
            
            response = await client.PostAsync(server, httpLogin);
            jsonString = await response.Content.ReadAsStringAsync();
            return JsonObject.Parse(jsonString).GetNamedArray("content");
        }

        private async Task<JsonObject> ExecuteMethod2(string url)
        {
            HttpStringContent httpLogin = new HttpStringContent(url);

            string jsonString = "booo";
            HttpResponseMessage response = new HttpResponseMessage();

            JsonObject tempJsonArray = new JsonObject();
            // JsonObject tempJsonArray2 = new JsonObject();

            response = await client.PostAsync(server, httpLogin);
            jsonString = await response.Content.ReadAsStringAsync();
            /*
            MessageDialog checkGetFeeds = new MessageDialog(jsonString);
            await checkGetFeeds.ShowAsync();
            */
            if (JsonObject.Parse(jsonString).GetNamedValue("status").ToString() == "0")
            {
                return JsonObject.Parse(jsonString).GetNamedObject("content");
            }
            else
            {
                return JsonObject.Parse(jsonString).GetNamedObject("content");
            }
        }

        /*
        static LoginRequest GetLoginRequest(string userName, string password)
        {
            return new LoginRequest() { user = userName, password = password };
        }
        */
    }
}
