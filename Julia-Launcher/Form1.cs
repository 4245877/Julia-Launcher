using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace Julia_Launcher
{
    public partial class Form1 : Form
    {
        // Изменяем путь для сохранения информации о системе
        private readonly string hardwareInfoFilePath;

        public Form1()
        {
            InitializeComponent();

            // Создаем директорию settings, если она не существует
            string settingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            // Устанавливаем путь к файлу с информацией о железе
            hardwareInfoFilePath = Path.Combine(settingsDirectory, "hardware_info.txt");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CollectHardwareInfo();
        }

        private void CollectHardwareInfo()
        {
            try
            {
                StringBuilder hardwareInfo = new StringBuilder();
                hardwareInfo.AppendLine("=== Характеристики компьютера ===");
                hardwareInfo.AppendLine($"Дата и время сбора: {DateTime.Now}");
                hardwareInfo.AppendLine();

                // Сбор информации о процессоре
                CollectProcessorInfo(hardwareInfo);

                // Сбор информации об оперативной памяти
                CollectMemoryInfo(hardwareInfo);

                // Сбор информации о видеокарте
                CollectVideoInfo(hardwareInfo);

                // Запись информации в файл
                File.WriteAllText(hardwareInfoFilePath, hardwareInfo.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сборе информации о характеристиках компьютера: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CollectProcessorInfo(StringBuilder hardwareInfo)
        {
            hardwareInfo.AppendLine("=== Процессор ===");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    hardwareInfo.AppendLine($"Название: {obj["Name"]}");
                    hardwareInfo.AppendLine($"Производитель: {obj["Manufacturer"]}");
                    hardwareInfo.AppendLine($"Количество ядер: {obj["NumberOfCores"]}");
                    hardwareInfo.AppendLine($"Количество логических процессоров: {obj["NumberOfLogicalProcessors"]}");
                    hardwareInfo.AppendLine($"Максимальная тактовая частота: {obj["MaxClockSpeed"]} МГц");
                    hardwareInfo.AppendLine($"Архитектура: {obj["AddressWidth"]} бит");
                }
            }
        }

        private void CollectMemoryInfo(StringBuilder hardwareInfo)
        {
            hardwareInfo.AppendLine("\n=== Оперативная память ===");
            ulong totalMemory = 0;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    ulong capacity = Convert.ToUInt64(obj["Capacity"]);
                    totalMemory += capacity;
                    hardwareInfo.AppendLine($"Модуль ОЗУ: {obj["Manufacturer"]} - {capacity / (1024 * 1024)} МБ");
                    hardwareInfo.AppendLine($"Формат: {obj["FormFactor"]}");
                    hardwareInfo.AppendLine($"Тип: {obj["MemoryType"]}");
                    hardwareInfo.AppendLine($"Скорость: {obj["Speed"]} МГц");
                }
            }
            hardwareInfo.AppendLine($"Общий объем ОЗУ: {totalMemory / (1024 * 1024 * 1024)} ГБ");
        }

        private void CollectVideoInfo(StringBuilder hardwareInfo)
        {
            hardwareInfo.AppendLine("\n=== Видеокарта ===");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    hardwareInfo.AppendLine($"Название: {obj["Name"]}");
                    hardwareInfo.AppendLine($"Производитель: {obj["AdapterCompatibility"]}");
                    hardwareInfo.AppendLine($"Видеопроцессор: {obj["VideoProcessor"]}");
                    hardwareInfo.AppendLine($"Версия драйвера: {obj["DriverVersion"]}");
                    hardwareInfo.AppendLine($"Объем памяти: {Convert.ToUInt64(obj["AdapterRAM"]) / (1024 * 1024)} МБ");
                    hardwareInfo.AppendLine($"Разрешение экрана: {obj["CurrentHorizontalResolution"]} x {obj["CurrentVerticalResolution"]}");
                    hardwareInfo.AppendLine($"Частота обновления: {obj["CurrentRefreshRate"]} Гц");
                }
            }
        }

        // Общий метод для загрузки UserControl в панель
        private void LoadUserControl(UserControl control)
        {
            panel1.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel1.Controls.Add(control);
        }




        public class SystemHardwareInfo
        {
            public int CpuCores { get; set; }            // Количество ядер процессора
            public List<string> GpuList { get; set; }    // Список видеокарт
            public int TotalMemoryGB { get; set; }       // Общий объем ОЗУ в гигабайтах
        }



























        // Обработчики событий кнопок
        private void button2_Click(object sender, EventArgs e)
        {
            // Пустой метод в исходном коде
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UserControl1());
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Пустой метод в исходном коде
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Пустой метод в исходном коде
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UserControl2());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Пустой метод в исходном коде
        }
    }
}