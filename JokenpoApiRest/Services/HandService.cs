using JokenpoApiRest.Data;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JokenpoApiRest.Services;

public class HandService(AppDbContext db) : IHandService
{
  private readonly AppDbContext _db = db;

  public async Task<IEnumerable<Hand>> GetAllAsync()
  {
    return await _db.Hands
                    .AsNoTracking()
                    .ToListAsync();
  }

  public async Task<Hand?> GetByIdAsync(int id)
  {
    return await _db.Hands
                    .AsNoTracking()
                    .FirstOrDefaultAsync(h =>
                        h.Id == id);
  }

  public async Task<Hand> CreateAsync(Hand hand)
  {
    _db.Hands.Add(hand);
    await _db.SaveChangesAsync();
    return hand;
  }

  public async Task<bool> UpdateAsync(Hand hand)
  {
    var exists = await _db.Hands
                          .AnyAsync(h =>
                              h.Id == hand.Id);

    if (!exists) return false;

    _db.Hands.Update(hand);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var hand = await _db.Hands
                        .FindAsync(id);
    if (hand == null) return false;

    _db.Hands.Remove(hand);
    await _db.SaveChangesAsync();
    return true;
  }
}