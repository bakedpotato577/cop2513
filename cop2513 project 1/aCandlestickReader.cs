using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cop2513_project_1
{
    public class CandlestickReader
    {
        /// <summary>
        /// Reads CSV files from the "Stock Data" folder in the Bin\Release folder of the project for a given ticker symbol.
        /// </summary>
        /// <param name="ticker">The ticker symbol for which to read data.</param>
        /// <returns>A DataSet containing the data from the CSV files.</returns>
        public DataSet ReadData(string ticker)
        {
            // Create a new DataSet to hold the data from the CSV files.
            DataSet ds = new DataSet();

            // Construct the path to the "Stock Data" folder in the Bin\Release folder of the project.
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stock Data");

            // Check if the "Stock Data" folder exists.
            if (!Directory.Exists(path))
            {
                throw new Exception("The 'Stock Data' folder doesn't exist in the Bin\\Release folder of the project.");
            }

            // Define an array of file names for the given ticker symbol.
            string[] fileNames = { $"{ticker}-Day.csv", $"{ticker}-Week.csv", $"{ticker}-Month.csv" };

            // Loop through each file and read the data.
            foreach (string fileName in fileNames)
            {
                // Construct the full path to the file.
                string filePath = Path.Combine(path, fileName);

                // Check if the file exists.
                if (!File.Exists(filePath))
                {
                    throw new Exception($"The file '{fileName}' doesn't exist in the 'Stock Data' folder.");
                }

                // Create a new DataTable to hold the data from the file.
                DataTable dt = new DataTable();
                dt.Columns.Add("Date", typeof(DateTime));
                dt.Columns.Add("Open", typeof(double));
                dt.Columns.Add("High", typeof(double));
                dt.Columns.Add("Low", typeof(double));
                dt.Columns.Add("Close", typeof(double));
                dt.Columns.Add("Volume", typeof(int));

                // Read the data from the file.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Skip the first line (header).
                    string line;
                    bool isFirstLine = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }

                        // Split the line into fields.
                        string[] fields = line.Split(',');

                        // Create a new DataRow for the current line of data.
                        DataRow dr = dt.NewRow();
                        dr["Date"] = DateTime.Parse(fields[0]);
                        dr["Open"] = double.Parse(fields[1]);
                        dr["High"] = double.Parse(fields[2]);
                        dr["Low"] = double.Parse(fields[3]);
                        dr["Close"] = double.Parse(fields[4]);
                        dr["Volume"] = int.Parse(fields[5]);
                        dt.Rows.Add(dr);
                    }
                }

                // Set the name of the DataTable to the name of the file (without the ".csv" extension).
                string tableName = fileName.Replace(".csv", "");
                ds.Tables.Add(dt);
                ds.Tables[ds.Tables.Count - 1].TableName = tableName;
            }

            return ds;
        }
    }
}
