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
        try
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
        catch
        {
            return true; // Default to requiring re-auth on error
        }
    }

    public static void UpdateLastReAuth()
    {
        try
        {
            using var key = Registry.LocalMachine.CreateSubKey(RegKeyPath);
            key.SetValue(LastCheckValue, DateTime.UtcNow.ToString("o"));
        }
        catch
        {
            // Silently fail if registry write fails
        }
    }

    /// <summary>
    /// Encrypts plaintext using AES encryption with random IV.
    /// Returns IV + ciphertext concatenated as Base64.
    /// </summary>
    public static string Encrypt(string plainText, string key = "Schol@ticaKey2024!")
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        aes.GenerateIV(); // FIXED: Generate random IV instead of using all zeros
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        // Write IV first
        ms.Write(aes.IV, 0, aes.IV.Length);
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            using (var writer = new StreamWriter(cs))
            {
                writer.Write(plainText);
            }
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// Decrypts ciphertext encrypted with the Encrypt method.
    /// Expects IV + ciphertext concatenated as Base64.
    /// </summary>
    public static string Decrypt(string cipherText, string key = "Schol@ticaKey2024!")
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException(nameof(cipherText));

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var buffer = Convert.FromBase64String(cipherText);
        // Extract IV (first 16 bytes)
        aes.IV = new byte[16];
        Array.Copy(buffer, 0, aes.IV, 0, 16);

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer, 16, buffer.Length - 16);
        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
        {
            using (var reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
