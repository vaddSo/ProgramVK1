using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace vaddso
{
    public class GameBenchmarkReport
    {
        [JsonPropertyName("AverageFPS")]
        public double AvgFps { get; set; }

        [JsonPropertyName("MinFPS")]
        public double MinFps { get; set; }
    }

    public static class LogParser
    {
        public static GameBenchmarkReport ExtractLatestReport()
        {
            string savedFolder =
                @"C:\Program Files (x86)\Steam\steamapps\common\Black Myth Wukong Benchmark Tool\b1\Saved";

            if (!Directory.Exists(savedFolder))
            {
                Console.WriteLine("Папка Saved не найдена");
                return null;
            }

            var files = Directory
                .EnumerateFiles(savedFolder, "*", SearchOption.AllDirectories)
                .Select(path => new FileInfo(path))
                .OrderByDescending(file => file.LastWriteTimeUtc)
                .ToList();

            Console.WriteLine("\nПоследние изменённые файлы:");

            foreach (var file in files.Take(20))
            {
                Console.WriteLine(
                    $"{file.LastWriteTime:HH:mm:ss} | {file.FullName}");
            }

            if (files.Count == 0)
            {
                Console.WriteLine("Результаты бенчмарка не найдены");
                return null;
            }

            var latestFile = files.First();

            try
            {
                string json = File.ReadAllText(latestFile.FullName);

                var report = JsonSerializer.Deserialize<GameBenchmarkReport>(json);

                Console.WriteLine($"Результат прочитан: {latestFile.Name}");
                return report;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось прочитать результат: {ex.Message}");
                return null;
            }
        }
    }
}