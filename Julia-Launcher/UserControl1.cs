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
            if (hardwareInfo != null)
            {
                LoadHardwareInfo();
            }
            else
            {
                MessageBox.Show("Информация о железе недоступна.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            radWhite.Checked = settings.radWhite;
            radDark.Checked = settings.radDark;
            radSystem.Checked = settings.radSystem;
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
            SaveSettings("HotkeyLounch", txtHotkeyLounch.Text);
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
            if (this.ParentForm != null)
            {
                this.ParentForm.Close();
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {

        }

        // ComboBox

        private void cmbCpuCores_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettings("CpuCores", cmbCpuCores.SelectedItem.ToString());
        }

        private void cmbUpdateBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettings("UpdateBranch", cmbUpdateBranch.SelectedItem.ToString());
        }

        private void cmbLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettings("LogLevel", cmbLogLevel.SelectedItem.ToString());
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
            SaveSettings("LogFormat", cmbLogFormat.SelectedItem.ToString());
        }
        private void cmbDebugging_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettings("Debugging", cmbDebugging.SelectedItem.ToString());
        }
        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettings("Language", cmbLanguage.SelectedItem.ToString());
        }
        private void cmbWarnings_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettings("Warnings", cmbWarnings.SelectedItem.ToString());
        }
        private void cmbInfoMassages_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSettings("InfoMassages", cmbInfoMassages.SelectedItem.ToString());
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
            SaveSettings("AutoUpdate", chkAutoUpdate.Checked);
        }
        private void chkProtectionWithaPassword_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("ProtectionWithaPassword", chkProtectionWithaPassword.Checked);
        }

        private void chkAllowedIPAddresses_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("AllowedIPAddresses", chkAllowedIPAddresses.Checked);
        }
        private void chkUpdPreferen_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("UpdPreferen", chkUpdPreferen.Checked);
        }
        private void chkUpdateSrartup_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("UpdateSrartup", chkUpdateSrartup.Checked);
        }
        private void chkManUpdate_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("ManUpdate", chkManUpdate.Checked);
        }
        private void chkUpdateSrartup_CheckedChanged_1(object sender, EventArgs e)
        {
            SaveSettings("UpdateSrartup", chkUpdateSrartup.Checked);
        }
        private void chkLogRetention_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("LogRetention", chkLogRetention.Checked);
        }




        // RadioButton

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("ThemeWhite", radWhite.Checked);
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            SaveSettings("ThemeDark", radDark.Checked);
        }

        private void radSystem_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings("ThemeSystem", radSystem.Checked);
        }

        // TrackBar

        private void trackRamUsage_Scroll(object sender, EventArgs e)
        {
            SaveSettings("RAMUsage", trackRamUsage.Value);
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
    }
}