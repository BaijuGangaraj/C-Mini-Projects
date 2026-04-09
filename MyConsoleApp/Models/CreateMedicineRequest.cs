using System.ComponentModel.DataAnnotations;

namespace MyConsoleApp.Models;

public class CreateMedicineRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public DateOnly ExpiryDate { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal Price { get; set; }

    [Required]
    public string Brand { get; set; } = string.Empty;
}
