using Microsoft.VisualBasic.Devices;
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

            CollectHardwareInfo(); // Сначала собираем информацию
            LoadHardwareInfo();    // Затем загружаем ее
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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


        private void LoadHardwareInfo()
        {
            ComputerInfo = new HardwareInfo();
            if (!ComputerInfo.ReadFromFile(hardwareInfoFilePath)) // Используем hardwareInfoFilePath
            {
                MessageBox.Show("Не удалось загрузить информацию о характеристиках компьютера.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public class HardwareInfo
        {
            // Данные о процессоре
            public string CpuName { get; set; }
            public string CpuManufacturer { get; set; }
            public int CpuCores { get; set; }
            public int CpuLogicalProcessors { get; set; }
            public int CpuMaxFrequency { get; set; }
            public string CpuArchitecture { get; set; }

            // Данные об оперативной памяти
            public int RamTotalSize { get; set; }
            public List<RamModule> RamModules { get; set; } = new List<RamModule>(); // Новый список

            // Данные о видеокарте
            public string GpuName { get; set; }
            public string GpuManufacturer { get; set; }
            public int GpuMemory { get; set; }
            public string GpuResolution { get; set; }
            public int GpuRefreshRate { get; set; }

            // Вспомогательный класс для модулей ОЗУ
            public class RamModule
            {
                public string Manufacturer { get; set; }
                public ulong Size { get; set; } // Используем ulong для больших объемов
                public int Speed { get; set; }
            }

            public bool ReadFromFile(string filePath)
            {
                try
                {
                    if (!File.Exists(filePath))
                        return false;

                    string[] lines = File.ReadAllLines(filePath);
                    string currentSection = "";

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        if (line.StartsWith("=== "))
                        {
                            if (line.Contains("Процессор"))
                                currentSection = "CPU";
                            else if (line.Contains("Оперативная память"))
                                currentSection = "RAM";
                            else if (line.Contains("Видеокарта"))
                                currentSection = "GPU";
                            continue;
                        }

                        if (currentSection == "CPU")
                        {
                            if (line.StartsWith("Название:"))
                                CpuName = line.Substring("Название:".Length).Trim();
                            else if (line.StartsWith("Производитель:"))
                                CpuManufacturer = line.Substring("Производитель:".Length).Trim();
                            else if (line.StartsWith("Количество ядер:"))
                                CpuCores = int.Parse(line.Substring("Количество ядер:".Length).Trim());
                            else if (line.StartsWith("Количество логических процессоров:"))
                                CpuLogicalProcessors = int.Parse(line.Substring("Количество логических процессоров:".Length).Trim());
                            else if (line.StartsWith("Максимальная тактовая частота:"))
                                CpuMaxFrequency = int.Parse(line.Substring("Максимальная тактовая частота:".Length).Split(' ')[0].Trim());
                            else if (line.StartsWith("Архитектура:"))
                                CpuArchitecture = line.Substring("Архитектура:".Length).Trim();
                        }
                        else if (currentSection == "RAM")
                        {
                            if (line.StartsWith("Модуль ОЗУ:"))
                            {
                                var parts = line.Substring("Модуль ОЗУ:".Length).Trim().Split('-');
                                var manufacturer = parts[0].Trim();
                                var size = ulong.Parse(parts[1].Trim().Split(' ')[0]) * 1024 * 1024; // Переводим МБ в байты
                                RamModules.Add(new RamModule { Manufacturer = manufacturer, Size = size });
                            }
                            else if (line.StartsWith("Скорость:"))
                            {
                                var speed = int.Parse(line.Substring("Скорость:".Length).Trim().Split(' ')[0]);
                                if (RamModules.Count > 0)
                                    RamModules[RamModules.Count - 1].Speed = speed;
                            }
                            else if (line.StartsWith("Общий объем ОЗУ:"))
                                RamTotalSize = int.Parse(line.Substring("Общий объем ОЗУ:".Length).Trim().Split(' ')[0]);
                        }
                        else if (currentSection == "GPU")
                        {
                            if (line.StartsWith("Название:"))
                                GpuName = line.Substring("Название:".Length).Trim();
                            else if (line.StartsWith("Производитель:"))
                                GpuManufacturer = line.Substring("Производитель:".Length).Trim();
                            else if (line.StartsWith("Объем памяти:"))
                                GpuMemory = int.Parse(line.Substring("Объем памяти:".Length).Trim().Split(' ')[0]);
                            else if (line.StartsWith("Разрешение экрана:"))
                                GpuResolution = line.Substring("Разрешение экрана:".Length).Trim();
                            else if (line.StartsWith("Частота обновления:"))
                                GpuRefreshRate = int.Parse(line.Substring("Частота обновления:".Length).Trim().Split(' ')[0]);
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Статический экземпляр класса HardwareInfo для доступа из других форм
        public static HardwareInfo ComputerInfo { get; private set; }






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