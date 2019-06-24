using MiniStockerIV.UI_Update;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniStockerIV
{
    public partial class FormAuto : Form
    {
        public FormAuto()
        {
            InitializeComponent();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            FormMainUpdate.SetFormEnable("FormMain", true);
            //tbMsg.Text = "";
            //this.Hide();
            this.Close();
        }

        private void FormAuto_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainUpdate.SetFormEnable("FormMain", true);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Pause!!");
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Continue!!");
        }
    }
}
