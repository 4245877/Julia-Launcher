using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using System.Text.Json;

namespace Julia_Launcher
{
    public partial class UserControl1 : UserControl
    {
        private const string settingsFilePath = "settings.json";

        public UserControl1()
        {
            InitializeComponent();
            // Подписываемся на события изменения текста
            txtInstallDirectory.TextChanged += txtInstallDirectory_TextChanged;
            txtLogDirectory.TextChanged += txtLogDirectory_TextChanged;
            txtModulesDirectory.TextChanged += txtModulesDirectory_TextChanged;

            // Загружаем настройки при запуске (опционально)
            LoadSettings();
        }

        private void SaveSettings(string key, string value)
        {
            try
            {
                // Читаем текущие настройки
                var settings = ReadSettings();

                // Обновляем нужное поле
                if (key == "InstallDirectory")
                {
                    settings.InstallDirectory = value;
                }
                else if (key == "LogDirectory")
                {
                    settings.LogDirectory = value;
                }
                else if (key == "ModulesDirectory")
                {
                    settings.ModulesDirectory = value;
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
                string json = File.ReadAllText(settingsFilePath);
                return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
            return new Settings();
        }

        // Класс для хранения настроек
        private class Settings
        {
            public string InstallDirectory { get; set; } = string.Empty;
            public string LogDirectory { get; set; } = string.Empty;
            public string ModulesDirectory { get; set; } = string.Empty;
        }

        // Загрузка сохраненных настроек в TextBox при запуске (опционально)
        private void LoadSettings()
        {
            var settings = ReadSettings();
            txtInstallDirectory.Text = settings.InstallDirectory;
            txtLogDirectory.Text = settings.LogDirectory;
            txtModulesDirectory.Text = settings.ModulesDirectory; // Исправлено: присваивание текстовому полю
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

        private void cmbCpuCores_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSelectInstallDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                // Настраиваем диалоговое окно
                folderDialog.Description = "Выберите папку для установки";
                folderDialog.ShowNewFolderButton = true; // Показывать кнопку создания новой папки
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer; // Начальная папка

                // Открываем диалог и проверяем, что пользователь выбрал папку
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Сохраняем выбранный путь в txtInstallDirectory
                    txtInstallDirectory.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void txtInstallDirectory_TextChanged(object sender, EventArgs e)
        {
            SaveSettings("InstallDirectory", txtInstallDirectory.Text);
        }

        private void btnSelectLogDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                // Настраиваем диалоговое окно
                folderDialog.Description = "Выберите папку для логов";
                folderDialog.ShowNewFolderButton = true; // Показывать кнопку создания новой папки
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer; // Начальная папка

                // Открываем диалог и проверяем, что пользователь выбрал папку
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Сохраняем выбранный путь в txtLogDirectory
                    txtLogDirectory.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnSelectModulesDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                // Настраиваем диалоговое окно
                folderDialog.Description = "Выберите папку для модулей";
                folderDialog.ShowNewFolderButton = true; // Показывать кнопку создания новой папки
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer; // Начальная папка

                // Открываем диалог и проверяем, что пользователь выбрал папку
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Сохраняем выбранный путь в txtModulesDirectory
                    txtModulesDirectory.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnSelectCacheDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                // Настраиваем диалоговое окно
                folderDialog.Description = "Выберите папку для кеша";
                folderDialog.ShowNewFolderButton = true; // Показывать кнопку создания новой папки
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer; // Начальная папка

                // Открываем диалог и проверяем, что пользователь выбрал папку
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Сохраняем выбранный путь в txtCacheDirectory
                    txtCacheDirectory.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void txtLogDirectory_TextChanged(object sender, EventArgs e)
        {
            SaveSettings("LogDirectory", txtLogDirectory.Text);
        }

        private void txtModulesDirectory_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
