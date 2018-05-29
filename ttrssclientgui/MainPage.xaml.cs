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
using System.ComponentModel;
using System.Runtime.CompilerServices;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ttrssclientgui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            this.OnPropertyChanged(propertyName);

            return true;
        }

        protected bool SetProperty([CallerMemberName]string propertyName = null)
        {
            this.OnPropertyChanged(propertyName);

            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        Uri server = new Uri("http://example/tt-rss/api/");

        private string username = "user";
        private string password = "password";

        HttpClient client = new HttpClient();

        // public string session_id = "test";

        // private ObservableCollection<Feed> feedList = new ObservableCollection<Feed>();
        public ObservableCollection<Feed> FeedList;
        public ObservableCollection<Feed> FeedList2;
        /*
        {
            get
            {
                return this.feedList;
            }
            set
            {
                SetProperty<ObservableCollection<Feed>>(ref feedList, value);
            }
        }
        */
        // private Feed selectedFeed = new Feed();
        public Feed SelectedFeed;
        /*
        {
            get
            {
                return this.selectedFeed;
            }
            set
            {
                SetProperty<Feed>(ref selectedFeed, value);
            }
        }
        */
        // private ObservableCollection<HeadLine> headList = new ObservableCollection<HeadLine>();
        public ObservableCollection<HeadLine> HeadList;
        public ObservableCollection<ObservableCollection<HeadLine>> ListHeadList;
        /*
        {
            get
            {
                return this.headList;
            }
            set
            {
                SetProperty<ObservableCollection<HeadLine>>(ref headList, value);
            }
        }
        */
        public MainPage()
        {
            this.InitializeComponent();
            FeedList = new ObservableCollection<Feed>();
            FeedList2 = new ObservableCollection<Feed>();
            HeadList = new ObservableCollection<HeadLine>();
            ListHeadList = new ObservableCollection<ObservableCollection<HeadLine>>();
            SelectedFeed = new Feed();
            SelectedFeed.headline = new ObservableCollection<HeadLine>();
            // this.DataContext = this;

            mainWorkAsync();

            this.ArticlesListView.ItemsSource = SelectedFeed.headline;
            this.FeedsListView.ItemsSource = FeedList;
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
                    
                    for (int i = 0; i < feedListJson.Count; i++)
                    // foreach (JsonValue feedJson in feedListJson)
                    {
                        JsonObject feedJson2 = JsonObject.Parse(feedListJson[i].ToString());

                        
                        MessageDialog checkFeedJson = new MessageDialog(feedJson2.ToString());
                        await checkFeedJson.ShowAsync();
                        

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
                        
                        JsonArray headListJson = await ExecuteMethod("{\"op\":\"getHeadlines\",\"sid\":\"" + session_id + "\",\"feed_id\":\"" + tempFeed.id + "\",\"view_mode\":\"" + "unread\"}");
                        
                        // for (int j = 0; j < headListJson.Count; j++)
                        foreach (JsonValue headJson in headListJson)
                        {
                            JsonObject headJson2 = new JsonObject();
                            headJson2 = JsonObject.Parse(headJson.ToString());
                            // MessageDialog checkHeadJson = new MessageDialog(headJson.ToString());
                            // await checkHeadJson.ShowAsync();

                            HeadLine tempHead = new HeadLine();

                            tempFeed.headline.Add(tempHead);
                            /*
                            tempFeed.headline[j].id = (int)headJson2.GetNamedNumber("id");
                            tempFeed.headline[j].unread = headJson2.GetNamedBoolean("unread");
                            tempFeed.headline[j].marked = headJson2.GetNamedBoolean("marked");
                            tempFeed.headline[j].published = headJson2.GetNamedBoolean("published");
                            tempFeed.headline[j].updated = (int)headJson2.GetNamedNumber("updated");
                            tempFeed.headline[j].is_updated = headJson2.GetNamedBoolean("is_updated");
                            tempFeed.headline[j].title = headJson2.GetNamedString("title");
                            tempFeed.headline[j].link = headJson2.GetNamedString("link");
                            tempFeed.headline[j].feed_id = headJson2.GetNamedString("feed_id");
                            tempFeed.headline[j].feed_title = headJson2.GetNamedString("feed_title");
                            */
                            
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
                            
                            // JsonArray tempArrayTags = headJson2.GetNamedArray("tags");
                            /*
                            for (uint i = 0; i < tempArrayTags.Count; i++)
                            {
                                tempHead.tags[i] = tempArrayTags.GetObjectAt(i).GetString();
                            }
                            */
                            // MessageDialog checkTags = new MessageDialog(headJson2.GetNamedString("tags"));
                            // await checkTags.ShowAsync();
                            tempFeed.headline.Add(tempHead);
                            HeadList.Add(tempHead);
                        }
                        FeedList.Add(tempFeed);
                        FeedList[i].headline = new ObservableCollection<HeadLine>();
                        MessageDialog checkCount = new MessageDialog(HeadList.Count.ToString());
                        await checkCount.ShowAsync();
                        /*
                        for (int l = 0; l < HeadList.Count; l++)
                        {
                            FeedList[i].headline.Add(HeadList[l]);
                        }
                        */
                        ObservableCollection<HeadLine> headListForAdd = new ObservableCollection<HeadLine>();
                        foreach (HeadLine h in HeadList)
                        {
                            headListForAdd.Add(h);
                        }
                        ListHeadList.Add(headListForAdd);
                        for (int k = 0; k < HeadList.Count; k++)
                        {
                            SelectedFeed.headline.Add(HeadList[k]);
                        }
                        HeadList.Clear();
                    }
                    
                    MessageDialog checkGetFeeds = new MessageDialog(feedListJson.ToString());
                    await checkGetFeeds.ShowAsync();
                    
                    
                    JsonObject logoutContent = await ExecuteMethod2("{\"op\":\"logout\",\"sid\":\"" + session_id + "\"}");
                    
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
        
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (splitView.IsPaneOpen)
            {
                splitView.IsPaneOpen = false;
            }
            else
            {
                splitView.IsPaneOpen = true;
            }
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // selectFeedItem();

        }

        private void FeedsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectFeedItem();
        }

        private void FeedsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // selectFeedItem();
        }

        private async void selectFeedItem()
        {
            BindingExpression articleBinding = ArticlesListView.GetBindingExpression(ListView.ItemsSourceProperty);
            int tempIndex = FeedsListView.SelectedIndex;
            // MessageDialog checkFeed = new MessageDialog(tempIndex.ToString());
            // await checkFeed.ShowAsync();
            // SelectedFeed = ListHeadList;
            // SelectedFeed.headline = FeedList.ElementAt(tempIndex).headline;
            
            SelectedFeed.headline.Clear();

            // articleBinding.UpdateSource();
            MessageDialog checkCount = new MessageDialog(ListHeadList[tempIndex].Count.ToString());
            await checkCount.ShowAsync();
            for (int i=0; i < ListHeadList[tempIndex].Count; i++)
            {
                // MessageDialog checkFeed = new MessageDialog((ListHeadList[tempIndex])[i].title.ToString());
                // await checkFeed.ShowAsync();
                SelectedFeed.headline.Add((ListHeadList[tempIndex])[i]);
            }
            
            // ArticlesListView.ItemsSource = SelectedFeed.headline;
            articleBinding.UpdateSource();
        }
        /*
        static LoginRequest GetLoginRequest(string userName, string password)
        {
            return new LoginRequest() { user = userName, password = password };
        }
        */
    }
}
