using System;
using System.Security.Cryptography;
using System.Text;

namespace LicenseGenerator;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Scholastica Reader License Generator ===");
        Console.Write("\nEnter Hardware ID: ");
        var hwid = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(hwid))
        {
            Console.WriteLine("Invalid HWID");
            return;
        }

        var secret = "ScholasticaSecretKey2026!";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(hwid));
        var hashString = Convert.ToHexString(hash).Substring(0, 12);
        var code = $"SCHOL-{hashString.Substring(0, 4)}-{hashString.Substring(4, 4)}-{hashString.Substring(8, 4)}";
        
        Console.WriteLine($"\nActivation Code: {code}");
        Console.WriteLine("\nGive this code to the user. It will work only once per HWID.");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
