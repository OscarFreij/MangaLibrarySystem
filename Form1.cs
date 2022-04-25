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

        private async void searchButton_Click(object sender, EventArgs e)
        {
            AmazonRequest.GetBook(isbnTextBox.Text);
        }
    }
}
