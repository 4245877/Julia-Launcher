using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Julia_Launcher
{
    public class GpuInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Нет данных";

        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; } = "Нет данных";

        [JsonPropertyName("memory_bytes")]
        public ulong MemoryBytes { get; set; } // В байтах

        [JsonPropertyName("resolution")]
        public string Resolution { get; set; } = "Нет данных";

        [JsonPropertyName("refresh_rate")]
        public string RefreshRate { get; set; } = "Нет данных";

        [JsonPropertyName("driver_version")]
        public string DriverVersion { get; set; } = "Нет данных";

        [JsonPropertyName("video_processor")]
        public string VideoProcessor { get; set; } = "Нет данных";
    }

    public class RamModuleInfo
    {
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; } = "Нет данных";

        [JsonPropertyName("capacity_bytes")]
        public ulong CapacityBytes { get; set; } // В байтах

        [JsonPropertyName("speed_mhz")]
        public string SpeedMHz { get; set; } = "Нет данных";

        [JsonPropertyName("form_factor")]
        public string FormFactor { get; set; } = "Нет данных";

        [JsonPropertyName("memory_type_description")] // Изменено для ясности
        public string MemoryTypeDescription { get; set; } = "Нет данных";
    }

    public class HardwareInfo
    {
        [JsonPropertyName("cpu_name")]
        public string CpuName { get; set; } = "Нет данных";
        [JsonPropertyName("cpu_manufacturer")]
        public string CpuManufacturer { get; set; } = "Нет данных";
        [JsonPropertyName("cpu_cores")]
        public uint CpuCores { get; set; }
        [JsonPropertyName("cpu_logical_processors")]
        public uint CpuLogicalProcessors { get; set; }
        [JsonPropertyName("cpu_max_clock_speed_mhz")]
        public uint CpuMaxClockSpeedMHz { get; set; }
        [JsonPropertyName("cpu_address_width_bits")] // Изменено для ясности
        public string CpuAddressWidthBits { get; set; } = "Нет данных";

        [JsonPropertyName("ram_total_visible_bytes")] // Изменено для ясности
        public ulong RamTotalVisibleBytes { get; set; }
        [JsonPropertyName("ram_modules")]
        public List<RamModuleInfo> RamModules { get; set; } = new List<RamModuleInfo>();

        [JsonPropertyName("gpus")]
        public List<GpuInfo> Gpus { get; set; } = new List<GpuInfo>();

        private static string SafeGetWmiProperty(ManagementBaseObject obj, string propertyName, string defaultValue = "Нет данных")
        {
            try
            {
                object value = obj[propertyName];
                return value != null && value != DBNull.Value ? value.ToString()?.Trim() : defaultValue;
            }
            catch (ManagementException mex) { /* TODO: Логирование! ($"WMI ошибка доступа к свойству {propertyName}: {mex.Message}") */ return defaultValue; }
            catch (Exception ex) { /* TODO: Логирование! ($"Ошибка получения свойства {propertyName}: {ex.Message}") */ return defaultValue; }
        }

        private static T SafeGetWmiProperty<T>(ManagementBaseObject obj, string propertyName, T defaultValue = default) where T : struct
        {
            try
            {
                object value = obj[propertyName];
                if (value != null && value != DBNull.Value)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                return defaultValue;
            }
            catch (ManagementException mex) { /* TODO: Логирование! ($"WMI ошибка доступа к свойству {propertyName} как {typeof(T).Name}: {mex.Message}") */ return defaultValue; }
            catch (Exception ex) { /* TODO: Логирование! ($"Ошибка получения свойства {propertyName} как {typeof(T).Name}: {ex.Message}") */ return defaultValue; }
        }

        public async Task CollectAllDataAsync()
        {
            // Запускаем все WMI запросы в фоновом потоке, чтобы не блокировать UI
            await Task.Run(() =>
            {
                CollectProcessorInfoInternal();
                CollectMemoryInfoInternal();
                CollectVideoInfoInternal();
            }).ConfigureAwait(false); // Возвращаемся в любой поток
        }

        private void CollectProcessorInfoInternal()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name, Manufacturer, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed, AddressWidth FROM Win32_Processor"))
                using (var collection = searcher.Get())
                {
                    foreach (ManagementObject obj in collection) // Обычно один объект
                    {
                        using (obj)
                        {
                            CpuName = SafeGetWmiProperty(obj, "Name");
                            CpuManufacturer = SafeGetWmiProperty(obj, "Manufacturer");
                            CpuCores = SafeGetWmiProperty<uint>(obj, "NumberOfCores");
                            CpuLogicalProcessors = SafeGetWmiProperty<uint>(obj, "NumberOfLogicalProcessors");
                            CpuMaxClockSpeedMHz = SafeGetWmiProperty<uint>(obj, "MaxClockSpeed");
                            CpuAddressWidthBits = SafeGetWmiProperty(obj, "AddressWidth");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { /* TODO: Логирование! ($"Полная ошибка сбора информации о процессоре: {ex.ToString()}") */ CpuName = "Ошибка сбора данных CPU"; }
        }

        private void CollectMemoryInfoInternal()
        {
            RamModules.Clear();
            RamTotalVisibleBytes = 0;
            try
            {
                using (var osSearcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem"))
                using (var osCollection = osSearcher.Get())
                {
                    foreach (ManagementObject obj in osCollection)
                    {
                        using (obj) { RamTotalVisibleBytes = SafeGetWmiProperty<ulong>(obj, "TotalVisibleMemorySize") * 1024; break; } // KB to Bytes
                    }
                }

                using (var memSearcher = new ManagementObjectSearcher("SELECT Manufacturer, Capacity, Speed, FormFactor, MemoryType FROM Win32_PhysicalMemory"))
                using (var memCollection = memSearcher.Get())
                {
                    foreach (ManagementObject obj in memCollection)
                    {
                        using (obj)
                        {
                            RamModules.Add(new RamModuleInfo
                            {
                                Manufacturer = SafeGetWmiProperty(obj, "Manufacturer"),
                                CapacityBytes = SafeGetWmiProperty<ulong>(obj, "Capacity"),
                                SpeedMHz = SafeGetWmiProperty(obj, "Speed"), // Может быть null или 0
                                FormFactor = GetRamFormFactorDescription(SafeGetWmiProperty<ushort>(obj, "FormFactor")),
                                MemoryTypeDescription = GetRamMemoryTypeDescription(SafeGetWmiProperty<ushort>(obj, "MemoryType"))
                            });
                        }
                    }
                }
                // Если RamTotalVisibleBytes не удалось получить, можно просуммировать модули (менее точно)
                if (RamTotalVisibleBytes == 0 && RamModules.Count > 0)
                {
                    foreach (var module in RamModules) RamTotalVisibleBytes += module.CapacityBytes;
                }
            }
            catch (Exception ex) { /* TODO: Логирование! ($"Полная ошибка сбора информации о памяти: {ex.ToString()}") */ }
        }

        private void CollectVideoInfoInternal()
        {
            Gpus.Clear();
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name, AdapterCompatibility, AdapterRAM, CurrentHorizontalResolution, CurrentVerticalResolution, CurrentRefreshRate, DriverVersion, VideoProcessor FROM Win32_VideoController"))
                using (var collection = searcher.Get())
                {
                    foreach (ManagementObject obj in collection)
                    {
                        using (obj)
                        {
                            Gpus.Add(new GpuInfo
                            {
                                Name = SafeGetWmiProperty(obj, "Name"),
                                Manufacturer = SafeGetWmiProperty(obj, "AdapterCompatibility"),
                                MemoryBytes = SafeGetWmiProperty<uint>(obj, "AdapterRAM"), // WMI AdapterRAM часто неточен для >2GB. uint может быть мал.
                                Resolution = $"{SafeGetWmiProperty(obj, "CurrentHorizontalResolution")}x{SafeGetWmiProperty(obj, "CurrentVerticalResolution")}",
                                RefreshRate = SafeGetWmiProperty(obj, "CurrentRefreshRate"),
                                DriverVersion = SafeGetWmiProperty(obj, "DriverVersion"),
                                VideoProcessor = SafeGetWmiProperty(obj, "VideoProcessor")
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { /* TODO: Логирование! ($"Полная ошибка сбора информации о видеокартах: {ex.ToString()}") */ }
        }

        public async Task SaveToFileAsync(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                string json = JsonSerializer.Serialize(this, options);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex) { /* TODO: Логирование! ($"Ошибка сохранения HardwareInfo в '{filePath}': {ex.ToString()}") */ }
        }

        public static HardwareInfo LoadFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) { /* TODO: Логирование ($"Файл HardwareInfo '{filePath}' не найден.") */ return null; }
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<HardwareInfo>(json);
            }
            catch (Exception ex) { /* TODO: Логирование! ($"Ошибка загрузки HardwareInfo из '{filePath}': {ex.ToString()}") */ return null; }
        }

        // Эти методы лучше бы тоже сделать более надежными или использовать enum с Description атрибутами
        private static string GetRamFormFactorDescription(ushort code) { /* ... твой код ... */ return $"FormFactor Code {code}"; }
        private static string GetRamMemoryTypeDescription(ushort code) { /* ... твой код ... */  return $"MemoryType Code {code}"; }
    }
}