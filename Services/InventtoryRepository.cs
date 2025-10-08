using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dave3.Model;  // Your namespace

public class InventoryRepository
{
    private readonly DelightfulContext _context;

    public InventoryRepository(DelightfulContext context)
    {
        _context = context;
    }

    public async Task<List<Inventory>> GetFilteredInventoryAsync(int? productId, int? locationId, int? categoryId, bool includeChildren)
    {
        var query = _context.Inventory.AsQueryable();
        if (productId.HasValue) query = query.Where(i => i.ProductId == productId.Value);  // Add child logic if needed
        // Similar for location/category
        return await query.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}