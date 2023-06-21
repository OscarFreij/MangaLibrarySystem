using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MangaLibrarySystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void CheckEnterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                Search();
            }
        }
        private void searchButton_Click(object sender, EventArgs e)
        {
            Search();
        }

        private async void Search()
        {
            int engineId = comboBox1.SelectedIndex;

            if (engineId == -1)
            {
                MessageBox.Show("Missing Search Engine!", "Please select a search engine in the dropdown");
                return;
            }

            switch (engineId)
            {
                case 0:
                    AmazonGoogleSearch(); //Default - Cover from Amazon and Text from Google.
                    break;
                case 1:
                    AmazonSearch(); //Amazon Only
                    break;
                case 2:
                    BookFinderSearch(); //BookFinder Only
                    break;
                case 3:
                    GoogleSearch(); //Google Only
                    break;
                default:
                    MessageBox.Show("Search Engine not found!", "Invalid engine selection");
                    break;
            }

            
        }

        private async void AmazonSearch()
        {
            if (isbnTextBox.Text.Length == 0)
            {
                MessageBox.Show("ISBN missing. Please fill in ISBN number and try again!", "Missing ISBN!");
                return;
            }

            AmazonRequest.AmazonRequestRespons respons = await AmazonRequest.GetBookDataAsync(isbnTextBox.Text);
            if (respons != null)
            {
                System.Diagnostics.Debug.WriteLine(respons);
                pictureBox1.ImageLocation = respons.imageUri.ToString();

                richTextBoxTitle.Text = respons.title;
                richTextBoxDetails.Clear();
                richTextBoxDetails.Text += "Author: " + respons.authors[0];
                richTextBoxDetails.Text += "\nRelease date: " + respons.releaseDateTime.ToString("dd'/'MM' - 'yyyy");
                richTextBoxDetails.Text += "\nISBN: " + respons.isbn;

                isbnTextBox.Clear();
            }
        }

        private async void BookFinderSearch()
        {
            if (isbnTextBox.Text.Length == 0)
            {
                MessageBox.Show("ISBN missing. Please fill in ISBN number and try again!", "Missing ISBN!");
                return;
            }

            BookFinderRequest.BookFinderRequestRespons respons = await BookFinderRequest.GetBookDataAsync(isbnTextBox.Text);
            if (respons != null)
            {
                System.Diagnostics.Debug.WriteLine(respons);
                pictureBox1.ImageLocation = respons.imageUri.ToString();

                richTextBoxTitle.Text = respons.title;
                richTextBoxDetails.Clear();
                richTextBoxDetails.Text += "Author: " + respons.authors[0];
                richTextBoxDetails.Text += "\nPublisher: " + respons.publisher;
                richTextBoxDetails.Text += "\nEdition: " + respons.edition;
                richTextBoxDetails.Text += "\nLanguage: " + respons.language;
                richTextBoxDetails.Text += "\nISBN: " + respons.isbn;

                isbnTextBox.Clear();
            }
        }

        private async void GoogleSearch()
        {
            if (isbnTextBox.Text.Length == 0)
            {
                MessageBox.Show("ISBN missing. Please fill in ISBN number and try again!", "Missing ISBN!");
                return;
            }

            GoogleRequest.GoogleRequestRespons respons = await GoogleRequest.GetBookDataAsync(isbnTextBox.Text);
            if (respons != null)
            {
                System.Diagnostics.Debug.WriteLine(respons);
                pictureBox1.ImageLocation = respons.items[0].volumeInfo.imageLinks.thumbnail.ToString();

                richTextBoxTitle.Text = respons.items[0].volumeInfo.title;
                richTextBoxDetails.Clear();
                richTextBoxDetails.Text += "Author: ";
                foreach (var author in respons.items[0].volumeInfo.authors)
                {
                    richTextBoxDetails.Text += author + ", ";
                }
                richTextBoxDetails.Text = richTextBoxDetails.Text.Substring(0, richTextBoxDetails.Text.Length - 2);

                richTextBoxDetails.Text += "\nPublished date: " + respons.items[0].volumeInfo.publishedDate;
                richTextBoxDetails.Text += "\nPublisher: " + respons.items[0].volumeInfo.publisher;
                richTextBoxDetails.Text += "\nMaturity Rating: " + respons.items[0].volumeInfo.maturityRating;

                richTextBoxDetails.Text += "\nDescription: " + respons.items[0].volumeInfo.description;

                foreach (var isbn in respons.items[0].volumeInfo.industryIdentifiers)
                {
                    richTextBoxDetails.Text += "\n" + isbn.type + ": " + isbn.identifier;
                }

                isbnTextBox.Clear();
            }
        }

        private async void AmazonGoogleSearch()
        {
            if (isbnTextBox.Text.Length == 0)
            {
                MessageBox.Show("ISBN missing. Please fill in ISBN number and try again!", "Missing ISBN!");
                return;
            }
            
            GoogleRequest.GoogleRequestRespons respons = await GoogleRequest.GetBookDataAsync(isbnTextBox.Text);
            AmazonRequest.AmazonRequestSlimRespons respons2 = await AmazonRequest.GetBookDataSlimAsync(isbnTextBox.Text); //Slim only returns cover image.
            if (respons != null)
            {
                System.Diagnostics.Debug.WriteLine(respons);

                if (respons2 != null)
                {
                    pictureBox1.ImageLocation = respons2.imageUri.ToString();
                }
                else
                {
                    pictureBox1.ImageLocation = respons.items[0].volumeInfo.imageLinks.thumbnail.ToString();
                }

                richTextBoxTitle.Text = respons.items[0].volumeInfo.title;
                richTextBoxDetails.Clear();
                richTextBoxDetails.Text += "Author: ";
                foreach (var author in respons.items[0].volumeInfo.authors)
                {
                    richTextBoxDetails.Text += author + ", ";
                }
                richTextBoxDetails.Text = richTextBoxDetails.Text.Substring(0, richTextBoxDetails.Text.Length - 2);

                richTextBoxDetails.Text += "\nPublished date: " + respons.items[0].volumeInfo.publishedDate;
                richTextBoxDetails.Text += "\nPublisher: " + respons.items[0].volumeInfo.publisher;
                richTextBoxDetails.Text += "\nMaturity Rating: " + respons.items[0].volumeInfo.maturityRating;

                richTextBoxDetails.Text += "\nDescription: " + respons.items[0].volumeInfo.description;

                foreach (var isbn in respons.items[0].volumeInfo.industryIdentifiers)
                {
                    richTextBoxDetails.Text += "\n" + isbn.type + ": " + isbn.identifier;
                }

                isbnTextBox.Clear();
            }
        }
    }
}
