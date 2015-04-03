using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Net;
using System.IO;
using System.Text;
using System.Data;

namespace GoogleNews_API
{
    public partial class News : System.Web.UI.Page
    {
        /// <summary>
        /// Page Load Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [WebMethod]
        public static NewsArticle[] GetNewsArticles(string topic)
        {
            // Initialize the List of News Articles
            List<NewsArticle> articles = new List<NewsArticle>();

            // Initialize the Google News RSS Feed request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://news.google.com/news?q=" + topic + "&output=rss");

            // Specify GET Method Request
            request.Method = "GET";

            // Perform the Request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check the Response State Code for Success
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                // Check the Character Set of the Response
                if (response.CharacterSet == "")
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                // Convert the Stream in a JSON string
                string data = readStream.ReadToEnd();

                // Initialize a DataSet to store the parsed information
                DataSet ds = new DataSet();
                StringReader reader = new StringReader(data);
                ds.ReadXml(reader);
                DataTable articleTable = new DataTable();

                // Make sure the response is not empty
                if (ds.Tables.Count > 0)
                {
                    articleTable = ds.Tables["item"];

                    // Loop through each RSS element and store it as a NewsArticle
                    foreach (DataRow row in articleTable.Rows)
                    {
                        NewsArticle article = new NewsArticle();
                        article.id = row["item_id"].ToString(); 
                        article.title = row["title"].ToString();
                        article.url = row["link"].ToString();
                        article.date = row["pubDate"].ToString();
                        article.description = row["description"].ToString();
                        articles.Add(article);
                    }
                }
            }

            return articles.ToArray();
        }

        public class NewsArticle
        {
            public string title { get; set; }
            public string url { get; set; }
            public string date { get; set; }
            public string id { get; set; }
            public string description { get; set; }
        }
    }
}