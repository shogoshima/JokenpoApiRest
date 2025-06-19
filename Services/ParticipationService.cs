using JokenpoApiRest.Data;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JokenpoApiRest.Services;

public class ParticipationService(AppDbContext db) : IParticipationService
{
  private readonly AppDbContext _db = db;

  public async Task<IEnumerable<Participation>> GetAllAsync()
  {
    return await _db.Participations
                    .Include(p => p.User)
                    .Include(p => p.Round)
                    .Include(p => p.Hand)
                    .AsNoTracking()
                    .ToListAsync();
  }

  public async Task<Participation?> GetByIdAsync(int roundId, int userId)
  {
    return await _db.Participations
                    .Include(p => p.User)
                    .Include(p => p.Round)
                    .Include(p => p.Hand)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p =>
                        p.RoundId == roundId &&
                        p.UserId == userId);
  }

  public async Task<Participation> CreateAsync(Participation participation)
  {
    _db.Participations.Add(participation);
    await _db.SaveChangesAsync();
    return participation;
  }

  public async Task<bool> UpdateAsync(Participation participation)
  {
    var exists = await _db.Participations
                          .AnyAsync(p =>
                              p.RoundId == participation.RoundId &&
                              p.UserId == participation.UserId);

    if (!exists) return false;

    _db.Participations.Update(participation);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteAsync(int roundId, int userId)
  {
    var participation = await _db.Participations
                                 .FindAsync(roundId, userId);
    if (participation == null) return false;

    _db.Participations.Remove(participation);
    await _db.SaveChangesAsync();
    return true;
  }
}