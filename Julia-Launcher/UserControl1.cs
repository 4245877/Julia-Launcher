using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Julia_Launcher
{
    public partial class UserControl1 : UserControl
    {
        private readonly string settingsFilePath;

        // Ссылка на информацию о характеристиках
        private Form1.HardwareInfo hardwareInfo;

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

            // Используем существующие методы вместо лямбда-выражений
            txtInstallDirectory.TextChanged += txtInstallDirectory_TextChanged;
            txtLogDirectory.TextChanged += txtLogDirectory_TextChanged;
            txtModulesDirectory.TextChanged += txtModulesDirectory_TextChanged;
            txtCacheDirectory.TextChanged += txtCacheDirectory_TextChanged;

            // Загружаем настройки при запуске
            LoadSettings();

            // Получаем ссылку на информацию о характеристиках
            hardwareInfo = Form1.ComputerInfo;
            LoadHardwareInfo();
        }

        private void SaveSettings(string key, object value)
        {
            try
            {
                var settings = ReadSettings();
                switch (key)
                {
                    case "InstallDirectory": settings.InstallDirectory = (string)value; break;
                    case "LogDirectory": settings.LogDirectory = (string)value; break;
                    case "ModulesDirectory": settings.ModulesDirectory = (string)value; break;
                    case "CacheDirectory": settings.CacheDirectory = (string)value; break;
                    case "CPULimit": settings.CPULimit = (string)value; break;
                    case "RAMUsage": settings.RAMUsage = (int)value; break;
                    case "CpuLoad": settings.CpuLoad = (string)value; break;
                    case "GPULimit": settings.GPULimit = (string)value; break;
                    case "NetworkSpeed": settings.NetworkSpeed = (string)value; break;
                    case "CpuCores": settings.CpuCores = (string)value; break;
                    case "GpuSelection": settings.GpuSelection = (string)value; break;
                    case "GPUEnable": settings.GPUEnable = (bool)value; break;
                    case "AutoStart": settings.AutoStart = (bool)value; break;
                    case "UpdateBranch": settings.UpdateBranch = (string)value; break;
                    case "LogLevel": settings.LogLevel = (string)value; break;
                    case "Language": settings.Language = (string)value; break;
                }
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
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении настроек: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new Settings();
                }
            }
            return new Settings();
        }

        // Класс для хранения настроек
        private class Settings
        {
            // Свойства для TextBox
            public string InstallDirectory { get; set; } = string.Empty;
            public string LogDirectory { get; set; } = string.Empty;
            public string ModulesDirectory { get; set; } = string.Empty;
            public string CacheDirectory { get; set; } = string.Empty;

            public bool SomeCheckboxState { get; set; } = false;
            public int SelectedComboBoxIndex { get; set; } = 0;
            public int SelectedRadioButtonIndex { get; set; } = 0;

            // Свойства для CheckBox
            public bool GPUEnable { get; set; } = false;
            public bool AutoStart { get; set; } = false;
            public bool UpdPreferen { get; set; } = false;
            public bool AutoUpdate { get; set; } = false;
            public bool UpdateSrartup { get; set; } = false;
            public bool ManUpdate { get; set; } = false;



            // Свойства для ComboBox (храним выбранное значение как string)
            public string CpuCores { get; set; } = string.Empty;
            public string GpuSelection { get; set; } = string.Empty;
            public string UpdateBranch { get; set; } = string.Empty;
            public string LogLevel { get; set; } = string.Empty;
            public string Errors { get; set; } = string.Empty;
            public string Language { get; set; } = string.Empty;
            public string Warnings { get; set; } = string.Empty;
            public string InfoMassages { get; set; } = string.Empty;
            public string LogFormat { get; set; } = string.Empty;

            // Свойства для TextBox
            public string NetworkSpeed { get; set; } = string.Empty;
            public string GPULimit { get; set; } = string.Empty;
            public string CpuLoad { get; set; } = string.Empty;
            public string CPULimit { get; set; } = string.Empty;

            // Свойства для TrackBar
            public int RAMUsage { get; set; } = 0;

            // Свойства для RadioButton 1 и 2
            public bool RadioButton1Checked { get; set; } = false;
            public bool RadioButton2Checked { get; set; } = false;


            public Dictionary<string, object> AdditionalSettings { get; set; } = new Dictionary<string, object>();
        }

        // Загрузка сохраненных настроек 
        private void LoadSettings()
        {
            var settings = ReadSettings();

            // TextBox controls
            txtInstallDirectory.Text = settings.InstallDirectory;
            txtLogDirectory.Text = settings.LogDirectory;
            txtModulesDirectory.Text = settings.ModulesDirectory;
            txtCacheDirectory.Text = settings.CacheDirectory;
            txtCPULimit.Text = settings.CPULimit;
            txtGPULimit.Text = settings.GPULimit;
            txtNetworkSpeed.Text = settings.NetworkSpeed;

            // CheckBox controls
            chkGPUEnable.Checked = settings.GPUEnable;
            chkAutoStart.Checked = settings.AutoStart;
            // Add other checkboxes here

            // ComboBox controls
            if (!string.IsNullOrEmpty(settings.CpuCores) && cmbCpuCores.Items.Contains(settings.CpuCores))
                cmbCpuCores.SelectedItem = settings.CpuCores;

            if (!string.IsNullOrEmpty(settings.UpdateBranch) && cmbUpdateBranch.Items.Contains(settings.UpdateBranch))
                cmbUpdateBranch.SelectedItem = settings.UpdateBranch;

            if (!string.IsNullOrEmpty(settings.LogLevel) && cmbLogLevel.Items.Contains(settings.LogLevel))
                cmbLogLevel.SelectedItem = settings.LogLevel;

            // TrackBar controls
            //trackRamUsage.Value = settings.RAMUsage;

            // RadioButton controls
            radioButton1.Checked = settings.RadioButton1Checked;
            radioButton2.Checked = settings.RadioButton2Checked;
        }



        private void LoadHardwareInfo()
        {
            // Получаем доступ к информации о железе через статический экземпляр
            var computerInfo = Form1.ComputerInfo;

            if (computerInfo != null)
            {
                // Заполняем данные в элементы управления
                // Используйте фактические имена ваших элементов управления

                // Процессор
                // Предполагается, что у вас есть ComboBox для отображения количества ядер
                if (cmbCpuCores != null)
                {
                    cmbCpuCores.Items.Clear();
                    cmbCpuCores.Items.Add(computerInfo.CpuCores.ToString());
                    if (cmbCpuCores.Items.Count > 0)
                        cmbCpuCores.SelectedIndex = 0;
                }

                // Здесь заполните другие элементы управления
                // в соответствии с их фактическими именами
            }
        }


















        private void txtCPULimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только цифры и управляющие клавиши
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCPULimit.Text))
                return;

            if (int.TryParse(txtCPULimit.Text, out int number))
            {
                if (number < 1 || number > 100)
                {
                    // Восстанавливаем предыдущее значение или очищаем TextBox
                    txtCPULimit.Text = number < 1 ? "1" : "100";
                    txtCPULimit.SelectionStart = txtCPULimit.Text.Length; // Устанавливаем курсор в конец
                }
            }
            else
            {
                // Удаляем некорректный ввод
                txtCPULimit.Text = "";
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

        private void txtCPULimit_TextChanged(object sender, EventArgs e)
        {
            // Существующий код проверки (был назван textBox4_TextChanged)
            if (string.IsNullOrEmpty(txtCPULimit.Text))
                return;

            if (int.TryParse(txtCPULimit.Text, out int number))
            {
                if (number < 1 || number > 100)
                {
                    txtCPULimit.Text = number < 1 ? "1" : "100";
                    txtCPULimit.SelectionStart = txtCPULimit.Text.Length;
                }
            }
            else
            {
                txtCPULimit.Text = "";
            }
            SaveSettings("CPULimit", txtCPULimit.Text);
        }

        private void txtGPULimit_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtGPULimit.Text))
                return;

            if (int.TryParse(txtGPULimit.Text, out int number))
            {
                if (number < 1 || number > 100)
                {
                    txtGPULimit.Text = number < 1 ? "1" : "100";
                    txtGPULimit.SelectionStart = txtGPULimit.Text.Length;
                }
            }
            else
            {
                txtGPULimit.Text = "";
            }
            SaveSettings("GPULimit", txtGPULimit.Text); // Уже есть в коде
        }

        private void txtNetworkSpeed_TextChanged(object sender, EventArgs e)
        {
            SaveSettings("NetworkSpeed", txtNetworkSpeed.Text);
        }


        //Button 



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

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnApply_Click(object sender, EventArgs e)
        {

        }

        // ComboBox

        private void cmbCpuCores_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ПОместить информацию сколько ядер выбрано
        }

        private void cmbUpdateBranch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbErrors.SelectedItem != null)
            {
                SaveSettings("Errors", cmbErrors.SelectedItem.ToString());
            }
        }
        private void cmbLogFormat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        // CheckBox


        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("AutoStart", chkAutoStart.Checked);
        }

        private void chkGPUEnable_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("GPUEnable", chkGPUEnable.Checked);
        }

        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        // RadioButton

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("RadioButton1Checked", radioButton1.Checked);
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            SaveSettings("RadioButton2Checked", radioButton2.Checked);
        }

        // TrackBar



        private void trackRamUsage_Scroll(object sender, EventArgs e)
        {
            SaveSettings("RAMUsage", trackRamUsage.Value);
        }

    }
}