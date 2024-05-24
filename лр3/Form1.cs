using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace лр3
{
    public partial class Form1 : Form
    {
        private DataManager populationDataManager;
        public Form1()
        {
            InitializeComponent();
        }
        private void PlotForecastGraph(List<PopulationData> forecast)
        {
            if (forecast != null && forecast.Any())
            {
                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Forecast",
                    Color = System.Drawing.Color.Red,
                    IsVisibleInLegend = true,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                foreach (var data in forecast)
                {
                    series.Points.AddXY(data.Year, data.Population);
                }

                chart1.Series.Add(series);
            }
            else
            {
                MessageBox.Show("Прогноз не выполнен!");
            }
        }
        private void UpdateGrowthData()
        {
            if (populationDataManager != null)
            {
                var (maxIncrease, maxIncreaseYear, maxDecrease, maxDecreaseYear) = populationDataManager.CalculateGrowth();
                textBox1.Text = $"{maxIncrease:F2}% ({maxIncreaseYear})";
                textBox4.Text = $"{maxDecrease:F2}% ({maxDecreaseYear})";
            }
            else
            {
                textBox1.Text = string.Empty;
                textBox4.Text = string.Empty;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                populationDataManager = new DataManager(filePath);
                UpdateGrowthData();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (populationDataManager != null)
            {
                chart1.Series.Clear();
                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Population",
                    Color = System.Drawing.Color.Blue,
                    IsVisibleInLegend = true,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                foreach (var data in populationDataManager.PopulationRecords)
                {
                    series.Points.AddXY(data.Year, data.Population);
                }

                chart1.Series.Add(series);
            }
            else
            {
                MessageBox.Show("Данные не загружены!");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Прогнозирование
            if (populationDataManager != null && int.TryParse(textBox3.Text, out int yearsToForecast))
            {
                List<PopulationData> forecast = populationDataManager.PerformForecast(yearsToForecast);
                PlotForecastGraph(forecast);
            }
            else
            {
                MessageBox.Show("Ошибка ввода данных!");
            }
        }
    }
}
