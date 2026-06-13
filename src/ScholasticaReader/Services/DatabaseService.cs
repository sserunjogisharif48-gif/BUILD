using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace ScholasticaReader.Services;

public static class DatabaseService
{
    private static string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScholasticaReader", "app.db");

    public static void Initialize()
    {
        var dir = Path.GetDirectoryName(DbPath);
        
        if (string.IsNullOrEmpty(dir))
            throw new InvalidOperationException("Failed to determine application data directory.");
            
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        using var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Licenses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                HardwareId TEXT NOT NULL UNIQUE,
                ActivationCode TEXT NOT NULL,
                ActivatedOn TEXT NOT NULL,
                IsActive INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Bookmarks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BookPath TEXT NOT NULL,
                PageNumber INTEGER NOT NULL,
                Note TEXT,
                CreatedAt TEXT NOT NULL
            );
        ";
        cmd.ExecuteNonQuery();
    }

    public static bool IsHardwareIdAlreadyActivated(string hwid)
    {
        if (string.IsNullOrEmpty(hwid))
            return false;
            
        using var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Licenses WHERE HardwareId = $hwid AND IsActive = 1";
        cmd.Parameters.AddWithValue("$hwid", hwid);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public static void StoreActivation(string hwid, string activationCode)
    {
        if (string.IsNullOrEmpty(hwid) || string.IsNullOrEmpty(activationCode))
            throw new ArgumentException("HWID and activation code cannot be empty.");
            
        using var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT OR REPLACE INTO Licenses (HardwareId, ActivationCode, ActivatedOn, IsActive) VALUES ($hwid, $code, $date, 1)";
        cmd.Parameters.AddWithValue("$hwid", hwid);
        cmd.Parameters.AddWithValue("$code", activationCode);
        cmd.Parameters.AddWithValue("$date", DateTime.UtcNow.ToString("o"));
        cmd.ExecuteNonQuery();
    }

    public static bool ValidateStoredLicense(string hwid, string enteredCode)
    {
        if (string.IsNullOrEmpty(hwid) || string.IsNullOrEmpty(enteredCode))
            return false;
            
        using var conn = new SqliteConnection($"Data Source={DbPath}");
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ActivationCode FROM Licenses WHERE HardwareId = $hwid AND IsActive = 1";
        cmd.Parameters.AddWithValue("$hwid", hwid);
        var stored = cmd.ExecuteScalar() as string;
        return stored != null && stored == enteredCode;
    }
}
