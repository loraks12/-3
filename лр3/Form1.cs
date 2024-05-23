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
        private DataManager dataManager;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                dataManager = new DataManager(filePath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataManager != null)
            {
                chart1.Series.Clear();
                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Population",
                    Color = System.Drawing.Color.Blue,
                    IsVisibleInLegend = true,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                foreach (var data in dataManager.PopulationRecords)
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
