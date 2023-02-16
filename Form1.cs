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

                titleLable.Text = respons.title;
                authorLable.Text = respons.authors[0];
                releaseDateLable.Text = respons.releaseDateTime.ToString("dd'/'MM' - 'yyyy");
                isbnLable.Text = respons.isbn;

                isbnTextBox.Clear();
            }
        }
    }
}
