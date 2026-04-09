using System.ComponentModel.DataAnnotations;
using MyConsoleApp.Models;
using MyConsoleApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PharmacyStore>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/medicines", async (string? search, PharmacyStore store) =>
{
    var medicines = await store.GetMedicinesAsync(search);
    return Results.Ok(medicines);
});

app.MapPost("/api/medicines", async (CreateMedicineRequest request, PharmacyStore store) =>
{
    var validation = Validate(request);
    if (validation.Count > 0)
    {
        return Results.ValidationProblem(validation);
    }

    if (request.ExpiryDate <= DateOnly.FromDateTime(DateTime.UtcNow.Date))
    {
        return Results.BadRequest(new { message = "Expiry date must be in the future." });
    }

    var medicine = await store.AddMedicineAsync(request);
    return Results.Created($"/api/medicines/{medicine.Id}", medicine);
});

app.MapGet("/api/sales", async (PharmacyStore store) =>
{
    var sales = await store.GetSalesAsync();
    return Results.Ok(sales);
});

app.MapPost("/api/sales", async (CreateSaleRequest request, PharmacyStore store) =>
{
    var validation = Validate(request);
    if (validation.Count > 0)
    {
        return Results.ValidationProblem(validation);
    }

    var result = await store.AddSaleAsync(request);
    return result.Success
        ? Results.Created($"/api/sales/{result.Sale!.Id}", result.Sale)
        : Results.BadRequest(new { message = result.Error });
});

app.Run();

static Dictionary<string, string[]> Validate<T>(T model)
{
    var validationResults = new List<ValidationResult>();
    var ctx = new ValidationContext(model!);
    Validator.TryValidateObject(model!, ctx, validationResults, true);

    return validationResults
        .GroupBy(v => v.MemberNames.FirstOrDefault() ?? string.Empty)
        .ToDictionary(
            g => g.Key,
            g => g.Select(v => v.ErrorMessage ?? "Invalid value.").ToArray());
}
