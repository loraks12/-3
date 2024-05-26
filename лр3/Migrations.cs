using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace лр3
{
    internal class Migrations
    {
        public List<MigrationData> MigrationRecords { get; private set; }

        public Migrations(string filePath)
        {
            LoadData(filePath);
        }
        private void LoadData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            MigrationRecords = lines.Skip(1) // Пропускаем заголовок
                                      .Select(line => line.Split('-'))
                                      .Select(parts => new MigrationData
                                      {
                                          Year = int.Parse(parts[0]),
                                          Immigrants = int.Parse(parts[1]),
                                          Emigrants = int.Parse(parts[2])
                                      })
                                      .ToList();
        }
        public (double maxChange, int maxChangeYear) CalculateMigrationChange()
        {
            double maxChange = 0;
            int maxChangeYear = 0;

            for (int i = 1; i < MigrationRecords.Count; i++)
            {
                double previousMigration = MigrationRecords[i - 1].Immigrants - MigrationRecords[i - 1].Emigrants;
                double currentMigration = MigrationRecords[i].Immigrants - MigrationRecords[i].Emigrants;

                if (previousMigration != 0)
                {
                    double change = ((currentMigration - previousMigration) / Math.Abs(previousMigration)) * 100;

                    if (Math.Abs(change) > Math.Abs(maxChange))
                    {
                        maxChange = change;
                        maxChangeYear = MigrationRecords[i].Year;
                    }
                }
            }

            return (maxChange, maxChangeYear);
        }

        public List<MigrationData> PerformForecast(int yearsToForecast)
        {
            var forecast = new List<MigrationData>();
            int lastYear = MigrationRecords.Last().Year;
            double[] movingAverageImmigrants = new double[MigrationRecords.Count];
            double[] movingAverageEmigrants = new double[MigrationRecords.Count];

            // Расчёт скользящей средней для иммигрантов
            for (int i = 2; i < MigrationRecords.Count; i++)
            {
                movingAverageImmigrants[i] = (MigrationRecords[i].Immigrants + MigrationRecords[i - 1].Immigrants + MigrationRecords[i - 2].Immigrants) / 3.0;
                movingAverageEmigrants[i] = (MigrationRecords[i].Emigrants + MigrationRecords[i - 1].Emigrants + MigrationRecords[i - 2].Emigrants) / 3.0;
            }

            // Экстраполяция
            for (int i = 0; i < yearsToForecast; i++)
            {
                lastYear++;
                double forecastedImmigrants = (movingAverageImmigrants[movingAverageImmigrants.Length - 1] +
                                               movingAverageImmigrants[movingAverageImmigrants.Length - 2] +
                                               movingAverageImmigrants[movingAverageImmigrants.Length - 3]) / 3.0;
                double forecastedEmigrants = (movingAverageEmigrants[movingAverageEmigrants.Length - 1] +
                                              movingAverageEmigrants[movingAverageEmigrants.Length - 2] +
                                              movingAverageEmigrants[movingAverageEmigrants.Length - 3]) / 3.0;
                forecast.Add(new MigrationData
                {
                    Year = lastYear,
                    Immigrants = (int)forecastedImmigrants,
                    Emigrants = (int)forecastedEmigrants
                });

                Array.Copy(movingAverageImmigrants, 1, movingAverageImmigrants, 0, movingAverageImmigrants.Length - 1);
                movingAverageImmigrants[movingAverageImmigrants.Length - 1] = forecastedImmigrants;

                Array.Copy(movingAverageEmigrants, 1, movingAverageEmigrants, 0, movingAverageEmigrants.Length - 1);
                movingAverageEmigrants[movingAverageEmigrants.Length - 1] = forecastedEmigrants;
            }

            return forecast;
        }
        public void PlotForecastGraphMigration(Chart chart, List<MigrationData> forecast)
        {
            if (forecast != null && forecast.Any())
            {
                var seriesImmigrants = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Immigrants Forecast",
                    Color = System.Drawing.Color.GreenYellow,
                    IsVisibleInLegend = true,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                var seriesEmigrants = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Emigrants Forecast",
                    Color = System.Drawing.Color.IndianRed,
                    IsVisibleInLegend = true,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                foreach (var data in forecast)
                {
                    seriesImmigrants.Points.AddXY(data.Year, data.Immigrants);
                    seriesEmigrants.Points.AddXY(data.Year, data.Emigrants);
                }

                chart.Series.Add(seriesImmigrants);
                chart.Series.Add(seriesEmigrants);
            }
            else
            {
                MessageBox.Show("Прогноз не выполнен!");
            }
        }
    }
}