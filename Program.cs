using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace vaddso
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Wukong Benchmark Tool";

            Console.WriteLine("Характеристики пк");
            ConfigManager.GatherSystemSpecifications();

            Console.WriteLine();
            Console.WriteLine("CPU-Тест");

            Dictionary<string, string> cpuSettings = ConfigManager.PrepareCpuTest();

            PrintSettings(cpuSettings);

            if (!AutomationEngine.ExecuteBenchmarkSession())
            {
                Console.WriteLine("CPU-тест не был выполнен.");
                Console.ReadKey();
                return;
            }
            var cpuReport = LogParser.ExtractLatestReport();

            Console.WriteLine();
            Console.WriteLine("GPU-Тест");

            Dictionary<string, string> gpuSettings = ConfigManager.PrepareGpuTest();

            PrintSettings(gpuSettings);

            if (!AutomationEngine.ExecuteBenchmarkSession())
            {
                Console.WriteLine("GPU-тест не был выполнен.");
                Console.ReadKey();
                return;
            }
            var gpuReport = LogParser.ExtractLatestReport();

            Console.Clear();
            Console.WriteLine("Результаты");

            Console.WriteLine("Характеристики пк");
            ConfigManager.GatherSystemSpecifications();

            Console.WriteLine();
            Console.WriteLine("CPU-тест");

            PrintSettings(cpuSettings);
            PrintReport(cpuReport);

            Console.WriteLine();
            Console.WriteLine("GPU-тест");

            PrintSettings(gpuSettings);
            PrintReport(gpuReport);

            Console.WriteLine();
            Console.WriteLine("Любая клавиша для выхода");
            Console.ReadKey();
        }

        private static void PrintSettings(Dictionary<string, string> settings)
        {
            Console.WriteLine("Настройки");
            foreach (var setting in settings)
            {
                Console.WriteLine($"  {setting.Key} = {setting.Value}");
            }
            Console.WriteLine();
        }

        private static void PrintReport(GameBenchmarkReport report)
        {
            if (report == null)
            {
                Console.WriteLine("Результат не получен");
                return;
            }
            Console.WriteLine($"Средний FPS: {report.AvgFps:F1}");
            Console.WriteLine($"Минимальный FPS: {report.MinFps:F1}");
        }
    }
}