using JokenpoApiRest.Data;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JokenpoApiRest.Services;

public class RoundService(AppDbContext db) : IRoundService
{
  private readonly AppDbContext _db = db;

  public async Task<IEnumerable<Round>> GetAllAsync(
    Expression<Func<Round, bool>>? filter = null)
  {
    IQueryable<Round> query = _db.Rounds
                    .AsNoTracking();

    if (filter != null)
      query = query.Where(filter);

    return await query.ToListAsync();
  }

  public async Task<Round?> GetByIdAsync(int id)
  {
    return await _db.Rounds
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r =>
                        r.Id == id);
  }

  public async Task<Round> CreateAsync(Round round)
  {
    _db.Rounds.Add(round);
    await _db.SaveChangesAsync();
    return round;
  }

  public async Task<bool> UpdateAsync(Round round)
  {
    var exists = await _db.Rounds.AnyAsync(r => r.Id == round.Id);
    if (!exists) return false;

    _db.Rounds.Update(round);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var round = await _db.Rounds.FindAsync(id);
    if (round == null) return false;

    _db.Rounds.Remove(round);
    await _db.SaveChangesAsync();
    return true;
  }
}