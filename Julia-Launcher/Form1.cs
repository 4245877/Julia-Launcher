using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Julia_Launcher
{
    public partial class Form1 : Form
    {
        private bool isInstalled = false;
        private readonly string hardwareInfoFilePath;

        public Form1()
        {
            InitializeComponent();
            UpdateUI();

            // Создаем директорию settings, если ее нет
            string settingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            hardwareInfoFilePath = Path.Combine(settingsDirectory, "hardware_info.json");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await CollectHardwareInfoAsync();
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

        private async Task CollectHardwareInfoAsync()
        {
            try
            {
                HardwareInfo hardwareInfo = new HardwareInfo();
                await hardwareInfo.CollectAllDataAsync();
                string json = JsonSerializer.Serialize(hardwareInfo, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(hardwareInfoFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сборе информации о железе: {ex.Message}");
                MessageBox.Show($"Ошибка при сборе информации о железе: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHardwareInfo()
        {
            try
            {
                if (File.Exists(hardwareInfoFilePath))
                {
                    string json = File.ReadAllText(hardwareInfoFilePath);
                    ComputerInfo = JsonSerializer.Deserialize<HardwareInfo>(json);
                }
                else
                {
                    MessageBox.Show("Файл с информацией о железе не найден.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке информации о железе: {ex.Message}");
                MessageBox.Show("Не удалось загрузить информацию о железе.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static HardwareInfo ComputerInfo { get; private set; }

        // Класс для хранения информации о железе


        public class CpuInfo
        {
            public string Name { get; set; }
            public string Manufacturer { get; set; }
            public int Cores { get; set; }
            public int LogicalProcessors { get; set; }
            public int MaxFrequency { get; set; }
            public string Architecture { get; set; }
        }

        public class RamInfo
        {
            public long TotalSize { get; set; }
            public List<RamModule> Modules { get; set; } = new List<RamModule>();
        }

        public class RamModule
        {
            public string Manufacturer { get; set; }
            public ulong Size { get; set; }
            public int Speed { get; set; }
        }

        public class GpuInfo
        {
            public string Name { get; set; }
            public string Manufacturer { get; set; }
            public long Memory { get; set; }
            public string Resolution { get; set; }
            public int RefreshRate { get; set; }
        }

        // Остальные методы формы
        private void LoadUserControl(UserControl control)
        {
            panel1.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel1.Controls.Add(control);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UserControl1());
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
            string installedFilePath = Path.Combine(Path.GetTempPath(), "installed_app.exe");
            string installerFilePath = Path.Combine(Path.GetTempPath(), "installer.exe");
            string downloadUrl = "https://drive.google.com/uc?export=download&id=1khCoBVr65P49kGOsEfRy9po_XKjlNQiR";

            if (File.Exists(installedFilePath))
            {
                DialogResult result = MessageBox.Show("Продукт уже установлен. Запустить его?", "Продукт готов", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try { Process.Start(installedFilePath); }
                    catch (Exception ex) { MessageBox.Show($"Ошибка при запуске: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                return;
            }

            progressBar.Visible = true;
            progressBar.Value = 0;

            try
            {
                using (var cts = new CancellationTokenSource())
                using (var httpClient = new HttpClient())
                {
                    // Скачивание
                    bool needToDownload = true;
                    HttpResponseMessage headResponse = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                    headResponse.EnsureSuccessStatusCode();
                    long? expectedSize = headResponse.Content.Headers.ContentLength;

                    if (File.Exists(installerFilePath))
                    {
                        long localSize = new FileInfo(installerFilePath).Length;
                        needToDownload = !(expectedSize.HasValue && localSize == expectedSize.Value);
                        if (needToDownload) File.Delete(installerFilePath);
                    }

                    if (needToDownload)
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                        response.EnsureSuccessStatusCode();
                        long? totalBytes = response.Content.Headers.ContentLength;
                        long bytesRead = 0;

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        using (FileStream fileStream = new FileStream(installerFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            byte[] buffer = new byte[8192];
                            int bytesReadThisTime;
                            while ((bytesReadThisTime = await contentStream.ReadAsync(buffer, 0, buffer.Length, cts.Token)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesReadThisTime, cts.Token);
                                bytesRead += bytesReadThisTime;
                                if (totalBytes.HasValue && totalBytes > 0)
                                    progressBar.Value = (int)((bytesRead * 100) / totalBytes.Value);
                            }
                        }
                    }

                    // Установка
                    if (File.Exists(installerFilePath))
                    {
                        Process installerProcess = Process.Start(installerFilePath);
                        await Task.Run(() => installerProcess.WaitForExit());
                        if (File.Exists(installedFilePath))
                        {
                            isInstalled = true;
                            UpdateUI();
                        }
                        else
                        {
                            MessageBox.Show("Установка завершилась, но продукт не установлен.", "Ошибка установки", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void progressBar_Click(object sender, EventArgs e) { }

        private void pictureBoxInfo_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "https://4245877.github.io/Julia_site/",
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}