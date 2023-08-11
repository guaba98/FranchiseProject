using System;
using System.Windows.Forms;

namespace FranchiseProject
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void ScreenTransition(object sender, MouseEventArgs e)
        {
            MainForm MainForm = new MainForm();
            MainForm.Show();
            this.Hide();
        }

        private void pictureboxClicked(object sender, EventArgs e)
        {
            MainForm MainForm = new MainForm();
            MainForm.Show();
            this.Hide();
        }
    }
}
