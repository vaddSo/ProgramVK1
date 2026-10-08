using System;
using System.Management;
using System.Collections.Generic;
using System.IO;

namespace vaddso
{
    public static class ConfigManager
    {
        private const string ConfigRelativePath = @"steamapps\common\Black Myth Wukong Benchmark Tool\b1\Saved\Config\Windows\GameUserSettings.ini";

        private static readonly string SteamRoot = @"C:\Program Files (x86)\Steam";
        private static readonly string ConfigPath = Path.Combine(SteamRoot, ConfigRelativePath);

        public static void GatherSystemSpecifications()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor"))
                {
                    foreach (var obj in searcher.Get())
                        Console.WriteLine($"Процессор: {obj["Name"]}");
                }

                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController"))
                {
                    foreach (var obj in searcher.Get())
                        Console.WriteLine($"Видеокарта: {obj["Name"]}");
                }

                using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        double ram = Convert.ToDouble(item["TotalPhysicalMemory"]) / (1024 * 1024 * 1024);
                        Console.WriteLine($"RAM: {ram:F1} GB");
                    }
                }
            }
            catch (ManagementException ex)
            {
                Console.WriteLine($"Не получилось собрать данные пк: {ex.Message}");
            }
        }

        public static Dictionary<string, string> PrepareCpuTest()
        {
            var settings = new Dictionary<string, string>
            {
                { "ResolutionSizeX", "1280" },
                { "ResolutionSizeY", "720" },
                { "LastUserConfirmedResolutionSizeX", "1280" },
                { "LastUserConfirmedResolutionSizeY", "720" },

                { "sg.ResolutionQuality", "100" },
                { "sg.ViewDistanceQuality", "0" },
                { "sg.AntiAliasingQuality", "0" },
                { "sg.ShadowQuality", "0" },
                { "sg.GlobalIlluminationQuality", "0" },
                { "sg.ReflectionQuality", "0" },
                { "sg.PostProcessQuality", "0" },
                { "sg.TextureQuality", "0" },
                { "sg.EffectsQuality", "0" },
                { "sg.FoliageQuality", "0" },
                { "sg.ShadingQuality", "0" },
                { "sg.RayTracingQuality", "0" },

            };
            UpdateIniFile(settings);
            return settings;
        }

        public static Dictionary<string, string> PrepareGpuTest()
        {
            var settings = new Dictionary<string, string>
            {
                { "ResolutionSizeX", "1920" },
                { "ResolutionSizeY", "1080" },
                { "LastUserConfirmedResolutionSizeX", "1920" },
                { "LastUserConfirmedResolutionSizeY", "1080" },

                { "sg.ResolutionQuality", "100" },
                { "sg.ViewDistanceQuality", "4" },
                { "sg.AntiAliasingQuality", "4" },
                { "sg.ShadowQuality", "4" },
                { "sg.GlobalIlluminationQuality", "4" },
                { "sg.ReflectionQuality", "4" },
                { "sg.PostProcessQuality", "4" },
                { "sg.TextureQuality", "4" },
                { "sg.EffectsQuality", "4" },
                { "sg.FoliageQuality", "4" },
                { "sg.ShadingQuality", "4" },
                { "sg.RayTracingQuality", "4" },

            };
            UpdateIniFile(settings);
            return settings;
        }

        private static void UpdateIniFile(Dictionary<string, string> settingsToUpdate)
        {
            if (!File.Exists(ConfigPath))
            {
                Console.WriteLine($"Файл конфиг не найден: {ConfigPath}");
                return;
            }

            bool readOnly = File.GetAttributes(ConfigPath).HasFlag(FileAttributes.ReadOnly);

            try
            {
                if (readOnly)
                {
                    File.SetAttributes(ConfigPath, File.GetAttributes(ConfigPath) & ~FileAttributes.ReadOnly);
                }

                string[] lines = File.ReadAllLines(ConfigPath);
                var changedKeys = new HashSet<string>();

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();

                    if (!line.Contains('=')) continue;

                    int index = line.IndexOf('=');
                    string currentKey = line.Substring(0, index).Trim();

                    if (settingsToUpdate.ContainsKey(currentKey))
                    {
                        lines[i] = $"{currentKey}={settingsToUpdate[currentKey]}";
                        changedKeys.Add(currentKey);
                    }
                }

                File.WriteAllLines(ConfigPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при изменении конфига: {ex.Message}");
            }
            finally
            {
                if (readOnly && File.Exists(ConfigPath))
                {
                    File.SetAttributes(
                        ConfigPath,
                        File.GetAttributes(ConfigPath) | FileAttributes.ReadOnly);
                }
            }
        }
    }
}