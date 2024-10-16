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

        public async Task<(List<ItemResponseDto>, List<ItemDto>)> CreateItem(List<ItemDto> items)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var createdItems = new List<ItemResponseDto>();
                var failedItems = new List<ItemDto>();

                _log.LogDebug("Starting item creation for {Count} item(s).", items.Count);
                foreach (var item in items)
                {
                    try
                    {
                        var existingItem = await _db.Items.FirstOrDefaultAsync(i =>
                            i.Hash == Cryptics.ComputeHash(item)
                        );
                        if (existingItem != null)
                        {
                            throw new Exception("Item already exists.");
                        }

                        var newItem = PropCopier.Copy(
                            item,
                            new ItemModel { Hash = Cryptics.ComputeHash(item) }
                        );
                        await _db.Items.AddAsync(newItem);
                        createdItems.Add(PropCopier.Copy(newItem, new ItemResponseDto()));
                        _log.LogInformation("Created item with ID {ItemId}.", newItem.Id);
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

        public async Task<(List<ItemResponseDto>?, int)> GetAllItems(int page, int limit)
        {
            _log.LogDebug("Fetching items: Page {Page}, Limit {Limit}.", page, limit);
            try
            {
                var items = await _db
                    .Items.Where(i => !i.IsDeleted)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                var response = items
                    .Select(i => PropCopier.Copy(i, new ItemResponseDto()))
                    .ToList();

                _log.LogInformation("Fetched {Count} items.", items.Count);
                return (response, items.Count);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred while fetching items.");
                throw;
            }
        }

        public async Task<ItemResponseDto?> GetItem(Guid id)
        {
            _log.LogDebug("Fetching item with ID {ItemId}.", id);
            try
            {
                var item = await _db.Items.FindAsync(id);
                if (item == null)
                {
                    _log.LogWarning("Item with ID {ItemId} not found.", id);
                    return null;
                }

                _log.LogDebug("Fetched item with ID {ItemId}.", id);
                return PropCopier.Copy(item, new ItemResponseDto());
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred while fetching item with ID {ItemId}.", id);
                throw;
            }
        }

        public async Task<(List<ItemResponseDto>, List<UpdateItemDto>)> UpdateItems(
            List<UpdateItemDto> items
        )
        {
            _log.LogDebug("Starting update for {Count} item(s).", items.Count);
            var updatedItems = new List<ItemResponseDto>();
            var failedItems = new List<UpdateItemDto>();

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
                        updatedItems.Add(PropCopier.Copy(existingItem, new ItemResponseDto()));
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

        public async Task<(ItemResponseDto[], int)> SearchItems(string query, int page, int limit)
        {
            _log.LogDebug(
                "Searching items with query '{Query}', Page {Page}, Limit {Limit}.",
                query,
                page,
                limit
            );
            try
            {
                var items = await _db
                    .Items.Where(i => (i.Brand ?? "").ToLower().Contains(query) && !i.IsDeleted)
                    .OrderBy(i => i.Id)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                var result = items.Select(i => PropCopier.Copy(i, new ItemResponseDto())).ToArray();
                _log.LogInformation("Found {Count} matching item(s).", items.Count);
                return (result, items.Count);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An error occurred while searching items.");
                throw;
            }
        }
    }
}
