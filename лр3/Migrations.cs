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
    }
}