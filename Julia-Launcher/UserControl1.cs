using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Julia_Launcher
{
    public partial class UserControl1 : UserControl
    {
        // Измененный путь для сохранения настроек в директории проекта в папке settings
        private readonly string settingsFilePath;

        public UserControl1()
        {
            InitializeComponent();

            // Создаем директорию settings, если она не существует
            string settingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            settingsFilePath = Path.Combine(settingsDirectory, "settings.json");

            // Подписываемся на события изменения текста
            // Используем существующие методы вместо лямбда-выражений
            txtInstallDirectory.TextChanged += txtInstallDirectory_TextChanged;
            txtLogDirectory.TextChanged += txtLogDirectory_TextChanged;
            txtModulesDirectory.TextChanged += txtModulesDirectory_TextChanged;
            txtCacheDirectory.TextChanged += txtCacheDirectory_TextChanged;

            // Загружаем настройки при запуске
            LoadSettings();
        }

        private void SaveSettings(string key, string value)
        {
            try
            {
                // Читаем текущие настройки
                var settings = ReadSettings();

                // Обновляем нужное поле
                switch (key)
                {
                    case "InstallDirectory":
                        settings.InstallDirectory = value;
                        break;
                    case "LogDirectory":
                        settings.LogDirectory = value;
                        break;
                    case "ModulesDirectory":
                        settings.ModulesDirectory = value;
                        break;
                    case "CacheDirectory":
                        settings.CacheDirectory = value;
                        break;
                }

                // Записываем обновленные настройки в файл
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении настроек: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Settings ReadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(settingsFilePath);
                    return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
                catch
                {
                    // В случае ошибки возвращаем новый объект настроек
                    return new Settings();
                }
            }
            return new Settings();
        }

        // Класс для хранения настроек
        private class Settings
        {
            public string InstallDirectory { get; set; } = string.Empty;
            public string LogDirectory { get; set; } = string.Empty;
            public string ModulesDirectory { get; set; } = string.Empty;
            public string CacheDirectory { get; set; } = string.Empty;
        }

        // Загрузка сохраненных настроек в TextBox при запуске
        private void LoadSettings()
        {
            var settings = ReadSettings();
            txtInstallDirectory.Text = settings.InstallDirectory;
            txtLogDirectory.Text = settings.LogDirectory;
            txtModulesDirectory.Text = settings.ModulesDirectory;
            txtCacheDirectory.Text = settings.CacheDirectory;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только цифры и управляющие клавиши
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text))
                return;

            if (int.TryParse(textBox4.Text, out int number))
            {
                if (number < 1 || number > 100)
                {
                    // Восстанавливаем предыдущее значение или очищаем TextBox
                    textBox4.Text = number < 1 ? "1" : "100";
                    textBox4.SelectionStart = textBox4.Text.Length; // Устанавливаем курсор в конец
                }
            }
            else
            {
                // Удаляем некорректный ввод
                textBox4.Text = "";
            }
        }

        // Общий метод для выбора директории
        private void SelectDirectory(TextBox textBox, string description)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = description;
                folderDialog.ShowNewFolderButton = true;
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        // Обработчики событий изменения текста
        private void txtInstallDirectory_TextChanged(object sender, EventArgs e)
        {
            SaveSettings("InstallDirectory", txtInstallDirectory.Text);
        }

        private void txtLogDirectory_TextChanged(object sender, EventArgs e)
        {
            SaveSettings("LogDirectory", txtLogDirectory.Text);
        }

        private void txtModulesDirectory_TextChanged(object sender, EventArgs e)
        {
            SaveSettings("ModulesDirectory", txtModulesDirectory.Text);
        }

        private void txtCacheDirectory_TextChanged(object sender, EventArgs e)
        {
            SaveSettings("CacheDirectory", txtCacheDirectory.Text);
        }

        private void btnSelectInstallDirectory_Click(object sender, EventArgs e)
        {
            SelectDirectory(txtInstallDirectory, "Выберите папку для установки");
        }

        private void btnSelectLogDirectory_Click(object sender, EventArgs e)
        {
            SelectDirectory(txtLogDirectory, "Выберите папку для логов");
        }

        private void btnSelectModulesDirectory_Click(object sender, EventArgs e)
        {
            SelectDirectory(txtModulesDirectory, "Выберите папку для модулей");
        }

        private void btnSelectCacheDirectory_Click(object sender, EventArgs e)
        {
            SelectDirectory(txtCacheDirectory, "Выберите папку для кеша");
        }

        private void cmbCpuCores_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Оставлено пустым, так как в исходном коде не было реализации
        }
    }
}