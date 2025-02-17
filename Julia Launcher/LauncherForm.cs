using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;

namespace Julia_Launcher
{
    public partial class MainForm : Form
    {




        public MainForm()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }

    // Классы данных
    public class Settings
    {
        public string AIPath { get; set; }
        public int AllocatedRAM { get; set; } = 4096;
        public CharacterSettings Character { get; set; } = new CharacterSettings();
    }

    public class CharacterSettings
    {
        public string HairColor { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
    }
    public class SystemInfo
    {
        public string CPU { get; set; }
        public long RAM { get; set; } // в байтах
    }

}
