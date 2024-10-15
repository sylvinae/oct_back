using System.Net.Http.Headers;
using API.Data;
using API.DTOs;
using API.Models;
using API.Utils;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Services
{
    public class ItemService(ApiDbContext db, ILogger<ItemService> log)
    {
        private readonly ApiDbContext _db = db;
        private readonly ILogger<ItemService> _log = log;

        public async Task<(List<ItemResponseDto> created, List<CreateItemDto> failed)> CreateItem(
            CreateItemDto[] items,
            int batchSize = 50
        )
        {
            var createdItems = new List<ItemResponseDto>();
            var failedItems = new List<CreateItemDto>();

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                for (int i = 0; i < items.Length; i += batchSize)
                {
                    var batch = items.Skip(i).Take(batchSize).ToArray();

                    foreach (var item in batch)
                    {
                        _log.LogInformation("Batch creating:");
                        try
                        {
                            var newItem = PropCopier.Copy(item, new ItemModel());
                            await _db.Items.AddAsync(newItem);
                            createdItems.Add(PropCopier.Copy(newItem, new ItemResponseDto()));
                            _log.LogInformation("Created item {}.", newItem.Id);
                        }
                        catch (Exception ex)
                        {
                            _log.LogError(
                                "Failed to create item {}, {}. {}.",
                                item.Brand,
                                item.Generic,
                                ex
                            );
                            failedItems.Add(item);
                        }
                    }
                }
                await _db.SaveChangesAsync();
                _log.LogInformation("Committed changes.");
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _log.LogError("Error, rolling back changes: {}", ex);
                await transaction.RollbackAsync();
                throw;
            }

            return (createdItems, failedItems);
        }

        public async Task<(List<ItemResponseDto> items, int count)> GetAllItems(int page, int limit)
        {
            try
            {
                var croppedRows = await _db
                    .Items.Where(i => !i.IsDeleted)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();
                _log.LogInformation("Cropping rows...");

                var response = croppedRows
                    .Select(r => PropCopier.Copy(r, new ItemResponseDto()))
                    .ToList();

                _log.LogInformation("Transmuting...");

                _log.LogInformation("Fetched {Count} items from page {Page}", response.Count, page);
                return (response, croppedRows.Count);
            }
            catch (Exception ex)
            {
                _log.LogError("Error fetching items: {}", ex);
                throw;
            }
        }

        public async Task<(List<ItemResponseDto> updated, List<UpdateItemDto> failed)> UpdateItems(
            UpdateItemDto[] items,
            int batchSize = 10
        )
        {
            var failedItems = new List<UpdateItemDto>();
            var updatedItems = new List<ItemResponseDto>();

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                for (int i = 0; i < items.Length; i += batchSize)
                {
                    var batch = items.Skip(i).Take(batchSize).ToArray();

                    foreach (var item in batch)
                    {
                        try
                        {
                            _log.LogInformation("Batch updating...");
                            var patchItem = PropCopier.Copy(item, new ItemModel { Id = item.Id });
                            var existingItem = await _db.Items.FindAsync(item.Id);
                            if (existingItem != null)
                                _db.Entry(existingItem).CurrentValues.SetValues(patchItem);

                            updatedItems.Add(PropCopier.Copy(item, new ItemResponseDto()));

                            _log.LogInformation("Updated:{}", item.Id);
                        }
                        catch (Exception ex)
                        {
                            _log.LogError("Error:{}", ex);
                            failedItems.Add(item);
                        }
                        _log.LogInformation("Batch finished.");
                    }
                }
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return (updatedItems, failedItems);
        }

        public async Task<(List<Guid> deletedItems, List<Guid> failedItems)> DeleteItems(
            Guid[] itemIds
        )
        {
            var failedItems = new List<Guid>();
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var tasks = itemIds.Select(async id =>
                {
                    try
                    {
                        var item = await _db.Items.FindAsync(id);
                        if (item != null)
                        {
                            _log.LogInformation("Deleted item: {}", item.Id);
                            item.IsDeleted = true;
                        }
                        else
                        {
                            _log.LogError("Failed to delete item: {}", id);
                            failedItems.Add(id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.LogError("Error: {}", ex);
                        failedItems.Add(id);
                    }
                });
                await Task.WhenAll(tasks);
                await _db.SaveChangesAsync();
                var deletedItems = itemIds.Except(failedItems).ToList();
                return (deletedItems, failedItems);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ItemResponseDto?> GetItem(Guid id)
        {
            try
            {
                var item = await _db.Items.FindAsync(id);
                _log.LogInformation("Found item: {]}", id);
                return item != null ? PropCopier.Copy(item, new ItemResponseDto()) : null;
            }
            catch (Exception ex)
            {
                _log.LogError("Not found item: {ex}", ex);
                throw;
            }
        }
    }
}
