using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace MangaLibrarySystem
{
    internal class BookFinderRequest
    {
        public static string baseURL { get; } = "https://www.bookfinder.com/isbn/";
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
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.64 Safari/537.36 Edg/101.0.1210.53");
            //httpClient.DefaultRequestHeaders.Add("ConnectionClose", "true");
        }

        private static Uri CreateRequestURI(string isbn)
        {
            return new Uri(baseURL+isbn+"/");
        }


        private static HtmlNodeCollection GetNodes(HtmlDocument doc)
        {
            try
            {
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes($"//*[@id=\"catalog\"]");
                return nodes;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        private static Uri GetImageUri(HtmlNode baseNode)
        {
            try
            {
                string src = String.Empty;

                HtmlAttributeCollection attributeList = baseNode.SelectSingleNode($"//*[@id=\"header-img\"]/img").Attributes;

                foreach (HtmlAttribute attribute in attributeList)
                {
                    if (attribute.Name == "src")
                    {
                        src = attribute.Value;
                        break;
                    }
                }

                Uri imageUri = new Uri(src);
                return imageUri;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        private static string GetTitle(HtmlNode baseNode)
        {
            try
            {
                string title = string.Empty;

                title = baseNode.SelectSingleNode($"//*[@id=\"book-info\"]/h1").InnerText.Trim();

                return title;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        private static string GetByLine(HtmlNode baseNode)
        {
            try
            {
                string byLine = string.Empty;
                byLine = baseNode.SelectSingleNode($"//*[@id=\"book-info\"]/div[1]").InnerText.Trim().Replace('\n', ' ').Remove(0, byLine.IndexOf("by") + 3).Trim();
                return byLine;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        private static string GetPublisher(HtmlNode baseNode)
        {
            try
            {
                string publisher = string.Empty;

                publisher = baseNode.SelectSingleNode($"//*[@id=\"book-info\"]/div[4]").InnerText;               
                publisher = publisher.Remove(0, publisher.IndexOf("Publisher:")+ "Publisher:".Length);
                publisher = publisher.Remove(publisher.IndexOf("Edition:"));

                string[] tempArray = publisher.Split('\n');
                publisher = string.Empty;
                foreach (string item in tempArray)
                {
                    publisher += item.Trim();
                }

                publisher = publisher.Insert(publisher.IndexOf(',') + 1, " ");

                return publisher;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        private static string GetEdition(HtmlNode baseNode)
        {
            try
            {
                string edition = string.Empty;

                edition = baseNode.SelectSingleNode($"//*[@id=\"book-info\"]/div[4]").InnerText;
                edition = edition.Remove(0, edition.IndexOf("Edition:") + "Edition:".Length);
                edition = edition.Remove(edition.IndexOf("Language:"));

                string[] tempArray = edition.Split('\n');
                edition = string.Empty;
                foreach (string item in tempArray)
                {
                    edition += item.Trim();
                }

                return edition;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        private static string GetLanguage(HtmlNode baseNode)
        {
            try
            {
                string language = string.Empty;

                language = baseNode.SelectSingleNode($"//*[@id=\"book-info\"]/div[4]").InnerText;
                language = language.Remove(0, language.IndexOf("Language:") + "Language:".Length);

                string[] tempArray = language.Split('\n');
                language = string.Empty;
                foreach (string item in tempArray)
                {
                    language += item.Trim();
                }

                return language;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public static async Task<BookFinderRequestRespons> GetBookDataAsync(string isbn)
        {
            HtmlDocument doc = await GetHtmlAsync(isbn);
            try
            {
                HtmlNodeCollection nodes = GetNodes(doc);
                return new BookFinderRequestRespons(
                    isbn,
                    GetImageUri(nodes[0]),
                    GetTitle(nodes[0]),
                    new string[] { GetByLine(nodes[0]) },
                    GetPublisher(nodes[0]),
                    GetEdition(nodes[0]),
                    GetLanguage(nodes[0])
                );
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

        public class BookFinderRequestRespons
        {
            private const string DateTimeFormat = "ddMMyyyy";

            public string isbn { get; }
            public Uri imageUri { get; }
            public string title { get; }
            public string[] authors { get; }
            public string publisher { get; }
            public string edition { get; }
            public string language { get; }

            public BookFinderRequestRespons(string isbn, Uri imageUri, string title, string[] authors, string publisher, string edition, string language)
            {
                this.isbn = isbn;
                this.imageUri = imageUri;
                this.title = title;
                this.authors = authors;
                this.publisher = publisher;
                this.edition = edition;
                this.language = language;
            }

            public override string ToString()
            {
                string authorsString = String.Empty;

                for (int i = 0; i < authors.Length; i++)
                {
                    authorsString += $"{i}|{authors[i]}";
                    if (i < authors.Length - 1)
                    {
                        authorsString += ",";
                    }
                }

                return $"isbn: {this.isbn}; title: {this.title}; imageUri: {this.imageUri.ToString()}; authors: {authorsString}; publisher: {this.publisher}; edition: {this.edition}; language: {this.language}";
            }
        }
    }
}
