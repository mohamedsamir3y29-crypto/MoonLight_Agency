using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoonLight_Agency
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TripForm frm = new TripForm();
            frm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CustomerForm frm = new CustomerForm();
            frm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); // ده بيقفل البرنامج وكل عملياته من الجذور
        }
    }
}
