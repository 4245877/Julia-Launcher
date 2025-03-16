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
        // Путь для сохранения информации о системе
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
            // Перенесена логика из конструктора
            CollectHardwareInfo();
            LoadHardwareInfo();
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
            using (ManagementObjectCollection collection = searcher.Get())
            {
                foreach (ManagementObject obj in collection)
                {
                    using (obj)
                    {
                        hardwareInfo.AppendLine($"Название: {SafeGetProperty(obj, "Name")}");
                        hardwareInfo.AppendLine($"Производитель: {SafeGetProperty(obj, "Manufacturer")}");
                        hardwareInfo.AppendLine($"Количество ядер: {SafeGetProperty(obj, "NumberOfCores")}");
                        hardwareInfo.AppendLine($"Количество логических процессоров: {SafeGetProperty(obj, "NumberOfLogicalProcessors")}");
                        hardwareInfo.AppendLine($"Максимальная тактовая частота: {SafeGetProperty(obj, "MaxClockSpeed")} МГц");
                        hardwareInfo.AppendLine($"Архитектура: {SafeGetProperty(obj, "AddressWidth")} бит");
                    }
                }
            }
        }

        private void CollectMemoryInfo(StringBuilder hardwareInfo)
        {
            hardwareInfo.AppendLine("\n=== Оперативная память ===");
            ulong totalMemory = 0;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory"))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                foreach (ManagementObject obj in collection)
                {
                    using (obj)
                    {
                        object capacityObj = obj["Capacity"];
                        if (capacityObj != null)
                        {
                            ulong capacity = Convert.ToUInt64(capacityObj);
                            totalMemory += capacity;
                            hardwareInfo.AppendLine($"Модуль ОЗУ: {SafeGetProperty(obj, "Manufacturer")} - {capacity / (1024 * 1024)} МБ");
                            hardwareInfo.AppendLine($"Формат: {SafeGetProperty(obj, "FormFactor")}");
                            hardwareInfo.AppendLine($"Тип: {SafeGetProperty(obj, "MemoryType")}");
                            hardwareInfo.AppendLine($"Скорость: {SafeGetProperty(obj, "Speed")} МГц");
                        }
                    }
                }
            }
            hardwareInfo.AppendLine($"Общий объем ОЗУ: {totalMemory / (1024 * 1024 * 1024)} ГБ");
        }

        private void CollectVideoInfo(StringBuilder hardwareInfo)
        {
            hardwareInfo.AppendLine("\n=== Видеокарта ===");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                foreach (ManagementObject obj in collection)
                {
                    using (obj)
                    {
                        hardwareInfo.AppendLine($"Название: {SafeGetProperty(obj, "Name")}");
                        hardwareInfo.AppendLine($"Производитель: {SafeGetProperty(obj, "AdapterCompatibility")}");
                        hardwareInfo.AppendLine($"Видеопроцессор: {SafeGetProperty(obj, "VideoProcessor")}");
                        hardwareInfo.AppendLine($"Версия драйвера: {SafeGetProperty(obj, "DriverVersion")}");

                        // Безопасное получение AdapterRAM с проверкой на null
                        string memorySize = "Нет данных";
                        object adapterRAM = obj["AdapterRAM"];
                        if (adapterRAM != null && adapterRAM != DBNull.Value)
                        {
                            try
                            {
                                memorySize = $"{Convert.ToUInt64(adapterRAM) / (1024 * 1024)} МБ";
                            }
                            catch (Exception)
                            {
                                memorySize = "Ошибка определения";
                            }
                        }
                        hardwareInfo.AppendLine($"Объем памяти: {memorySize}");

                        hardwareInfo.AppendLine($"Разрешение экрана: {SafeGetProperty(obj, "CurrentHorizontalResolution")} x {SafeGetProperty(obj, "CurrentVerticalResolution")}");
                        hardwareInfo.AppendLine($"Частота обновления: {SafeGetProperty(obj, "CurrentRefreshRate")} Гц");
                    }
                }
            }
        }

        // Метод для безопасного получения свойств из WMI объектов
        private string SafeGetProperty(ManagementObject obj, string propertyName)
        {
            try
            {
                object value = obj[propertyName];
                return value != null && value != DBNull.Value ? value.ToString() : "Нет данных";
            }
            catch
            {
                return "Нет данных";
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
            if (!ComputerInfo.ReadFromFile(hardwareInfoFilePath))
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
            public long RamTotalSize { get; set; } // Изменен на long для избежания переполнения
            public List<RamModule> RamModules { get; set; } = new List<RamModule>();

            // Данные о видеокарте
            public string GpuName { get; set; }
            public string GpuManufacturer { get; set; }
            public long GpuMemory { get; set; } // Изменен на long для избежания переполнения
            public string GpuResolution { get; set; }
            public int GpuRefreshRate { get; set; }

            // Вспомогательный класс для модулей ОЗУ
            public class RamModule
            {
                public string Manufacturer { get; set; }
                public ulong Size { get; set; }
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

                        try
                        {
                            if (currentSection == "CPU")
                            {
                                ParseCpuLine(line);
                            }
                            else if (currentSection == "RAM")
                            {
                                ParseRamLine(line);
                            }
                            else if (currentSection == "GPU")
                            {
                                ParseGpuLine(line);
                            }
                        }
                        catch (Exception)
                        {
                            // Игнорируем ошибки парсинга для отдельных строк
                            continue;
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            private void ParseCpuLine(string line)
            {
                if (line.StartsWith("Название:"))
                    CpuName = line.Substring("Название:".Length).Trim();
                else if (line.StartsWith("Производитель:"))
                    CpuManufacturer = line.Substring("Производитель:".Length).Trim();
                else if (line.StartsWith("Количество ядер:"))
                {
                    if (int.TryParse(line.Substring("Количество ядер:".Length).Trim(), out int cores))
                        CpuCores = cores;
                }
                else if (line.StartsWith("Количество логических процессоров:"))
                {
                    if (int.TryParse(line.Substring("Количество логических процессоров:".Length).Trim(), out int logicalProcessors))
                        CpuLogicalProcessors = logicalProcessors;
                }
                else if (line.StartsWith("Максимальная тактовая частота:"))
                {
                    string[] parts = line.Substring("Максимальная тактовая частота:".Length).Trim().Split(' ');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int frequency))
                        CpuMaxFrequency = frequency;
                }
                else if (line.StartsWith("Архитектура:"))
                    CpuArchitecture = line.Substring("Архитектура:".Length).Trim();
            }

            private void ParseRamLine(string line)
            {
                if (line.StartsWith("Модуль ОЗУ:"))
                {
                    string moduleInfo = line.Substring("Модуль ОЗУ:".Length).Trim();
                    string[] parts = moduleInfo.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 2)
                    {
                        var manufacturer = parts[0].Trim();
                        string[] sizeParts = parts[1].Trim().Split(' ');

                        if (sizeParts.Length > 0 && ulong.TryParse(sizeParts[0], out ulong size))
                        {
                            RamModules.Add(new RamModule
                            {
                                Manufacturer = manufacturer,
                                Size = size * 1024 * 1024 // Переводим МБ в байты
                            });
                        }
                    }
                }
                else if (line.StartsWith("Скорость:"))
                {
                    string[] parts = line.Substring("Скорость:".Length).Trim().Split(' ');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int speed) && RamModules.Count > 0)
                    {
                        RamModules[RamModules.Count - 1].Speed = speed;
                    }
                }
                else if (line.StartsWith("Общий объем ОЗУ:"))
                {
                    string[] parts = line.Substring("Общий объем ОЗУ:".Length).Trim().Split(' ');
                    if (parts.Length > 0 && long.TryParse(parts[0], out long totalSize))
                        RamTotalSize = totalSize;
                }
            }



            private void ParseGpuLine(string line)
            {
                if (line.StartsWith("Название:"))
                    GpuName = line.Substring("Название:".Length).Trim();
                else if (line.StartsWith("Производитель:"))
                    GpuManufacturer = line.Substring("Производитель:".Length).Trim();
                else if (line.StartsWith("Объем памяти:"))
                {
                    string[] parts = line.Substring("Объем памяти:".Length).Trim().Split(' ');
                    if (parts.Length > 0)
                    {
                        // Проверяем, что это число, а не "Нет данных" или "Ошибка определения"
                        if (long.TryParse(parts[0], out long memory))
                            GpuMemory = memory;
                    }
                }
                else if (line.StartsWith("Разрешение экрана:"))
                    GpuResolution = line.Substring("Разрешение экрана:".Length).Trim();
                else if (line.StartsWith("Частота обновления:"))
                {
                    string[] parts = line.Substring("Частота обновления:".Length).Trim().Split(' ');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int refreshRate))
                        GpuRefreshRate = refreshRate;
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