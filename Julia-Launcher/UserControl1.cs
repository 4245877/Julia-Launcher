using Julia_Launcher;
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
        private readonly HardwareInfo hardwareInfo;
        private int previousRamUsage;
        private const long BytesInMegabyte = 1024L * 1024L;

        public UserControl1(HardwareInfo hardwareInfo)
        {
            InitializeComponent();

            this.hardwareInfo = hardwareInfo ?? throw new ArgumentNullException(nameof(hardwareInfo));

            // Создание директории и установка пути к файлу настроек
            string settingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }
            settingsFilePath = Path.Combine(settingsDirectory, "settings.json");

            // Подписка на события TextChanged
            txtInstallDirectory.TextChanged += txtInstallDirectory_TextChanged;
            txtLogDirectory.TextChanged += txtLogDirectory_TextChanged;
            txtModulesDirectory.TextChanged += txtModulesDirectory_TextChanged;
            txtCacheDirectory.TextChanged += txtCacheDirectory_TextChanged;

            // Инициализация с использованием hardwareInfo
            trackRamUsage.Maximum = (int)(hardwareInfo.RamTotalVisibleBytes / BytesInMegabyte);
            previousRamUsage = trackRamUsage.Value;
            InitializeComboBox();
            LoadHardwareInfo();

            // Загрузка настроек
            LoadSettings();
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
        // Добавляем перечисление для тем
        public enum Theme
        {
            White,
            Dark,
            System
        }

        private void LoadSettings()
        {
            var settings = Julia_Launcher.SettingsManager.ReadSettings();

            // TextBox controls
            txtInstallDirectory.Text = settings.InstallDirectory;
            txtLogDirectory.Text = settings.LogDirectory;
            txtModulesDirectory.Text = settings.ModulesDirectory;
            txtCacheDirectory.Text = settings.CacheDirectory;
            txtCPULimit.Text = settings.CPULimit;
            txtGPULimit.Text = settings.GPULimit;
            txtNetworkSpeed.Text = settings.NetworkSpeed;
            txtHotkeyLounch.Text = settings.HotkeyLounch;
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

            // ComboBox controls
            SetComboBoxSelectedItem(cmbCpuCores, settings.CpuCores);
            SetComboBoxSelectedItem(cmbGpuSelection, settings.GpuSelection, 0); // По умолчанию "Auto"
            SetComboBoxSelectedItem(cmbUpdateBranch, settings.UpdateBranch);
            SetComboBoxSelectedItem(cmbLogLevel, settings.LogLevel);
            SetComboBoxSelectedItem(cmbLanguage, settings.Language);
            SetComboBoxSelectedItem(cmbErrors, settings.Errors);
            SetComboBoxSelectedItem(cmbWarnings, settings.Warnings);
            SetComboBoxSelectedItem(cmbInfoMassages, settings.InfoMassages);
            SetComboBoxSelectedItem(cmbLogFormat, settings.LogFormat);
            SetComboBoxSelectedItem(cmbDebugging, settings.Debugging);

            // Установка SelectedIndex для cmbCpuCores
            if (settings.SelectedComboBoxIndex >= 0 && settings.SelectedComboBoxIndex < cmbCpuCores.Items.Count)
            {
                cmbCpuCores.SelectedIndex = settings.SelectedComboBoxIndex; // Теперь это CpuCoresSelectedIndex
            }

            // TrackBar controls
            trackRamUsage.Value = Math.Clamp(settings.RAMUsage, trackRamUsage.Minimum, trackRamUsage.Maximum);

            // RadioButton controls
            switch (settings.SelectedTheme)
            {
                case Theme.White:
                    radWhite.Checked = true;
                    break;
                case Theme.Dark:
                    radDark.Checked = true;
                    break;
                case Theme.System:
                    radSystem.Checked = true;
                    break;
                default:
                    radSystem.Checked = true; // По умолчанию
                    break;
            }
        }

        // Приватный метод для установки значений ComboBox
        private void SetComboBoxSelectedItem(ComboBox comboBox, string value, int defaultIndex = -1)
        {
            if (!string.IsNullOrEmpty(value) && comboBox.Items.Contains(value))
            {
                comboBox.SelectedItem = value;
            }
            else if (defaultIndex >= 0 && defaultIndex < comboBox.Items.Count)
            {
                comboBox.SelectedIndex = defaultIndex;
            }
        }

        private void LoadHardwareInfo()
        {
            if (hardwareInfo != null)
            {
                if (cmbCpuCores != null)
                {
                    cmbCpuCores.Items.Clear();
                    cmbCpuCores.Items.Add(hardwareInfo.CpuCores.ToString());
                    if (cmbCpuCores.Items.Count > 0)
                        cmbCpuCores.SelectedIndex = 0;
                }
            }
        }

        private Julia_Launcher.Settings CollectSettings()
        {
            return new Julia_Launcher.Settings
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
                SelectedTheme = radWhite.Checked ? Theme.White : radDark.Checked ? Theme.Dark : Theme.System
            };
        }

        private void SaveAllSettings(Julia_Launcher.Settings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCPULimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
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



        // Поля с валидацией — оставляем только проверку
        private void txtCPULimit_TextChanged(object sender, EventArgs e)
        {
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
        }

        private void txtNetworkSpeed_TextChanged(object sender, EventArgs e)
        {
            string text = txtNetworkSpeed.Text;
            string trimmedText = text.TrimStart('0');
            if (trimmedText == "")
            {
                trimmedText = "0";
            }
            txtNetworkSpeed.Text = trimmedText;
        }

        private void txtCPULimit_TextChanged_1(object sender, EventArgs e)
        {
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
        }

        // Обработчики кнопок
        private void btnSelectInstallDirectory_Click(object sender, EventArgs e)
        {
            SelectDirectory(txtInstallDirectory, "Select installation folder");
        }

        private void btnSelectLogDirectory_Click(object sender, EventArgs e)
        {
            SelectDirectory(txtLogDirectory, "Select a folder for logs");
        }

        private void btnSelectModulesDirectory_Click(object sender, EventArgs e)
        {
            SelectDirectory(txtModulesDirectory, "Select a folder for modules");
        }

        private void btnSelectCacheDirectory_Click(object sender, EventArgs e)
        {
            SelectDirectory(txtCacheDirectory, "Select a folder for the cache");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var settings = CollectSettings();
            SaveAllSettings(settings);
            MessageBox.Show("The settings have been saved successfully.", "Preservation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
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

        private void txtInstallDirectory_TextChanged(object sender, EventArgs e) { }
        private void txtLogDirectory_TextChanged(object sender, EventArgs e) { }
        private void txtModulesDirectory_TextChanged(object sender, EventArgs e) { }
        private void txtCacheDirectory_TextChanged(object sender, EventArgs e) { }

       
    }
}