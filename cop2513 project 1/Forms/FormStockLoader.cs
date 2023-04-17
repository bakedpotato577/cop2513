using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

//Form for loading stock data from a CSV file and displaying it in a data grid view and chart.

namespace cop2513_project_1.Forms
{
    public partial class FormStockLoader : Form
    {
        public FormStockLoader()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private bool IsDoji(DataRow row)
        {
            double open = Convert.ToDouble(row["Open"]);
            double close = Convert.ToDouble(row["Close"]);

            // A Doji is characterized by a small difference between the open and close prices.
            // You can use a threshold (e.g., 0.01) to determine if the difference is small enough.
            return Math.Abs(open - close) <= 0.01;
        }

        private bool IsHammer(DataRow row)
        {
            double open = Convert.ToDouble(row["Open"]);
            double high = Convert.ToDouble(row["High"]);
            double low = Convert.ToDouble(row["Low"]);
            double close = Convert.ToDouble(row["Close"]);

            double bodySize = Math.Abs(open - close);
            double upperWickSize = high - Math.Max(open, close);
            double lowerWickSize = Math.Min(open, close) - low;

            // A Hammer is characterized by a small body, a small upper wick, and a long lower wick.
            // You can use ratios (e.g., 2 or 3) to determine if the wick sizes are long/short enough.
            return bodySize <= lowerWickSize * 0.5 && upperWickSize <= bodySize * 0.5 && lowerWickSize >= bodySize * 2;
        }
        private bool IsInvertedHammer(DataRow row)
        {
            double open = Convert.ToDouble(row["Open"]);
            double high = Convert.ToDouble(row["High"]);
            double low = Convert.ToDouble(row["Low"]);
            double close = Convert.ToDouble(row["Close"]);

            double bodySize = Math.Abs(open - close);
            double upperWickSize = high - Math.Max(open, close);
            double lowerWickSize = Math.Min(open, close) - low;

            return bodySize <= upperWickSize * 0.5 && lowerWickSize <= bodySize * 0.5 && upperWickSize >= bodySize * 2;
        }

        private bool IsGravestoneDoji(DataRow row)
        {
            double open = Convert.ToDouble(row["Open"]);
            double high = Convert.ToDouble(row["High"]);
            double low = Convert.ToDouble(row["Low"]);
            double close = Convert.ToDouble(row["Close"]);

            double bodySize = Math.Abs(open - close);
            double upperWickSize = high - Math.Max(open, close);
            double lowerWickSize = Math.Min(open, close) - low;

            return bodySize <= upperWickSize * 0.1 && lowerWickSize <= bodySize * 0.1 && upperWickSize >= bodySize * 3;
        }

        private bool IsDragonflyDoji(DataRow row)
        {
            double open = Convert.ToDouble(row["Open"]);
            double high = Convert.ToDouble(row["High"]);
            double low = Convert.ToDouble(row["Low"]);
            double close = Convert.ToDouble(row["Close"]);

            double bodySize = Math.Abs(open - close);
            double upperWickSize = high - Math.Max(open, close);
            double lowerWickSize = Math.Min(open, close) - low;

            return bodySize <= lowerWickSize * 0.1 && upperWickSize <= bodySize * 0.1 && lowerWickSize >= bodySize * 3;
        }

        private bool IsLongLeggedDoji(DataRow row)
        {
            double open = Convert.ToDouble(row["Open"]);
            double high = Convert.ToDouble(row["High"]);
            double low = Convert.ToDouble(row["Low"]);
            double close = Convert.ToDouble(row["Close"]);

            double bodySize = Math.Abs(open - close);
            double upperWickSize = high - Math.Max(open, close);
            double lowerWickSize = Math.Min(open, close) - low;

            return bodySize <= upperWickSize * 0.1 && bodySize <= lowerWickSize * 0.1 && upperWickSize >= bodySize * 3 && lowerWickSize >= bodySize * 3;
        }

        private bool IsWhiteMarubozu(DataRow row)
        {
            double open = Convert.ToDouble(row["Open"]);
            double high = Convert.ToDouble(row["High"]);
            double low = Convert.ToDouble(row["Low"]);
            double close = Convert.ToDouble(row["Close"]);

            return open == low && close == high && open < close;
        }

        private bool IsBlackMarubozu(DataRow row)
        {
            double open = Convert.ToDouble(row["Open"]);
            double high = Convert.ToDouble(row["High"]);
            double low = Convert.ToDouble(row["Low"]);
            double close = Convert.ToDouble(row["Close"]);

            return open == high && close == low && open > close;
        }

