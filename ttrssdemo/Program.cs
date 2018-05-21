namespace ttrssdemo
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Web.Script.Serialization;
    using ttrssclient.dto;
    using ttrssclient.requests;
    using ttrssclient.responses;


    class Program
    {
        static WebClient webClient = new WebClient();
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        static void Main(string[] args)
        {
            /*System.Net.HttpWebRequest adds the header 'HTTP header "Expect: 100-Continue"' to every request unless you explicitly ask it not to by setting this static property to false:*/
            System.Net.ServicePointManager.Expect100Continue = false;
            /*
            var server = ConfigurationManager.AppSettings["http://192.168.100.9/tt-rss/api/"];
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            */
            /*
            string server = "http://example.com/tt-rss/api/";
            string username = "user";
            string password = "password";
            */
            
            Console.Write("Url : ");
            string server = Console.ReadLine();
            Console.Write("User : ");
            string username = Console.ReadLine();
            Console.Write("Password : ");
            string password = Console.ReadLine();

            Console.WriteLine(serializer.Serialize(GetLoginRequest(username, password)));

            string response = webClient.UploadString(server, serializer.Serialize(GetLoginRequest(username, password)));
            Response<LoginResponse> loginResponse = serializer.Deserialize<Response<LoginResponse>>(response);

            string session_id = loginResponse.content.session_id;

            Console.WriteLine("successfully logged in, session_id: " + session_id);

            GetUnreadRequest getUnreadReqeust = new GetUnreadRequest(session_id);
            int unread = ExecuteMethod<GetUnreadRequest, GetUnreadResponse>(getUnreadReqeust, server).unread;
            Console.WriteLine("unread number of articles: " + unread);

            GetCountersRequest getCountersRequest = new GetCountersRequest(session_id, "l");
            Counter[] counters = ExecuteMethod<GetCountersRequest, Counter[]>(getCountersRequest, server);
            Console.WriteLine(counters);

            Console.WriteLine("Categories: ");
            Category[] categories = ExecuteMethod<GetCategoriesRequest, Category[]>(new GetCategoriesRequest(session_id, true, false, false), server);
            foreach (Category categroy in categories)
            {
                Console.WriteLine(categroy.title + ": " + categroy.unread);
            }
            Console.WriteLine();

            Showfeed(session_id, server);

            Console.WriteLine();

            LogoutRequest logoutObject = new LogoutRequest(session_id);
            LogoutResponse logoutResponse = ExecuteMethod<LogoutRequest, LogoutResponse>(logoutObject, server);
            Console.WriteLine(logoutResponse.status);

            Console.ReadKey();




            Console.WriteLine("press the any key");
            Console.ReadKey();
        }

        static LoginRequest GetLoginRequest(string userName, string password)
        {
            return new LoginRequest() { user = userName, password = password };
        }

        static U ExecuteMethod<T, U>(T Request, string url)
        {
            Console.WriteLine(serializer.Serialize(Request));
            string response = webClient.UploadString(url, serializer.Serialize(Request));
            return serializer.Deserialize<Response<U>>(response).content;
        }

        static void Showfeed(string session_id, string server)
        {
            Console.WriteLine("Feeds: ");
            Feed[] feeds = ExecuteMethod<GetFeedsRequest, Feed[]>(new GetFeedsRequest(session_id, -3, true, true), server);
            foreach (Feed feed in feeds)
            {
                Console.WriteLine(feed.title + ": " + feed.unread);

                Console.WriteLine("HeadLines of Feed" + " " + feed.id);
                HeadLine[] headLines = ExecuteMethod<GetHeadLinesRequest, HeadLine[]>(new GetHeadLinesRequest(session_id, Int32.Parse(feed.id)), server);
                foreach (HeadLine head in headLines)
                {
                    Console.WriteLine(head.id + " " + head.title);
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
