using API.Data;
using API.DTOs;
using API.Models;
using API.Utils;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RestockService(ApiDbContext db, ILogger<RestockService> log)
    {
        private readonly ApiDbContext _db = db;
        private readonly ILogger<RestockService> _log = log;

        public async Task<(List<ItemResponseDto> restocked, List<ItemResponseDto> added)> Restock(
            List<RestockDto> items
        )
        {
            var restocked = new List<ItemResponseDto>();
            var added = new List<ItemResponseDto>();

            _log.LogInformation("Starting restock operation for {Count} items.", items.Count);
            var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in items)
                {
                    try
                    {
                        var hash = Cryptics.ComputeHash(item);
                        var existing = await _db.Items.FirstOrDefaultAsync(i => i.Hash == hash);

                        if (existing != null)
                        {
                            existing.Stock += item.Stock;
                            restocked.Add(PropCopier.Copy(existing, new ItemResponseDto()));
                            _log.LogInformation(
                                "Restocked item with ID: {Id}, new stock: {Stock}.",
                                existing.Id,
                                existing.Stock
                            );
                        }
                        else
                        {
                            var newItem = PropCopier.Copy(item, new ItemModel());
                            newItem.Hash = Cryptics.ComputeHash(newItem);
                            var response = await _db.Items.AddAsync(newItem);
                            added.Add(PropCopier.Copy(response.Entity, new ItemResponseDto()));

                            _log.LogInformation("Added new item with hash: {Hash}.", hash);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.LogError("Error processing item {Brand}: {Ex}", item.Brand, ex);
                    }
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _log.LogInformation("Restock operation completed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _log.LogError("Restock transaction failed: {Ex}", ex);
                throw; // Re-throw to ensure proper error handling in the controller
            }

            return (restocked, added);
        }
    }
}
