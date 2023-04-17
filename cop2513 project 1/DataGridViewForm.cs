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
    public partial class DataGridViewForm : Form
    {
        public DataGridViewForm()
        {
            InitializeComponent();
            stockDataGridView.Dock = DockStyle.Fill; // Set the DataGridView to fill the entire form
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        public void SetDataGridViewData(DataView dv)
        {
            stockDataGridView.DataSource = null; // To clear the previous data before adding the new ones
            stockDataGridView.DataSource = dv;
        }
    }
}
