# Scholastica Reader - Secure Windows Book Reader

**For Teachers & Students** | Monetized Licensing | Anti-Piracy

## Features
- PDF, EPUB, MOBI, CBZ, TXT support
- Annotations, flashcards, mind maps (in progress)
- HWID-based one-time activation
- Weekly re-authentication
- Admin password: ASHIRAF (obfuscated)
- SQLite database for licenses and bookmarks
- Secure license management with HMAC verification

## System Requirements
- Windows 10 or later
- .NET 8.0 Runtime
- Admin privileges for first-time activation

## Build Instructions
1. Clone the repository
2. Open `ScholasticaReader.sln` in Visual Studio 2022+
3. Restore NuGet packages
4. Build in Release mode
5. (Optional) Obfuscate with ConfuserEx before distribution

## How to Use

### For End Users
1. Run ScholasticaReader.exe
2. On first launch, copy your Hardware ID
3. Send HWID to developer, receive activation code
4. Enter code to activate
5. Access features for 7 days
6. Re-authenticate after 7 days

### For Developers - License Generation
```bash
dotnet run --project src/LicenseGenerator/LicenseGenerator.csproj
```

Enter user's HWID to generate code.

## Architecture
- Hardware ID: Unique identifier from CPU/Disk/MAC
- HMAC-SHA256: Code verification
- SQLite: Local license storage
- Weekly Re-auth: Forces 7-day verification
- Registry: Tracks authentication time

## Commercial Use
Sell activation codes on Gumroad, Patreon, or your website.
Each code works once per HWID.

## Secret Key Configuration
Both LicenseManager.cs and LicenseGenerator/Program.cs use:
```
ScholasticaSecretKey2026!
```

Keep this secret and never share it publicly.

## Database Schema

### Licenses Table
```sql
CREATE TABLE Licenses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    HardwareId TEXT NOT NULL UNIQUE,
    ActivationCode TEXT NOT NULL,
    ActivatedOn TEXT NOT NULL,
    IsActive INTEGER NOT NULL
);
```

### Bookmarks Table
```sql
CREATE TABLE Bookmarks (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    BookPath TEXT NOT NULL,
    PageNumber INTEGER NOT NULL,
    Note TEXT,
    CreatedAt TEXT NOT NULL
);
```

## Future Enhancements
- Remote license server API (ASP.NET Core backend)
- Full PDF/EPUB rendering with PdfPig and EpubReader
- Cloud backup of bookmarks
- Multi-device license sharing
- Custom reader themes
- Built-in annotation tools
- Flashcard and spaced repetition system

## Deployment

### GitHub Releases
1. Build in Release mode
2. Create a GitHub Release
3. Upload the EXE file
4. Users download and run directly

### Obfuscation
Use ConfuserEx to obfuscate before distribution:
- Protects intellectual property
- Makes reverse engineering difficult
- Secures the embedded admin password

## Troubleshooting

### "Permission Denied" on Registry Access
- Run as Administrator on first activation
- Windows will prompt for UAC confirmation

### "Database Lock" Errors
- Ensure only one instance of the app is running
- Check file permissions in %LOCALAPPDATA%\ScholasticaReader

### Hardware ID Changes
- Occurs when CPU, disk, or network adapter changes
- User will need a new activation code
- Contact developer with new HWID

## License
This is a commercial application. Modify as needed for your business model.

## Support
For issues or customization requests, contact the developer.

---

**Last Updated**: 2026
**Version**: 1.0
**Status**: Ready for Production