        private bool IsBullishEngulfing(DataRow firstRow, DataRow secondRow)
        {
            double firstOpen = Convert.ToDouble(firstRow["Open"]);
            double firstHigh = Convert.ToDouble(firstRow["High"]);
            double firstLow = Convert.ToDouble(firstRow["Low"]);
            double firstClose = Convert.ToDouble(firstRow["Close"]);

            double secondOpen = Convert.ToDouble(secondRow["Open"]);
            double secondHigh = Convert.ToDouble(secondRow["High"]);
            double secondLow = Convert.ToDouble(secondRow["Low"]);
            double secondClose = Convert.ToDouble(secondRow["Close"]);

            // Check if the first candle is bearish and the second candle is bullish
            if (firstClose < firstOpen && secondClose > secondOpen)
            {
                // Check if the second candle's body engulfs the first candle's body
                if (secondOpen < firstClose && secondClose > firstOpen)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsBearishEngulfing(DataRow firstRow, DataRow secondRow)
        {
            double firstOpen = Convert.ToDouble(firstRow["Open"]);
            double firstHigh = Convert.ToDouble(firstRow["High"]);
            double firstLow = Convert.ToDouble(firstRow["Low"]);
            double firstClose = Convert.ToDouble(firstRow["Close"]);

            double secondOpen = Convert.ToDouble(secondRow["Open"]);
            double secondHigh = Convert.ToDouble(secondRow["High"]);
            double secondLow = Convert.ToDouble(secondRow["Low"]);
            double secondClose = Convert.ToDouble(secondRow["Close"]);

            // Check if the first candle is bullish and the second candle is bearish
            if (firstClose > firstOpen && secondClose < secondOpen)
            {
                // Check if the second candle's body engulfs the first candle's body
                if (secondOpen > firstClose && secondClose < firstOpen)
                {
                    return true;
                }
            }
            return false;
        }
        private void HighlightPattern(Func<DataRow, bool> patternDetector, Color markerColor)
        {
            stockChart.Series[0].Points.Clear();

            for (int i = 0; i < dv.Count; i++)
            {
                DataRow row = dv[i].Row;
                if (patternDetector(row))
                {
                    stockChart.Series[0].Points.AddXY(row["Date"], row["High"], row["Low"], row["Open"], row["Close"]);
                    stockChart.Series[0].Points.Last().MarkerStyle = MarkerStyle.Star5;
                    stockChart.Series[0].Points.Last().MarkerColor = markerColor;
                    stockChart.Series[0].Points.Last().MarkerSize = 8;
                }
            }
        }

        private void patternsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPattern = patternComboBox.SelectedItem.ToString();
if (selectedPattern == "All Patterns")
    {
        HighlightPattern(IsDoji, Color.Blue);
        HighlightPattern(IsGravestoneDoji, Color.Magenta);
        HighlightPattern(IsDragonflyDoji, Color.Orange);
        HighlightPattern(IsLongLeggedDoji, Color.Brown);
        HighlightPattern(IsWhiteMarubozu, Color.Cyan);
        HighlightPattern(IsBlackMarubozu, Color.DarkCyan);
        HighlightPattern(IsHammer, Color.Red);
        HighlightPattern(IsInvertedHammer, Color.DarkRed);
        HighlightPattern(IsBullishEngulfing, Color.Green);
        HighlightPattern(IsBearishEngulfing, Color.DarkGreen);
    }
    else if (selectedPattern == "Doji")
            {
                HighlightPattern(IsDoji, Color.Blue);
            }
 
            else if (selectedPattern == "Gravestone Doji")
            {
                HighlightPattern(IsGravestoneDoji, Color.Aqua);
            }
            else if (selectedPattern == "Dragonfly Doji")
            {
                HighlightPattern(IsDragonflyDoji, Color.LightBlue);
            }
            else if (selectedPattern == "Long-Legged Doji")
            {
                HighlightPattern(IsLongLeggedDoji, Color.CornflowerBlue);
            }
            else if (selectedPattern == "White Marubozu")
            {
                HighlightPattern(IsWhiteMarubozu, Color.Green);
            }
            else if (selectedPattern == "Black Marubozu")
            {
                HighlightPattern(IsBlackMarubozu, Color.Red);
            }
            else if (selectedPattern == "Hammer")
            {
                HighlightPattern(IsHammer, Color.Orange);
            }
            else if (selectedPattern == "Inverted Hammer")
            {
                HighlightPattern(IsInvertedHammer, Color.Yellow);
            }
            else if (selectedPattern == "Bullish Engulfing")
            {
                HighlightPattern(IsBullishEngulfing, Color.Lime);
            }
            else if (selectedPattern == "Bearish Engulfing")
            {
                HighlightPattern(IsBearishEngulfing, Color.Magenta);
            }
        }

        /// <summary>
        /// Sets the default values for the stock symbols and period type comboboxes.
        /// </summary>
        private void FormStockLoader_Load(object sender, EventArgs e)
        {
            stockSymbolsComboBox.SelectedIndex = 0;
            periodComboBox.SelectedIndex = 0;
        }



        /// <summary>
        /// Generates the file name for the selected stock symbol, period type, and date range and sets the text of the
        /// file name textbox accordingly. Also sets the text of the file location label to the directory where the
        /// data is stored and displays the number of matching files found.
        /// </summary>
        private void refreshFilenameButton_Click(object sender, EventArgs e)
        {
            string stockSymbol = stockSymbolsComboBox.SelectedItem.ToString();
            string periodType = periodComboBox.SelectedItem.ToString();
            DateTime startDate = startDateTimePicker.Value;
            DateTime endDate = endDateTimePicker.Value;

            string fileName = $"{stockSymbol}-{periodType}.CSV";
            filenameTextbox.Text = fileName.Trim();

            if (fileLocationLabel.Text.Length == 0)
            {
                fileLocationLabel.Text = System.AppDomain.CurrentDomain.BaseDirectory;
            }

            string directory = currentExecutableFolder();
            string dataDirectory = $@"{directory}Stock Data\";
            fileLocationLabel.Text = dataDirectory;

            string fileStock = filenameTextbox.Text;

            var fileCount = (from file in Directory.EnumerateFiles(dataDirectory, fileStock, SearchOption.AllDirectories)
                             select file).Count();
        }

        /// <summary>
        /// Returns the current executable folder path.
        /// </summary>
        private string currentExecutableFolder()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }


