// Services/InventoryRepository.cs (rename from InventtoryRepository.cs if needed; full file)

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dave3.Model; // Namespace for Inventory, TreeNodeEntity, and DelightfulContext

namespace dave3.Services
{
    public class InventoryRepository
    {
        private readonly DelightfulContext _context;

        public InventoryRepository(DelightfulContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Inventory>> GetFilteredInventoryAsync(int? productId, int? locationId, int? categoryId, bool includeChildren)
        {
            var query = _context.Inventories.AsQueryable();

            if (productId.HasValue)
            {
                var ids = includeChildren ? await GetDescendantIdsAsync(productId.Value, 1) : new List<int> { productId.Value };
                query = query.Where(i => ids.Contains(i.ProductId));
            }

            if (locationId.HasValue)
            {
                var ids = includeChildren ? await GetDescendantIdsAsync(locationId.Value, 2) : new List<int> { locationId.Value };
                query = query.Where(i => ids.Contains(i.LocationId));
            }

            if (categoryId.HasValue)
            {
                var ids = includeChildren ? await GetDescendantIdsAsync(categoryId.Value, 3) : new List<int> { categoryId.Value };
                query = query.Where(i => ids.Contains(i.CategoryId));
            }

            return await query.ToListAsync();
        }

        public async Task<List<Inventory>> GetAllAsync()
        {
            return await _context.Inventories.ToListAsync();
        }

        public async Task AddAsync(Inventory item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _context.Inventories.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Inventory item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_context.Entry(item).State == EntityState.Detached)
            {
                _context.Inventories.Attach(item);
                _context.Entry(item).State = EntityState.Modified;
            }
            else
            {
                _context.Inventories.Update(item);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.Inventories.FindAsync(id);
            if (item != null)
            {
                _context.Inventories.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<int> ids)
        {
            var items = await _context.Inventories.Where(i => ids.Contains(i.InventoryId)).ToListAsync();
            if (items.Any())
            {
                _context.Inventories.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private async Task<List<int>> GetDescendantIdsAsync(int parentId, int treeId)
        {
            var descendants = new List<int> { parentId };

            var children = await _context.TreeNodeEntities
            .Where(n => n.ParentId == parentId && n.TreeId == treeId)
            .Select(n => n.Id)
            .ToListAsync();

            foreach (var childId in children)
            {
                var childDescendants = await GetDescendantIdsAsync(childId, treeId);
                descendants.AddRange(childDescendants);
            }

            return descendants;
        }
    }
}