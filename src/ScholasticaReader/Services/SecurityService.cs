using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;

namespace ScholasticaReader.Services;

public class SecurityService
{
    private const string AdminPasswordHash = "ASHIRAF";
    private const string RegKeyPath = @"SOFTWARE\ScholasticaReader";
    private const string LastCheckValue = "LastServerCheck";

    public static bool VerifyAdminPassword(string input)
    {
        // Hardcoded ASHIRAF – will be obfuscated by ConfuserEx
        return input == "ASHIRAF";
    }

    public static bool IsWeeklyReAuthRequired()
    {
        using var key = Registry.LocalMachine.OpenSubKey(RegKeyPath);
        if (key == null) return true;
        var lastCheck = key.GetValue(LastCheckValue) as string;
        if (string.IsNullOrEmpty(lastCheck)) return true;
        if (DateTime.TryParse(lastCheck, out var lastDate))
        {
            return (DateTime.UtcNow - lastDate).TotalDays >= 7;
        }
        return true;
    }

    public static void UpdateLastReAuth()
    {
        using var key = Registry.LocalMachine.CreateSubKey(RegKeyPath);
        key.SetValue(LastCheckValue, DateTime.UtcNow.ToString("o"));
    }

    // Simple AES encryption for license storage
    public static string Encrypt(string plainText, string key = "Schol@ticaKey2024!")
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        aes.IV = new byte[16];
        var encryptor = aes.CreateEncryptor();
        var plain = Encoding.UTF8.GetBytes(plainText);
        var cipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);
        return Convert.ToBase64String(cipher);
    }

    public static string Decrypt(string cipherText, string key = "Schol@ticaKey2024!")
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        aes.IV = new byte[16];
        var decryptor = aes.CreateDecryptor();
        var cipher = Convert.FromBase64String(cipherText);
        var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        return Encoding.UTF8.GetString(plain);
    }
}
