using System.Text.Json;
using MyConsoleApp.Models;

namespace MyConsoleApp.Services;

public class PharmacyStore
{
    private readonly string _medicinesPath;
    private readonly string _salesPath;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public PharmacyStore(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDir);

        _medicinesPath = Path.Combine(dataDir, "medicines.json");
        _salesPath = Path.Combine(dataDir, "sales.json");

        EnsureSeedData();
    }

    public async Task<IReadOnlyList<Medicine>> GetMedicinesAsync(string? search)
    {
        var medicines = await ReadJsonAsync<List<Medicine>>(_medicinesPath) ?? [];

        if (!string.IsNullOrWhiteSpace(search))
        {
            medicines = medicines
                .Where(m => m.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return medicines
            .OrderBy(m => m.FullName)
            .ToList();
    }

    public async Task<Medicine> AddMedicineAsync(CreateMedicineRequest request)
    {
        var medicine = new Medicine
        {
            FullName = request.FullName.Trim(),
            Notes = request.Notes.Trim(),
            ExpiryDate = request.ExpiryDate,
            Quantity = request.Quantity,
            Price = decimal.Round(request.Price, 2),
            Brand = request.Brand.Trim()
        };

        await _sync.WaitAsync();
        try
        {
            var medicines = await ReadJsonAsync<List<Medicine>>(_medicinesPath) ?? [];
            medicines.Add(medicine);
            await WriteJsonAsync(_medicinesPath, medicines);
            return medicine;
        }
        finally
        {
            _sync.Release();
        }
    }

    public async Task<IReadOnlyList<SaleRecord>> GetSalesAsync()
    {
        var sales = await ReadJsonAsync<List<SaleRecord>>(_salesPath) ?? [];
        return sales.OrderByDescending(s => s.SoldAtUtc).ToList();
    }

    public async Task<(bool Success, string? Error, SaleRecord? Sale)> AddSaleAsync(CreateSaleRequest request)
    {
        await _sync.WaitAsync();
        try
        {
            var medicines = await ReadJsonAsync<List<Medicine>>(_medicinesPath) ?? [];
            var medicine = medicines.FirstOrDefault(m => m.Id == request.MedicineId);

            if (medicine is null)
            {
                return (false, "Medicine not found.", null);
            }

            if (request.QuantitySold > medicine.Quantity)
            {
                return (false, "Sale quantity exceeds stock quantity.", null);
            }

            medicine.Quantity -= request.QuantitySold;

            var sales = await ReadJsonAsync<List<SaleRecord>>(_salesPath) ?? [];
            var sale = new SaleRecord
            {
                MedicineId = medicine.Id,
                MedicineName = medicine.FullName,
                QuantitySold = request.QuantitySold,
                UnitPrice = medicine.Price
            };

            sales.Add(sale);

            await WriteJsonAsync(_medicinesPath, medicines);
            await WriteJsonAsync(_salesPath, sales);

            return (true, null, sale);
        }
        finally
        {
            _sync.Release();
        }
    }

    private void EnsureSeedData()
    {
        if (!File.Exists(_medicinesPath))
        {
            var seed = new List<Medicine>
            {
                new()
                {
                    FullName = "Paracetamol 500mg Tablet",
                    Notes = "Pain and fever relief",
                    ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(18)),
                    Quantity = 8,
                    Price = 5.99m,
                    Brand = "ABC Care"
                },
                new()
                {
                    FullName = "Amoxicillin 250mg Capsule",
                    Notes = "Prescription antibiotic",
                    ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(150)),
                    Quantity = 45,
                    Price = 12.49m,
                    Brand = "MediLife"
                }
            };

            File.WriteAllText(_medicinesPath, JsonSerializer.Serialize(seed, _jsonOptions));
        }

        if (!File.Exists(_salesPath))
        {
            File.WriteAllText(_salesPath, "[]");
        }
    }

    private async Task<T?> ReadJsonAsync<T>(string path)
    {
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);
    }

    private async Task WriteJsonAsync<T>(string path, T value)
    {
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, value, _jsonOptions);
    }
}
