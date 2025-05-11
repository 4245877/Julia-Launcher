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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Julia_Launcher
{
    public partial class Form1 : Form
    {
        private bool isInstalled = false;
        private readonly string hardwareInfoFilePath;
        private readonly IHttpClientFactory _httpClientFactory;
        private CancellationTokenSource cts;

        public Form1()
        {
            InitializeComponent();
            UpdateUI();

            // Настройка DI
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

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
            // Проверяем актуальность файла (например, 1 день)
            if (!File.Exists(hardwareInfoFilePath) || File.GetLastWriteTime(hardwareInfoFilePath) < DateTime.Now.AddDays(-1))
            {
                await Task.Run(async () => await CollectHardwareInfoAsync());
            }
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
                Log("Информация о железе успешно собрана.");
            }
            catch (ManagementException mex)
            {
                Log($"Ошибка WMI: {mex.Message}");
                MessageBox.Show($"Ошибка WMI: {mex.Message}", "Ошибка оборудования", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Log($"Ошибка при сборе информации о железе: {ex.Message}");
                MessageBox.Show($"Ошибка при сборе информации о железе: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Файл с информацией о железе не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при загрузке информации о железе: {ex.Message}");
                MessageBox.Show("Не удалось загрузить информацию о железе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static HardwareInfo ComputerInfo { get; private set; }

        // Классы для хранения информации о железе
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
            labelStatus.Text = "Проверка файла...";

            try
            {
                cts = new CancellationTokenSource();
                var httpClient = _httpClientFactory.CreateClient();
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
                    labelStatus.Text = "Скачивание...";
                    HttpResponseMessage response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                    response.EnsureSuccessStatusCode();
                    long? totalBytes = response.Content.Headers.ContentLength;
                    long bytesRead = 0;
                    long lastReportedBytes = 0;
                    long reportInterval = 102400; // 100 КБ

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(installerFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[8192];
                        int bytesReadThisTime;
                        while ((bytesReadThisTime = await contentStream.ReadAsync(buffer, 0, buffer.Length, cts.Token)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesReadThisTime, cts.Token);
                            bytesRead += bytesReadThisTime;
                            if (totalBytes.HasValue && totalBytes > 0 && bytesRead - lastReportedBytes >= reportInterval)
                            {
                                progressBar.Value = (int)((bytesRead * 100) / totalBytes.Value);
                                lastReportedBytes = bytesRead;
                            }
                        }
                        progressBar.Value = 100;
                    }
                    labelStatus.Text = "Скачивание завершено.";
                }

                if (File.Exists(installerFilePath))
                {
                    labelStatus.Text = "Установка...";
                    Process installerProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo(installerFilePath),
                        EnableRaisingEvents = true
                    };
                    installerProcess.Exited += (s, ev) =>
                    {
                        if (installerProcess.ExitCode == 0 && File.Exists(installedFilePath))
                        {
                            isInstalled = true;
                            UpdateUI();
                            labelStatus.Text = "Установка завершена.";
                            Log("Установка успешно завершена.");
                        }
                        else
                        {
                            MessageBox.Show($"Установка завершилась с ошибкой. Код: {installerProcess.ExitCode}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            labelStatus.Text = "Ошибка установки.";
                            Log($"Ошибка установки. Код: {installerProcess.ExitCode}");
                        }
                    };
                    installerProcess.Start();
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Загрузка отменена.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                labelStatus.Text = "Загрузка отменена.";
                Log("Загрузка отменена пользователем.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Ошибка.";
                Log($"Ошибка при загрузке/установке: {ex.Message}");
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
        }

        private void Log(string message)
        {
            File.AppendAllText("log.txt", $"{DateTime.Now}: {message}\n");
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