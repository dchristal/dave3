#nullable enable
using System.ComponentModel.DataAnnotations.Schema;

namespace dave3.Model;

public class Inventory
{
    public Inventory() { } // Parameterless constructor
    
    public Inventory(string productName, string locationName, string categoryName)
    {
        ProductName = productName;
        LocationName = locationName;
        CategoryName = categoryName;
    }


    public int ProductId { get; set; }


    public int Location { get; set; }

    public int CategoryId { get; set; }

    public DateTime LastUpdate { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string? Material { get; set; }
    public float? Weight { get; set; }
    public string? UoM { get; set; }
    public float? Length { get; set; }
    public float? Width { get; set; }
    public float? Height { get; set; }
    public float? Diameter { get; set; }
    public float? Pitch { get; set; }
    public float? Volts { get; set; }
    public float? Amps { get; set; }
    public float? Watts { get; set; }


    public string? Notes { get; set; }

    public int InventoryId { get; set; }


    [NotMapped] public string? ProductName { get; set; }

    [NotMapped] public string? LocationName { get; set; }

    [NotMapped] public string? CategoryName { get; set; }
}