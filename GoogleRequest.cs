using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Newtonsoft.Json;
using System.Runtime.InteropServices.ComTypes;

namespace MangaLibrarySystem
{
    public static class GoogleRequest
    {
        public static string baseURL { get; } = "https://www.googleapis.com/books/v1/volumes?q=isbn:";
        private static HttpClient httpClient { get; set; }
        public static async Task<HtmlDocument> GetHtmlAsync(string isbn)
        {
            if (httpClient == null)
            {
                InitHttpClient();
            }

            Uri uri = CreateRequestURI(isbn);
            System.Diagnostics.Debug.WriteLine(uri.ToString());

            try
            {
                //httpClient.DefaultRequestHeaders.ConnectionClose = true;

                //HttpResponseMessage response = await httpClient.GetAsync(uri);
                //response.EnsureSuccessStatusCode();
                //string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                string responseBody = await httpClient.GetStringAsync(uri);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseBody);

                //System.Diagnostics.Debug.WriteLine(doc.DocumentNode.OuterHtml);

                return doc;

            }
            catch (HttpRequestException e)
            {
                System.Diagnostics.Debug.Fail(e.Message);
                return null;
            }
        }

        private static void InitHttpClient()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            if (httpClientHandler.SupportsAutomaticDecompression)
            {
                httpClientHandler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
            }

            httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.64 Safari/537.36 Edg/101.0.1210.53");
            httpClient.DefaultRequestHeaders.Add("ConnectionClose", "true");
        }

        private static Uri CreateRequestURI(string isbn)
        {
            try
            {
                return new Uri(baseURL+isbn);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Fail(e.Message);
                return null;
            }
        }


        private static GoogleRequestRespons GetJson(HtmlDocument doc)
        {
            try
            {
                //HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes($"//*[@id='search']/div[1]/div[1]/div/span[3]/div[2]/*[@data-index='1']");
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                serializerSettings.NullValueHandling = NullValueHandling.Include;

                System.Diagnostics.Debug.WriteLine(doc.DocumentNode);
                GoogleRequestRespons grr = JsonConvert.DeserializeObject<GoogleRequestRespons>(doc.DocumentNode.InnerText, serializerSettings);

                return grr;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public static async Task<GoogleRequestRespons> GetBookDataAsync(string isbn)
        {
            try
            {
                HtmlDocument doc = await GetHtmlAsync(isbn);
                GoogleRequestRespons grr = GetJson(doc);

                return grr;
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to find media by ISBN, please check the ISBN again and search again.", "Invalid ISBN");
                return null;
            }
        }

        public static async Task<System.IO.Stream> GetStreamAsync(Uri uri)
        {
            if (httpClient == null)
            {
                InitHttpClient();
            }

            System.Diagnostics.Debug.WriteLine(uri.ToString());

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (HttpRequestException e)
            {
                System.Diagnostics.Debug.Fail(e.Message);
                return null;
            }
        }

        public class GoogleBook
        {
            private const string DateTimeFormat = "ddMMyyyy";

            public string kind { get; set; }
            public string id { get; set; }
            public string etag { get; set; }
            public Uri selfLink { get; set; }
            public VolumeInfo volumeInfo { get; set; }

            public class VolumeInfo
            {
                public string title { get; set; }
                public string description { get; set; }
                public IList<string> authors { get; set; }
                public string publisher { get; set; }
                public string publishedDate { get; set; }

                public IList<IndustryIdentifier> industryIdentifiers { get; set; }
                public class IndustryIdentifier
                {
                    public string type { get; set; }
                    public string identifier { get; set; }
                }

                public int pageCount { get; set; }
                public string printType { get; set; }
                public IList<string> categories { get; set; }
                public int avrageRating { get; set; }
                public int ratingsCount { get; set; }
                public string maturityRating { get; set; }
                public bool comicsContent { get; set; }
                public ImageLinks imageLinks { get; set; }
                public class ImageLinks
                {
                    public Uri smallThumbnail { get; set; }
                    public Uri thumbnail { get; set; }
                }
                public string language { get; set; }

            }
        }


        public class GoogleRequestRespons
        {
            public string kind { get; set; }
            public int totalItems { get; set; }
            public IList<GoogleBook> items { get; set; }
        }
    }
}