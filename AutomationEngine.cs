using System;
using System.Diagnostics;
using System.Threading;

namespace vaddso
{
    public static class AutomationEngine
    {
        private const string SteamAppUri = "steam://run/3132990";
        private const string ProcessName = "b1-Win64-Shipping";

        public static bool ExecuteBenchmarkSession()
        {
            foreach (Process process in Process.GetProcessesByName(ProcessName))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch
                {
                }
            }

            Console.WriteLine("Запуск бенчмарка");

            Process.Start(new ProcessStartInfo
            {
                FileName = SteamAppUri,
                UseShellExecute = true
            });

            Process benchmark = null;

            for (int i = 0; i < 60; i++)
            {
                Process[] processes = Process.GetProcessesByName(ProcessName);

                if (processes.Length > 0)
                {
                    benchmark = processes[0];
                    break;
                }
                Thread.Sleep(1000);
            }

            if (benchmark == null)
            {
                Console.WriteLine("Бенчмарк не запустился");
                return false;
            }

            Console.WriteLine($"Бенчмарк запущен PID:{benchmark.Id}");
            benchmark.WaitForExit();

            Console.WriteLine("Завершение");
            Thread.Sleep(2000);

            return true;
        }
    }
}