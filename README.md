# portalFacturas

Portal de facturas, administrado por contribuyente. Consulta y administración de facturas electrónicas (CFDI).

## Structure
- `Portal` — web portal
- `Business` — business logic
- `Data_Access` — data access layer
- `Entidades` — entities
- `DXML` — CFDI XML handling
- `Setup_PFX` / `ToPfx` — digital certificate (PFX) handling
- `test` — tests

## Stack
C# · .NET · CFDI (SAT)

## How to run
1. Open `launchCFDI.sln` in Visual Studio.
2. Restore NuGet packages.
3. Run the Portal project.
