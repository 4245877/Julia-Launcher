using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Julia_Launcher
{
    public static class SettingsManager
    {
        // Статические поля для директории и файла настроек
        private static readonly string settingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
        private static readonly string settingsFilePath = Path.Combine(settingsDirectory, "settings.json");

        // Метод для создания директории, если она не существует
        private static void EnsureSettingsDirectoryExists()
        {
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }
        }

        public static void SaveSettings(string key, object value)
        {
            try
            {
                EnsureSettingsDirectoryExists(); // Убеждаемся, что директория существует
                var settings = ReadSettings() ?? new Settings();

                switch (key)
                {
                    // TextBox
                    case "InstallDirectory": settings.InstallDirectory = (string)value; break;
                    case "LogDirectory": settings.LogDirectory = (string)value; break;
                    case "ModulesDirectory": settings.ModulesDirectory = (string)value; break;
                    case "CacheDirectory": settings.CacheDirectory = (string)value; break;
                    case "CPULimit": settings.CPULimit = (string)value; break;
                    case "GPULimit": settings.GPULimit = (string)value; break;
                    case "NetworkSpeed": settings.NetworkSpeed = (string)value; break;
                    case "HotkeyLounch": settings.HotkeyLounch = (string)value; break;

                    // CheckBox
                    case "CpuLoad": settings.CpuLoad = (string)value; break;
                    case "GPUEnable": settings.GPUEnable = (bool)value; break;
                    case "AutoStart": settings.AutoStart = (bool)value; break;
                    case "Lenguage": settings.Language = (string)value; break;
                    case "ManUpdate": settings.ManUpdate = (bool)value; break;
                    case "UpdateSrartup": settings.UpdateSrartup = (bool)value; break;
                    case "ChkLogRetention": settings.CheckLogRetention = (bool)value; break;
                    case "AutoUpdate": settings.AutoUpdate = (bool)value; break;
                    case "AllowedIPAddresses": settings.AllowedIPAddresses = (bool)value; break;
                    case "UpdPreferen": settings.UpdPreferen = (bool)value; break;
                        

                    // ComboBox
                    case "GpuSelection": settings.GpuSelection = (string)value; break;
                    case "CpuCores": settings.CpuCores = (string)value; break;
                    case "UpdateBranch": settings.UpdateBranch = (string)value; break;
                    case "LogLevel": settings.LogLevel = (string)value; break;
                    case "Language": settings.Language = (string)value; break;
                    case "Errors": settings.Errors = (string)value; break;
                    case "LogFormat": settings.LogFormat = (string)value; break;
                    case "Warnings": settings.Warnings = (string)value; break;
                    case "InfoMassages": settings.InfoMassages = (string)value; break;
                    case "Debugging": settings.Debugging = (string)value; break;

                    //TrackBar
                    case "RAMUsage": settings.RAMUsage = (int)value; break;
                    case "Height": settings.Height = (short)(int)value; break;
                    case "Weight": settings.Weight = (short)(int)value; break;
                    case "Age": settings.Age = (short)(int)value; break;
                    case "Tone": settings.Age = (short)(int)value; break;
                    case "SpeechRate": settings.Tone = (short)(int)value; break;
                    case "Volume": settings.Volume = (short)(int)value; break;
                    case "Timbre": settings.Timbre = (short)(int)value; break;

                    // RadioButton
                    case "ThemeWhite": settings.radWhite = (bool)value; break;
                    case "ThemeDark": settings.radDark = (bool)value; break;
                    case "ThemeSystem": settings.radSystem = (bool)value; break;
                }

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении настроек: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static Settings ReadSettings()
        {
            EnsureSettingsDirectoryExists(); // Убеждаемся, что директория существует
            if (File.Exists(settingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(settingsFilePath);
                    return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении настроек: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new Settings();
                }
            }
            return new Settings();
        }

    }
}