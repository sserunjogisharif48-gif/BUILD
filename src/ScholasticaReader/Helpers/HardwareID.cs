using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace ScholasticaReader.Helpers;

public static class HardwareID
{
    public static string GetUniqueHardwareId()
    {
        var cpuId = GetCpuId();
        var driveSerial = GetDriveSerial();
        var mac = GetMacAddress();
        var combined = $"{cpuId}-{driveSerial}-{mac}";
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
        return Convert.ToHexString(hash).Substring(0, 16);
    }

    private static string GetCpuId()
    {
        using var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
        foreach (var obj in searcher.Get())
            return obj["ProcessorId"]?.ToString() ?? "CPU_UNKNOWN";
        return "CPU_UNKNOWN";
    }

    private static string GetDriveSerial()
    {
        using var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE Index=0");
        foreach (var obj in searcher.Get())
            return obj["SerialNumber"]?.ToString()?.Trim() ?? "DRIVE_UNKNOWN";
        return "DRIVE_UNKNOWN";
    }

    private static string GetMacAddress()
    {
        using var searcher = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled=True");
        foreach (var obj in searcher.Get())
            return obj["MACAddress"]?.ToString()?.Replace(":", "") ?? "MAC_UNKNOWN";
        return "MAC_UNKNOWN";
    }
}
