using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text;
using System.Windows.Forms;
using System.Net;         // Для работы с WebClient
using System.Diagnostics; // Для запуска процесса

namespace Julia_Launcher
{
    public partial class Form1 : Form
    {
        private bool isInstalled = false;
        private BackgroundWorker backgroundWorker1; // Объявление поля

        // Путь для сохранения информации о системе
        private readonly string hardwareInfoFilePath;

        public Form1()
        {
            InitializeComponent();

            // Инициализация BackgroundWorker
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;

            UpdateUI();

            // Создаем директорию settings, если она не существует
            string settingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            hardwareInfoFilePath = Path.Combine(settingsDirectory, "hardware_info.txt");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Перенесена логика из конструктора
            CollectHardwareInfo();
            LoadHardwareInfo();
        }
        private void UpdateUI()
        {
            if (!isInstalled)
            {
                button1.Text = "Install";
                progressBar.Visible = false;
            }
            else
            {
                button1.Text = "Launch";
                progressBar.Visible = false;
            }
        }
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isInstalled = true;
            progressBar.Visible = false;
            UpdateUI();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                System.Threading.Thread.Sleep(50);
                backgroundWorker1.ReportProgress(i);
            }
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
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                int gpuIndex = 0; // Индекс для нумерации видеокарт
                foreach (ManagementObject obj in collection)
                {
                    using (obj)
                    {
                        hardwareInfo.AppendLine($"\n=== Видеокарта {gpuIndex} ==="); // Уникальный заголовок для каждой видеокарты
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

                        gpuIndex++; // Увеличиваем индекс для следующей видеокарты
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

        public class GpuInfo
        {
            public string Name { get; set; }
            public string Manufacturer { get; set; }
            public long Memory { get; set; }
            public string Resolution { get; set; }
            public int RefreshRate { get; set; }
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
            public List<GpuInfo> Gpus { get; set; } = new List<GpuInfo>();
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
            private void ParseGpuLine(string line, GpuInfo gpu)
            {
                if (line.StartsWith("Название:"))
                    gpu.Name = line.Substring("Название:".Length).Trim();
                else if (line.StartsWith("Производитель:"))
                    gpu.Manufacturer = line.Substring("Производитель:".Length).Trim();
                else if (line.StartsWith("Объем памяти:"))
                {
                    string[] parts = line.Substring("Объем памяти:".Length).Trim().Split(' ');
                    if (parts.Length > 0 && long.TryParse(parts[0], out long memory))
                        gpu.Memory = memory;
                }
                else if (line.StartsWith("Разрешение экрана:"))
                    gpu.Resolution = line.Substring("Разрешение экрана:".Length).Trim();
                else if (line.StartsWith("Частота обновления:"))
                {
                    string[] parts = line.Substring("Частота обновления:".Length).Trim().Split(' ');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int refreshRate))
                        gpu.RefreshRate = refreshRate;
                }
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
                            {
                                currentSection = "CPU";
                            }
                            else if (line.Contains("Оперативная память"))
                            {
                                currentSection = "RAM";
                            }
                            else if (line.Contains("Видеокарта"))
                            {
                                // Новая видеокарта начинается с секции "=== Видеокарта N ==="
                                Gpus.Add(new GpuInfo());
                                currentSection = "GPU";
                            }
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
                            else if (currentSection == "GPU" && Gpus.Count > 0)
                            {
                                // Парсим данные в последнюю добавленную видеокарту
                                ParseGpuLine(line, Gpus[Gpus.Count - 1]);
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
            string settingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
            if (Directory.Exists(settingsDirectory))
            {
                System.Diagnostics.Process.Start("explorer.exe", settingsDirectory);
            }
            else
            {
                MessageBox.Show("Папка настроек не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UserControl2());
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (isInstalled)
            {
                // Если продукт уже установлен, запускаем его (добавьте нужную логику)
                MessageBox.Show("Продукт уже установлен. Добавьте логику запуска здесь.");
                return;
            }

            // URL для загрузки файла с Google Диска
            string downloadUrl = "https://drive.google.com/uc?export=download&id=YOUR_FILE_ID"; // Замените YOUR_FILE_ID на реальный ID файла
            string localFilePath = Path.Combine(Path.GetTempPath(), "installer.exe"); // Путь для сохранения файла во временной папке

            // Показываем progressBar
            progressBar.Visible = true;
            progressBar.Value = 0;

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    // Обновление progressBar при изменении прогресса загрузки
                    webClient.DownloadProgressChanged += (s, ev) =>
                    {
                        progressBar.Value = ev.ProgressPercentage;
                    };

                    // Действия после завершения загрузки
                    webClient.DownloadFileCompleted += (s, ev) =>
                    {
                        if (ev.Error == null)
                        {
                            // Запускаем установочный файл
                            Process.Start(localFilePath);
                            isInstalled = true;
                            UpdateUI();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при загрузке файла: " + ev.Error.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        progressBar.Visible = false;
                    };

                    // Асинхронная загрузка файла
                    await webClient.DownloadFileTaskAsync(new Uri(downloadUrl), localFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progressBar.Visible = false;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void progressBar_Click(object sender, EventArgs e)
        {

        }
    }
}