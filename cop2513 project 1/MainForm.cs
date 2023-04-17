using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cop2513_project_1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void stockLoaderMenuItem_Click(object sender, EventArgs e)
        {
           Forms.FormStockLoader frm = new Forms.FormStockLoader();
                frm.MdiParent = this;
                frm.Show();
            
        }
    }
}
