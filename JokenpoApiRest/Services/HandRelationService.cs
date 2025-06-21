using JokenpoApiRest.Data;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JokenpoApiRest.Services;

public class HandRelationService(AppDbContext db) : IHandRelationService
{
  private readonly AppDbContext _db = db;

  public async Task<IEnumerable<HandRelation>> GetAllAsync()
  {
    return await _db.HandRelations
                    .Include(hr => hr.WinnerHand)
                    .Include(hr => hr.LoserHand)
                    .AsNoTracking()
                    .ToListAsync();
  }

  public async Task<HandRelation> CreateAsync(HandRelation handRelation)
  {
    _db.HandRelations.Add(handRelation);
    await _db.SaveChangesAsync();
    return handRelation;
  }

  public async Task<bool> DeleteAsync(int winnerHandId, int loserHandId)
  {
    var handRelation = await _db.HandRelations
                        .FindAsync(winnerHandId, loserHandId);
    if (handRelation == null) return false;

    _db.HandRelations.Remove(handRelation);
    await _db.SaveChangesAsync();
    return true;
  }
}