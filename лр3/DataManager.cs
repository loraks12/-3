using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace лр3
{
    public class DataManager
    {
        public List<PopulationData> PopulationRecords { get; private set; }

        public DataManager(string filePath)
        {
            LoadData(filePath);
        }

        private void LoadData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            PopulationRecords = lines.Skip(1) // Пропускаем заголовок
                                      .Select(line => line.Split(','))
                                      .Select(parts => new PopulationData
                                      {
                                          Year = int.Parse(parts[0]),
                                          Population = int.Parse(parts[1])
                                      })
                                      .ToList();
        }

        public (double maxIncrease, int maxIncreaseYear, double maxDecrease, int maxDecreaseYear) CalculateGrowth()
        {
            double maxIncrease = 0;
            double maxDecrease = 0;
            int maxIncreaseYear = 0;
            int maxDecreaseYear = 0;

            for (int i = 1; i < PopulationRecords.Count; i++)
            {
                double change = ((double)(PopulationRecords[i].Population - PopulationRecords[i - 1].Population)
                    / PopulationRecords[i - 1].Population) * 100;
                if (change > maxIncrease)
                {
                    maxIncrease = change;
                    maxIncreaseYear = PopulationRecords[i].Year;
                }
                if (change < maxDecrease)
                {
                    maxDecrease = change;
                    maxDecreaseYear = PopulationRecords[i].Year;
                }
            }

            return (maxIncrease, maxIncreaseYear, maxDecrease, maxDecreaseYear);
        }

        public List<PopulationData> PerformForecast(int yearsToForecast)
        {
            var forecast = new List<PopulationData>();
            int lastYear = PopulationRecords.Last().Year;
            double[] movingAverage = new double[PopulationRecords.Count];

            // Расчёт скользящей средней
            for (int i = 2; i < PopulationRecords.Count; i++)
            {
                movingAverage[i] = (PopulationRecords[i].Population + PopulationRecords[i - 1].Population + PopulationRecords[i - 2].Population) / 3.0;
            }

            // Экстраполяция
            for (int i = 0; i < yearsToForecast; i++)
            {
                lastYear++;
                double forecastValue = (movingAverage[movingAverage.Length - 1] + movingAverage[movingAverage.Length - 2] + movingAverage[movingAverage.Length - 3]) / 3.0;
                forecast.Add(new PopulationData { Year = lastYear, Population = (int)forecastValue });
                Array.Copy(movingAverage, 1, movingAverage, 0, movingAverage.Length - 1);
                movingAverage[movingAverage.Length - 1] = forecastValue;
            }

            return forecast;
        }
    }
}
