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

namespace Julia_Launcher
{
    public partial class Form1 : Form
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
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
            try
            {
                if (!File.Exists(hardwareInfoFilePath) || File.GetLastWriteTime(hardwareInfoFilePath) < DateTime.Now.AddDays(-1))
                {
                    await Task.Run(CollectHardwareInfoAsync);
                }
                await LoadHardwareInfoAsync(); // Асинхронный вызов
            }
            catch (Exception ex)
            {
                Log($"Error in Form1_Load: {ex.Message}");
                MessageBox.Show("There was an error loading the form.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                Log("Hardware information successfully collected.");
            }
            catch (ManagementException mex)
            {
                Log($"Error WMI: {mex.Message}");
                MessageBox.Show($"Error WMI: {mex.Message}", "Hardware error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Log($"Error collecting hardware information: {ex.Message}");
                MessageBox.Show($"Error collecting hardware information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadHardwareInfoAsync()
        {
            try
            {
                if (File.Exists(hardwareInfoFilePath))
                {
                    string json = await File.ReadAllTextAsync(hardwareInfoFilePath);
                    ComputerInfo = JsonSerializer.Deserialize<HardwareInfo>(json, _jsonSerializerOptions);
                }
                else
                {
                    MessageBox.Show("Hardware information file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Log($"Error loading hardware information: {ex.Message}");
                MessageBox.Show("Failed to load hardware information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                DialogResult result = MessageBox.Show("The product is already installed. Run it?", "The product is ready", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try { Process.Start(installedFilePath); }
                    catch (Exception ex) { MessageBox.Show($"Error while starting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                return;
            }

            progressBar.Visible = true;
            progressBar.Value = 0;
            labelStatus.Text = "Checking file...";

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
                    labelStatus.Text = "Download...";
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
                    labelStatus.Text = "Download complete.";
                }

                if (File.Exists(installerFilePath))
                {
                    labelStatus.Text = "Installation...";
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
                            labelStatus.Text = "Installation is complete.";
                            Log("Installation completed successfully.");
                        }
                        else
                        {
                            MessageBox.Show($"Installation failed with error. Code: {installerProcess.ExitCode}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            labelStatus.Text = "Installation error.";
                            Log($"Installation error. Code: {installerProcess.ExitCode}");
                        }
                    };
                    installerProcess.Start();
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Upload cancelled.", "Cancel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                labelStatus.Text = "Upload cancelled.";
                Log("Upload cancelled by user.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "Error.";
                Log($"Error while downloading/installing: {ex.Message}");
            }
            finally
            {
                progressBar.Visible = false;
            }
        }


        public class HardwareInfoService
        {
            public HardwareInfo ComputerInfo { get; private set; }
            public async Task LoadHardwareInfoAsync(string filePath) { /* логика загрузки */ }
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








        private void pictureBoxInfo_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "https://4245877.github.io/Julia_site/",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void btnModelVoiceSettings_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UserControl2());
        }

        private void btnCoreFolder_Click(object sender, EventArgs e)
        {
            string settingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
            if (Directory.Exists(settingsDirectory))
            {
                System.Diagnostics.Process.Start("explorer.exe", settingsDirectory);
            }
            else
            {
                MessageBox.Show("The settings folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSetings_Click(object sender, EventArgs e)
        {
            if (ComputerInfo != null)
            {
                LoadUserControl(new UserControl1(ComputerInfo));
            }
            else
            {
                MessageBox.Show("Hardware information is not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}