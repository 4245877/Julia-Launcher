using static Julia_Launcher.SettingsManager;
using static Julia_Launcher.Settings;
using Julia_Launcher;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System.Diagnostics;


namespace Julia_Launcher
{
    public partial class UserControl1 : UserControl
    {
        private readonly string settingsFilePath;

        // Ссылка на информацию о характеристиках
        private HardwareInfo hardwareInfo;
        private int previousRamUsage; // Хранение предыдущего значения TrackBar

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
            if (hardwareInfo != null)
            {
                trackRamUsage.Maximum = (int)(hardwareInfo.RamTotalVisibleBytes / (1024 * 1024)); // Переводим байты в МБ
                previousRamUsage = trackRamUsage.Value; // Инициализируем предыдущее значение
                InitializeComboBox();
                LoadHardwareInfo();
                LoadSettings(); // Загружаем настройки только если hardwareInfo доступен
            }
            else
            {
                MessageBox.Show("Информация о железе недоступна.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComboBox()
        {
            cmbGpuSelection.Items.Clear();
            cmbGpuSelection.Items.Add("Auto");

            if (hardwareInfo != null && hardwareInfo.Gpus != null)
            {
                for (int i = 0; i < hardwareInfo.Gpus.Count; i++)
                {
                    var gpu = hardwareInfo.Gpus[i];
                    cmbGpuSelection.Items.Add($"GPU {i} - {gpu.Name}");
                }
            }

            cmbGpuSelection.Items.Add("Default");
            cmbGpuSelection.Items.Add("None");
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
            txtHotkeyLounch.Text = settings.HotkeyLounch;
            // Добавлено свойство CpuLoad, отсутствующее в исходном коде метода
            txtCpuLoad.Text = settings.CpuLoad;

            // CheckBox controls
            chkGPUEnable.Checked = settings.GPUEnable;
            chkAutoStart.Checked = settings.AutoStart;
            chkUpdPreferen.Checked = settings.UpdPreferen;
            chkAutoUpdate.Checked = settings.AutoUpdate;
            chkUpdateSrartup.Checked = settings.UpdateSrartup;
            chkManUpdate.Checked = settings.ManUpdate;
            chkProtectionWithaPassword.Checked = settings.ProtectionWithaPassword;
            chkAllowedIPAddresses.Checked = settings.AllowedIPAddresses;

            chkLogRetention.Checked = settings.CheckLogRetention;

            // ComboBox 
            if (!string.IsNullOrEmpty(settings.CpuCores) && cmbCpuCores.Items.Contains(settings.CpuCores))
                cmbCpuCores.SelectedItem = settings.CpuCores;

            if (!string.IsNullOrEmpty(settings.GpuSelection) && cmbGpuSelection.Items.Contains(settings.GpuSelection))
                cmbGpuSelection.SelectedItem = settings.GpuSelection;

            if (!string.IsNullOrEmpty(settings.UpdateBranch) && cmbUpdateBranch.Items.Contains(settings.UpdateBranch))
                cmbUpdateBranch.SelectedItem = settings.UpdateBranch;

            if (!string.IsNullOrEmpty(settings.LogLevel) && cmbLogLevel.Items.Contains(settings.LogLevel))
                cmbLogLevel.SelectedItem = settings.LogLevel;

            if (!string.IsNullOrEmpty(settings.Language) && cmbLanguage.Items.Contains(settings.Language))
                cmbLanguage.SelectedItem = settings.Language;

            if (!string.IsNullOrEmpty(settings.Errors) && cmbErrors.Items.Contains(settings.Errors))
                cmbErrors.SelectedItem = settings.Errors;

            if (!string.IsNullOrEmpty(settings.Warnings) && cmbWarnings.Items.Contains(settings.Warnings))
                cmbWarnings.SelectedItem = settings.Warnings;

            if (!string.IsNullOrEmpty(settings.InfoMassages) && cmbInfoMassages.Items.Contains(settings.InfoMassages))
                cmbInfoMassages.SelectedItem = settings.InfoMassages;

            if (!string.IsNullOrEmpty(settings.LogFormat) && cmbLogFormat.Items.Contains(settings.LogFormat))
                cmbLogFormat.SelectedItem = settings.LogFormat;

            if (!string.IsNullOrEmpty(settings.Debugging) && cmbDebugging.Items.Contains(settings.Debugging))
                cmbDebugging.SelectedItem = settings.Debugging;

            // Для SelectedComboBoxIndex предполагается, что это индекс одного из ComboBox
            if (settings.SelectedComboBoxIndex >= 0 && settings.SelectedComboBoxIndex < cmbCpuCores.Items.Count)
                cmbCpuCores.SelectedIndex = settings.SelectedComboBoxIndex; // Пример использования
            if (!string.IsNullOrEmpty(settings.GpuSelection) && cmbGpuSelection.Items.Contains(settings.GpuSelection))
                cmbGpuSelection.SelectedItem = settings.GpuSelection;
            else
                cmbGpuSelection.SelectedIndex = 0; // По умолчанию выбираем "Auto"


            // TrackBar controls
            if (settings.RAMUsage < trackRamUsage.Minimum)
            {
                settings.RAMUsage = trackRamUsage.Minimum;
            }
            else if (settings.RAMUsage > trackRamUsage.Maximum)
            {
                settings.RAMUsage = trackRamUsage.Maximum;
            }
            trackRamUsage.Value = settings.RAMUsage;


            // RadioButton controls
            radWhite.Checked = settings.radWhite;
            radDark.Checked = settings.radDark;
            radSystem.Checked = settings.radSystem;

            // Для SelectedRadioButtonIndex предполагается выбор радиокнопки по индексу
            switch (settings.SelectedRadioButtonIndex)
            {
                case 0: radWhite.Checked = true; break;
                case 1: radDark.Checked = true; break;
                case 2: radSystem.Checked = true; break;
                default: break;
            }
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

        private Settings CollectSettings()
        {
            return new Settings
            {
                InstallDirectory = txtInstallDirectory.Text,
                LogDirectory = txtLogDirectory.Text,
                ModulesDirectory = txtModulesDirectory.Text,
                CacheDirectory = txtCacheDirectory.Text,
                CPULimit = txtCPULimit.Text,
                GPULimit = txtGPULimit.Text,
                NetworkSpeed = txtNetworkSpeed.Text,
                HotkeyLounch = txtHotkeyLounch.Text,
                CpuLoad = txtCpuLoad.Text,
                GPUEnable = chkGPUEnable.Checked,
                AutoStart = chkAutoStart.Checked,
                UpdPreferen = chkUpdPreferen.Checked,
                AutoUpdate = chkAutoUpdate.Checked,
                UpdateSrartup = chkUpdateSrartup.Checked,
                ManUpdate = chkManUpdate.Checked,
                ProtectionWithaPassword = chkProtectionWithaPassword.Checked,
                AllowedIPAddresses = chkAllowedIPAddresses.Checked,
                RAMUsage = trackRamUsage.Value,
                CpuCores = cmbCpuCores.SelectedItem?.ToString(),
                GpuSelection = cmbGpuSelection.SelectedItem?.ToString(),
                UpdateBranch = cmbUpdateBranch.SelectedItem?.ToString(),
                LogLevel = cmbLogLevel.SelectedItem?.ToString(),
                CheckLogRetention = chkLogRetention.Checked,
                Language = cmbLanguage.SelectedItem?.ToString(),
                Errors = cmbErrors.SelectedItem?.ToString(),
                Warnings = cmbWarnings.SelectedItem?.ToString(),
                InfoMassages = cmbInfoMassages.SelectedItem?.ToString(),
                LogFormat = cmbLogFormat.SelectedItem?.ToString(),
                Debugging = cmbDebugging.SelectedItem?.ToString(),
                radWhite = radWhite.Checked,
                radDark = radDark.Checked,
                radSystem = radSystem.Checked,
                SelectedRadioButtonIndex = radWhite.Checked ? 0 : radDark.Checked ? 1 : 2
            };
        }
        private void SaveAllSettings(Settings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFilePath, json);
        }


        private void txtCPULimit_KeyPress(object sender, KeyPressEventArgs e)
        {
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
            string text = txtNetworkSpeed.Text;
            string trimmedText = text.TrimStart('0');
            if (trimmedText == "")
            {
                trimmedText = "0";
            }
            SaveSettings("NetworkSpeed", trimmedText);
        }
        private void txtHotkeyLounch_TextChanged(object sender, EventArgs e)
        {

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
            var settings = CollectSettings();
            SaveAllSettings(settings);
            MessageBox.Show("Настройки успешно сохранены.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Очистка ресурсов, если есть
            // Например: timer1.Stop();

            if (this.Parent != null)
            {
                this.Parent.Controls.Remove(this);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            var settings = CollectSettings();
            SaveAllSettings(settings);
            if (this.Parent != null)
            {
                this.Parent.Controls.Remove(this);
            }
        }

        // ComboBox

        private void cmbCpuCores_SelectedIndexChanged(object sender, EventArgs e) { }

        private void cmbUpdateBranch_SelectedIndexChanged(object sender, EventArgs e) { }

        private void cmbLogLevel_SelectedIndexChanged(object sender, EventArgs e) { }

        private void cmbErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbErrors.SelectedItem != null)
            {
                SaveSettings("Errors", cmbErrors.SelectedItem.ToString());
            }
        }
        private void cmbLogFormat_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmbDebugging_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmbWarnings_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmbInfoMassages_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmbGpuSelection_SelectedIndexChanged(object sender, EventArgs e) { }

        // CheckBox

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e) { }

        private void chkGPUEnable_CheckedChanged(object sender, EventArgs e) { }

        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e) { }
        private void chkProtectionWithaPassword_CheckedChanged(object sender, EventArgs e) { }

        private void chkAllowedIPAddresses_CheckedChanged(object sender, EventArgs e) { }
        private void chkUpdPreferen_CheckedChanged(object sender, EventArgs e) { }
        private void chkUpdateSrartup_CheckedChanged(object sender, EventArgs e) { }
        private void chkManUpdate_CheckedChanged(object sender, EventArgs e) { }
        private void chkUpdateSrartup_CheckedChanged_1(object sender, EventArgs e)
        {

        }
        private void chkLogRetention_CheckedChanged(object sender, EventArgs e) { }
        // RadioButton
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e) { } 

        private void radSystem_CheckedChanged(object sender, EventArgs e) { }

        // TrackBar
        private void trackRamUsage_Scroll(object sender, EventArgs e) { }


        private void tabPage3_Click(object sender, EventArgs e) { }
        private void tabPage4_Click(object sender, EventArgs e) { }
        private void UserControl1_Load(object sender, EventArgs e) { }
        private void tabPage1_Click(object sender, EventArgs e) { }

        private void txtCpuLoad_TextChanged(object sender, EventArgs e) { }
    }
}