        /// <summary>
        /// Reads data from a CSV file and returns a DataTable containing the data.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <param name="isFirstRowHeader">A boolean value indicating whether the first row of the CSV file should be
        /// used as column headers.</param>
        /// <returns>A DataTable containing the data from the CSV file.</returns>
        static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }


        /// <summary>
        /// Reads data from a CSV file and returns a DataTable containing the data.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <returns>A DataTable containing the data from the CSV file.</returns>
        public DataTable ImportFromCSVFileAsync(string filePath)
        {
            int rowNumber = 0;

            // "Date","Open","High","Low","Close","Volume"
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Open");
            dt.Columns.Add("High");
            dt.Columns.Add("Low");
            dt.Columns.Add("Close");
            dt.Columns.Add("Volume");


            // splitting the values using Split() command 
            foreach (var srLine in File.ReadAllLines(filePath))
            {
                if (rowNumber > 0)
                {
                    dt.Rows.Add(srLine.Split(','));
                }
                rowNumber++;
            }
            return dt;

        }

        private async Task btnLoad_ClickAsync(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string stockSymbol = stockSymbolsComboBox.SelectedItem.ToString();
            string periodType = periodComboBox.SelectedItem.ToString();
            DateTime startDate = startDateTimePicker.Value;
            DateTime endDate = endDateTimePicker.Value;

            string fileName = $"{stockSymbol}-{periodType}.CSV";
            filenameTextbox.Text = fileName.Trim();

            if (fileLocationLabel.Text.Length == 0)
            {
                fileLocationLabel.Text = System.AppDomain.CurrentDomain.BaseDirectory;
            }

            string directory = currentExecutableFolder();
            string dataDirectory = $@"{directory}Stock Data\";
            fileLocationLabel.Text = dataDirectory;

            string fileStock = Path.Combine(dataDirectory.Trim(), filenameTextbox.Text.Trim());



            progressBar1.Visible = true;
            try
            {
                string startDateField = $"'{startDateTimePicker.Value.ToString("yyyy-MM-dd")}'";
                string endDateField = $"'{endDateTimePicker.Value.ToString("yyyy-MM-dd")}'";
                //DataTable dt = ImportFromCSVFileAsync(fileStock);

                DataTable dt = GetDataTableFromCsv(fileStock, true);

                string filtro = $"Date >= {startDateField} AND Date <= {endDateField}";
                DataView dv = new DataView(dt);
                dv.RowFilter = filtro; // query example = "id = 10"



                if (dt.Rows.Count > 0)

                {
                    dataGridView1.DataSource = null; // To clear the previous data before adding the new ones
                    dataGridView1.DataSource = dv;
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

                    stockChart.DataSource = dv;

                    // Show chart in a separate form
                    ChartForm chartForm = new ChartForm();
                    chartForm.StartPosition = FormStartPosition.Manual;
                    chartForm.Location = new Point(0, 0); // Set the position to top left
                    chartForm.Show();
                    chartForm.SetChartData(dv); // Pass the DataView to the ChartForm

                    // Show DataGridView in a separate form
                    DataGridViewForm dataGridViewForm = new DataGridViewForm();
                    dataGridViewForm.StartPosition = FormStartPosition.Manual;
                    dataGridViewForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - dataGridViewForm.Width, 0); // Set the position to top right
                    dataGridViewForm.Show();
                    dataGridViewForm.SetDataGridViewData(dv); // Pass the DataView to the DataGridViewForm
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            progressBar1.Visible = false;

            
    }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void stockSymbolsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void endDateTimePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void stockChart_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Populate the patternComboBox with pattern names
            patternComboBox.Items.AddRange(new string[]
            {
        "Common Doji",
        "Gravestone Doji",
        "Dragonfly Doji",
        "Long-Legged Doji",
        "White Marubozu",
        "Black Marubozu",
        "Hammer",
        "Inverted Hammer",
        "Bullish Engulfing",
        "Bearish Engulfing",
        "All Patterns"
            });
        }
    }
}
