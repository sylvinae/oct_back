using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Models;
using API.Utils;
using Microsoft.EntityFrameworkCore;
using Sprache;

namespace API.Services
{
    public class ItemService(ApiDbContext db, ILogger<ItemService> log)
    {
        private readonly ApiDbContext _db = db;
        private readonly ILogger<ItemService> _log = log;

        public async Task<(List<object>, List<object>)> CreateItems(List<ItemModel> items)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var createdItems = new List<object>();
                var failedItems = new List<object>();

                foreach (var item in items)
                {
                    try
                    {
                        var hash = Cryptics.ComputeHash(item);
                        var existingItem = await _db.Items.FirstOrDefaultAsync(i => i.Hash == hash);
                        if (existingItem != null)
                        {
                            _log.LogWarning(
                                "Item already exists: {Brand} - {Generic}.",
                                item.Brand,
                                item.Generic
                            );
                            failedItems.Add(item);
                            continue;
                        }

                        item.Hash = hash;
                        var result = await _db.Items.AddAsync(item);
                        createdItems.Add(result.Entity);
                        _log.LogInformation("Created item with ID {ItemId}.", result.Entity.Id);
                    }
                    catch (Exception ex)
                    {
                        failedItems.Add(item);
                        _log.LogError(
                            ex,
                            "Failed to create item {Brand} - {Generic}.",
                            item.Brand,
                            item.Generic
                        );
                    }
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _log.LogInformation("Successfully saved changes to DB.");
                return (createdItems, failedItems);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred. Rolling back changes.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public IQueryable<ItemModel> GetAllItems(int page, int limit)
        {
            try
            {
                return _db.Items.Where(i => !i.IsDeleted).Skip((page - 1) * limit).Take(limit);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred while fetching items.");
                throw;
            }
        }

        public async Task<ItemModel?> GetItem(Guid id)
        {
            try
            {
                return await _db.Items.FirstOrDefaultAsync(i => i.IsDeleted == false && i.Id == id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred while fetching item with ID {ItemId}.", id);
                throw;
            }
        }

        public async Task<(List<object>, List<object>)> UpdateItems(List<ItemModel> items)
        {
            _log.LogDebug("Starting update for {Count} item(s).", items.Count);
            var updatedItems = new List<object>();
            var failedItems = new List<object>();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in items)
                {
                    var existingItem = await _db.Items.FindAsync(item.Id);
                    if (existingItem != null)
                    {
                        _log.LogInformation("Updating item with ID {ItemId}.", existingItem.Id);
                        _db.Entry(existingItem).CurrentValues.SetValues(item);
                        updatedItems.Add(existingItem);
                    }
                    else
                    {
                        _log.LogWarning("Item with ID {ItemId} not found.", item.Id);
                        failedItems.Add(item);
                    }
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _log.LogInformation("Successfully saved changes to DB.");
                return (updatedItems, failedItems);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred. Rolling back changes.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<(List<Guid>, List<Guid>)> DeleteItems(List<Guid> itemIds)
        {
            _log.LogDebug("Starting deletion of {Count} item(s).", itemIds.Count);
            var failedItems = new List<Guid>();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var id in itemIds)
                {
                    var item = await _db.Items.FindAsync(id);
                    if (item != null)
                    {
                        item.IsDeleted = true;
                        _log.LogInformation("Soft-deleted item with ID {ItemId}.", id);
                    }
                    else
                    {
                        _log.LogWarning("Item with ID {ItemId} not found.", id);
                        failedItems.Add(id);
                    }
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                var deletedItems = itemIds.Except(failedItems).ToList();
                _log.LogInformation("Successfully saved changes to DB.");
                return (deletedItems, failedItems);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred. Rolling back changes.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public IQueryable<ItemModel> SearchItems(string query, int page, int limit)
        {
            try
            {
                var items = _db
                    .Items.Where(i => (i.Brand ?? "").ToLower().Contains(query) && !i.IsDeleted)
                    .OrderBy(i => i.Id)
                    .Skip((page - 1) * limit)
                    .Take(limit);

                return items;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred while searching items.");
                throw;
            }
        }
    }
}
