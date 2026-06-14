using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ScholasticaReader.Helpers;

namespace ScholasticaReader.Services;

public static class LicenseManager
{
    private static readonly HttpClient client = new HttpClient();
    // Replace with your actual server URL (free tier: render.com, fly.io)
    private const string ServerValidationEndpoint = "https://your-license-server.com/validate";

    public static async Task<bool> ActivateWithCode(string userProvidedCode)
    {
        var hwid = HardwareID.GetUniqueHardwareId();
        if (DatabaseService.IsHardwareIdAlreadyActivated(hwid))
        {
            // Already activated: check weekly
            if (SecurityService.IsWeeklyReAuthRequired())
            {
                var valid = await ValidateWithServer(hwid, userProvidedCode);
                if (valid) SecurityService.UpdateLastReAuth();
                return valid;
            }
            return true; // Still within week
        }

        // First-time activation: code must match server-generated one for this HWID
        var isValid = await ValidateCodeWithServer(hwid, userProvidedCode);
        if (isValid)
        {
            DatabaseService.StoreActivation(hwid, userProvidedCode);
            SecurityService.UpdateLastReAuth();
        }
        return isValid;
    }

    private static Task<bool> ValidateCodeWithServer(string hwid, string code)
    {
        // FIXED: Removed unnecessary async - this is synchronous validation
        // In production: send to your own license server.
        // For demo: we simulate using a built-in check (but you MUST run the generator tool).
        // The generator creates codes like "SCHOL-XXXX-YYYY-ZZZZ" based on HMAC.
        // We'll replicate the same HMAC verification here.
        return Task.FromResult(VerifyCodeForHwid(hwid, code));
    }

    private static Task<bool> ValidateWithServer(string hwid, string code)
    {
        // FIXED: Removed unnecessary async - this is synchronous validation
        // Weekly re-auth – same HMAC check
        return Task.FromResult(VerifyCodeForHwid(hwid, code));
    }

    // Local HMAC verification (same as generator)
    private static bool VerifyCodeForHwid(string hwid, string code)
    {
        // Secret key (only known to you and your generator)
        var secret = "ScholasticaSecretKey2026!";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var expectedHash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(hwid))).Substring(0, 12);
        var expectedCode = $"SCHOL-{expectedHash.Substring(0, 4)}-{expectedHash.Substring(4, 4)}-{expectedHash.Substring(8, 4)}";
        return string.Equals(code, expectedCode, StringComparison.OrdinalIgnoreCase);
    }
}
