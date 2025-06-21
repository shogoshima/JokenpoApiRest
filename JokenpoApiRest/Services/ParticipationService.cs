using System.Linq.Expressions;
using JokenpoApiRest.Data;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JokenpoApiRest.Services;

public class ParticipationService(AppDbContext db) : IParticipationService
{
  private readonly AppDbContext _db = db;

  public async Task<IEnumerable<Participation>> GetAllAsync(
    Expression<Func<Participation, bool>>? filter = null)
  {
    // Não expor as jogadas escolhidas dos usuários
    IQueryable<Participation> query = _db.Participations
                                          .Include(p => p.User)
                                          .Include(p => p.Round)
                                          .Include(p => p.Hand)
                                          .AsNoTracking();

    if (filter != null)
      query = query.Where(filter);

    return await query.ToListAsync();
  }

  public async Task<Participation?> GetByIdAsync(int userId, int roundId)
  {
    return await _db.Participations
                    .Include(p => p.User)
                    .Include(p => p.Round)
                    .Include(p => p.Hand)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p =>
                        p.UserId == userId &&
                        p.RoundId == roundId);
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
                              p.UserId == participation.UserId &&
                              p.RoundId == participation.RoundId);

    if (!exists) return false;

    _db.Participations.Update(participation);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteAsync(int userId, int roundId)
  {
    var participation = await _db.Participations
                                 .FindAsync(userId, roundId);
    if (participation == null) return false;

    _db.Participations.Remove(participation);
    await _db.SaveChangesAsync();
    return true;
  }
}