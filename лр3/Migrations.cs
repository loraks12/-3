using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace лр3
{
    internal class Migrations
    {
        private DataTable data;

        public Migrations(string filePath)
        {
            data = ReadDataFromFile(filePath);
        }

        private DataTable ReadDataFromFile(string filePath)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Year", typeof(int));
            dt.Columns.Add("Immigrants", typeof(int));
            dt.Columns.Add("Emigrants", typeof(int));

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                int year = int.Parse(parts[0]);
                int immigrants = int.Parse(parts[1]);
                int emigrants = int.Parse(parts[2]);
                dt.Rows.Add(year, immigrants, emigrants);
            }

            return dt;
        }

        public void DisplayData()
        {
            foreach (DataRow row in data.Rows)
            {
                Console.WriteLine($"Year: {row["Year"]}, Immigrants: {row["Immigrants"]}, Emigrants: {row["Emigrants"]}");
            }
        }

        public void PlotMigrationTrend()
        {
            // You can use a charting library like 'System.Windows.Forms.DataVisualization' or any other library to plot the data
            // Let's say we are using console output for demonstration
            Console.WriteLine("Plotting migration trends...");
            Console.WriteLine("Not implemented for demo purposes.");
        }

        public int CalculateMaxMigrationChange()
        {
            int maxChange = 0;
            foreach (DataRow row in data.Rows)
            {
                int immigrants = Convert.ToInt32(row["Immigrants"]);
                int emigrants = Convert.ToInt32(row["Emigrants"]);
                int migrationChange = Math.Abs(immigrants - emigrants);

                if (migrationChange > maxChange)
                {
                    maxChange = migrationChange;
                }
            }

            return maxChange;
        }

        public void ForecastMigration(int nYears)
        {
            int lastYear = Convert.ToInt32(data.Rows[data.Rows.Count - 1]["Year"]);
            int lastImmigrants = Convert.ToInt32(data.Rows[data.Rows.Count - 1]["Immigrants"]);
            int lastEmigrants = Convert.ToInt32(data.Rows[data.Rows.Count - 1]["Emigrants"]);

            for (int i = 1; i <= nYears; i++)
            {
                int forecastedImmigrants = lastImmigrants + (1000 * i); // Assuming an increase of 1000 immigrants per year
                int forecastedEmigrants = lastEmigrants + (500 * i); // Assuming an increase of 500 emigrants per year

                Console.WriteLine($"Forecast for year {lastYear + i}: Immigrants - {forecastedImmigrants}, Emigrants - {forecastedEmigrants}");
            }
        }
    }
}