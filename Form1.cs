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
                    AmazonSearch();
                    break;
                case 1:
                    BookFinderSearch();
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
    }
}
