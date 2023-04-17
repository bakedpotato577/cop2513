using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace cop2513_project_1
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();
            stockChart.Dock = DockStyle.Fill; // Set the chart to fill the entire form
        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }

        public void SetChartData(DataView dv)
        {
            stockChart.DataSource = dv;

            stockChart.Series.Clear(); // Clear any previously loaded data

            // Bind data to chart
            Series series_candlestick = new Series("Candlestick");

            series_candlestick.ChartType = SeriesChartType.Candlestick;
            series_candlestick.XValueMember = "Date";
            series_candlestick.YValuesPerPoint = 4;
            series_candlestick.YValueMembers = "High,Low,Open,Close";
            series_candlestick.XValueType = ChartValueType.Date;
            series_candlestick.CustomProperties = "PriceDownColor=Red, PriceUpColor=Green";
            series_candlestick["OpenCloseStyle"] = "Triangle";
            series_candlestick["ShowOpenClose"] = "Both";
            stockChart.Series.Add(series_candlestick);

            // Hide X-axis grid lines
            stockChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;

            // Hide Y-axis grid lines
            stockChart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            stockChart.DataSource = dv;
        }
    }
}
