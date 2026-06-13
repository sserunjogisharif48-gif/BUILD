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
