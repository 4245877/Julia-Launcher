﻿using System;
using System.Collections.Generic;
using static Julia_Launcher.UserControl1;
namespace Julia_Launcher
{
    // Класс для хранения настроек
    public class Settings
    {
        // Свойства для TextBox
        public string InstallDirectory { get; set; } = string.Empty;
        public string LogDirectory { get; set; } = string.Empty;
        public string ModulesDirectory { get; set; } = string.Empty;
        public string CacheDirectory { get; set; } = string.Empty;
        public string HotkeyLounch { get; set; } = string.Empty;

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
        public bool CheckLogRetention { get; set; } = false;
        public bool ProtectionWithaPassword { get; set; } = false;
        public bool AllowedIPAddresses { get; set; } = false;

        // Свойства для ComboBox
        public string CpuCores { get; set; } = string.Empty;
        public string GpuSelection { get; set; } = string.Empty;
        public string UpdateBranch { get; set; } = string.Empty;
        public string LogLevel { get; set; } = string.Empty;
        public string Errors { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Warnings { get; set; } = string.Empty;
        public string InfoMassages { get; set; } = string.Empty;
        public string LogFormat { get; set; } = string.Empty;
        public string Debugging { get; set; } = string.Empty;


        // Свойства для TextBox
        public string NetworkSpeed { get; set; } = string.Empty;
        public string GPULimit { get; set; } = string.Empty;
        public string CpuLoad { get; set; } = string.Empty;
        public string CPULimit { get; set; } = string.Empty;

        // Свойства для TrackBar
        public int RAMUsage { get; set; } = 4097;

        // Свойства для RadioButton
        public Theme SelectedTheme { get; set; } = Theme.System;

        //UserControl2
        public short Height { get; set; } = 175;
        public short Weight { get; set; } = 65;
        public short Age { get; set; } = 21;
        public short Tone { get; set; } = 3;
        public short Timbre { get; set; } = 2;
        public short SpeechRate { get; set; } = 1;
        public short Volume { get; set; } = 70;


        public Dictionary<string, object> AdditionalSettings { get; set; } = new Dictionary<string, object>();
    }
}