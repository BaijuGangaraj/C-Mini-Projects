# ABC Pharmacy SPA (.NET Core + JavaScript)

This project is a single-page application for medicine tracking and sale recording.

## Features
- View medicine list in a grid (without Notes field)
- Add new medicine details
- Search medicine by name
- Record sales and auto-reduce stock
- Color indicators:
  - Red when expiry date is less than 30 days
  - Yellow when quantity is less than 10
- JSON file storage on server side (`Data/medicines.json`, `Data/sales.json`)

## Launch Steps
1. Create project:
   ```bash
   dotnet new web -n MyConsoleApp
   ```
2. Build and run:
   ```bash
   dotnet run --project MyConsoleApp
   ```
3. Open preview/browser at URL shown in terminal.
