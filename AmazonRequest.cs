using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MangaLibrarySystem
{
    public static class AmazonRequest
    {
        public static string baseURL { get; } = "https://www.amazon.com/s?k=&i=stripbooks";
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
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

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
        }

        private static Uri CreateRequestURI(string isbn)
        {
            return new Uri(baseURL.Insert(baseURL.IndexOf('=')+1, isbn));
        }


        private static HtmlNodeCollection GetNodes(HtmlDocument doc)
        {
            try
            {
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes($"//*[@id='search']/div[1]/div[1]/div/span[3]/div[2]/*[@data-index='1']");
                return nodes;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Fail(e.Message);
                return null;
            }
        }

        private static Uri GetImageUri(HtmlNode baseNode)
        {
            try
            {
                string srcset = String.Empty;

                HtmlAttributeCollection attributeList = baseNode.SelectSingleNode($"//div[@class='sg-row']//img/@srcset").Attributes;

                foreach (HtmlAttribute attribute in attributeList)
                {
                    if (attribute.Name == "srcset")
                    {
                        srcset = attribute.Value;
                        break;
                    }
                }

                string[] srcsetArray = srcset.Split(',');
                string imageSrc = srcsetArray[srcsetArray.Length - 1];
                imageSrc = imageSrc.TrimStart();
                imageSrc = imageSrc.Remove(imageSrc.IndexOf(' '));
                imageSrc = imageSrc.TrimEnd();

                Uri imageUri = new Uri(imageSrc);
                return imageUri;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Fail(e.Message);
                return null;
            }
        }

        private static string GetTitle(HtmlNode baseNode)
        {
            try
            {
                string title = string.Empty;

                title = baseNode.SelectSingleNode($"//*[@id='search']/div[1]/div[1]/div/span[3]/div[2]/div[2]//div[@class='sg-row']/*[2]//h2").InnerText;
                
                return title;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Fail(e.Message);
                return null;
            }
        }

        private static string GetByLine(HtmlNode baseNode)
        {
            try
            {
                string byLine = string.Empty;

                byLine = baseNode.SelectSingleNode($"//*[@id='search']/div[1]/div[1]/div/span[3]/div[2]/div[2]//div[@class='sg-row']/*[2]//div[@class='a-row']").InnerText;

                byLine = byLine.Remove(0, byLine.IndexOf("by ")+3);

                return byLine;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Fail(e.Message);
                return null;
            }
        }

        public static async Task<AmazonRequestRespons> GetBookDataAsync(string isbn)
        {
            HtmlDocument doc = await GetHtmlAsync(isbn);
            HtmlNodeCollection nodes = GetNodes(doc);
            string[] byLineSplit = GetByLine(nodes[0]).Split('|');

            return new AmazonRequestRespons(isbn, GetImageUri(nodes[0]), GetTitle(nodes[0]), new string[] { byLineSplit[0] }, byLineSplit[1]);
        }

        public class AmazonRequestRespons
        {
            private const string DateTimeFormat = "dd/MM/yyyy";

            public string isbn { get; }
            public Uri imageUri { get; }
            public string title { get; }
            public string[] authors { get; }
            public DateTime releaseDateTime { get; }

            public AmazonRequestRespons(string isbn, Uri imageUri, string title, string[] authors, string releaseDateTimeString)
            {
                this.isbn = isbn;
                this.imageUri = imageUri;
                this.title = title;
                this.authors = authors;
                this.releaseDateTime = DateTime.Parse(releaseDateTimeString);
            }

            public override string ToString()
            {
                string authorsString = String.Empty;

                for (int i = 0; i < authors.Length; i++)
                {
                    authorsString += $"{i}|{authors[i]}";
                    if (i < authors.Length-1)
                    {
                        authorsString += ",";
                    }
                }

                return $"isbn: {this.isbn}; title: {this.title}; imageUri: {this.imageUri.ToString()}; authors: {authorsString}; releaseDateTime: {this.releaseDateTime.ToString(DateTimeFormat)}";
            }
        }
    }
}