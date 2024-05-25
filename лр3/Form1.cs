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
            InitializeDataGridView();
        }
        private void InitializeDataGridView()
        {
            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "Year";
            dataGridView1.Columns[1].Name = "Population";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
        private void PopulateDataGridView()
        {
            if (populationDataManager != null)
            {
                dataGridView1.Rows.Clear();
                foreach (var record in populationDataManager.PopulationRecords)
                {
                    dataGridView1.Rows.Add(record.Year, record.Population);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                populationDataManager = new DataManager(filePath);
            }

            PopulateDataGridView();
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
