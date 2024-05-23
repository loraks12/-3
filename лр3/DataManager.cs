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
        public List<PopulationData> PopulationRecords {  get; private set; }

        public DataManager(string filePath) {
            
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
    }
}
