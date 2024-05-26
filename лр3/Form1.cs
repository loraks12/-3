using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace лр3
{
    public partial class Form1 : Form
    {
        private DataManager population;
        private Migrations migration;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeDataGridView()
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Year";
            dataGridView1.Columns[1].Name = checkBox1.Checked ? "Immigrants" : "Population";
            dataGridView1.Columns[2].Name = checkBox1.Checked ? "Emigrants" : "";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void PopulateDataGridView()
        {
            if (checkBox1.Checked && migration != null)
            {
                dataGridView1.Rows.Clear();
                foreach (var record in migration.MigrationRecords)
                {
                    dataGridView1.Rows.Add(record.Year, record.Immigrants, record.Emigrants);
                }
            }
            else if (population != null)
            {
                dataGridView1.Rows.Clear();
                foreach (var record in population.PopulationRecords)
                {
                    dataGridView1.Rows.Add(record.Year, record.Population, DBNull.Value);
                }
            }
        }

        private void UpdateGrowthData()
        {
            if (checkBox1.Checked && migration != null)
            {
                var (maxChange, maxChangeYear) = migration.CalculateMigrationChange();
                textBox2.Text = $"{maxChange:F2}% ({maxChangeYear})";
                textBox4.Text = string.Empty;
                textBox1.Text = string.Empty;
            }
            else if (population != null)
            {
                var (maxIncrease, maxIncreaseYear, maxDecrease, maxDecreaseYear) = population.CalculateGrowth();
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
            InitializeDataGridView();

            textBox1.Text = string.Empty;
            textBox2.Text= string.Empty;
            textBox3.Text= string.Empty;
            textBox4.Text= string.Empty;

            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filepath = openFileDialog1.FileName;
                if (checkBox1.Checked)
                {
                    migration = new Migrations(filepath);
                }
                else
                {
                    population = new DataManager(filepath);
                }
            }
            UpdateGrowthData();
            PopulateDataGridView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked && migration != null && int.TryParse(textBox3.Text, out int yearsToForecast))
            {
                List<MigrationData> forecast = migration.PerformForecast(yearsToForecast);
                migration.PlotForecastGraphMigration(chart1, forecast);
            }
            else if (population != null && int.TryParse(textBox3.Text, out int yearstoForecast))
            {
                List<PopulationData> forecast = population.PerformForecast(yearstoForecast);
                population.PlotForecastGraphPopulation(chart1, forecast);
            }
            else
            {
                MessageBox.Show("Ошибка ввода данных!");
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked && migration != null)
            {
                chart1.Series.Clear();
                var seriesImmigrants = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Immigrants",
                    Color = System.Drawing.Color.Green,
                    IsVisibleInLegend = true,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                var seriesEmigrants = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Emigrants",
                    Color = System.Drawing.Color.Orange,
                    IsVisibleInLegend = true,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                foreach (var data in migration.MigrationRecords)
                {
                    seriesImmigrants.Points.AddXY(data.Year, data.Immigrants);
                    seriesEmigrants.Points.AddXY(data.Year, data.Emigrants);
                }

                chart1.Series.Add(seriesImmigrants);
                chart1.Series.Add(seriesEmigrants);
            }
            else if (population != null)
            {
                chart1.Series.Clear();
                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Population",
                    Color = System.Drawing.Color.Blue,
                    IsVisibleInLegend = true,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                foreach (var data in population.PopulationRecords)
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
    }
}
