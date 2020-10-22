using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SanwaMarco;

namespace MiniStockerIV
{
    public partial class FormMCType : Form
    {
        public FormMCType()
        {
            InitializeComponent();
        }

        private void FormMCType_Load(object sender, EventArgs e)
        {
            // ComboBox　預設顯示值為 O
            cmbSetMCType.SelectedIndex = 0;
        }

        private void btnSetMCType_Click(object sender, EventArgs e)
        {
            Marco.machineType = (Marco.MachineType)cmbSetMCType.SelectedIndex;
            Close();
        }
    }
}
