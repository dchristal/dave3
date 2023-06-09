#nullable enable
namespace dave3.Model;

public partial class InventoryView
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int Location { get; set; }

    public DateTime LastUpdate { get; set; }

    public string? Notes { get; set; }

    public string ProductName { get; set; } = null!;

    public string LocationName { get; set; } = null!;
}
