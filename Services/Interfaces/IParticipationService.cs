using JokenpoApiRest.Models;
using System.Linq.Expressions;

namespace JokenpoApiRest.Services.Interfaces;

public interface IParticipationService
{
  Task<IEnumerable<Participation>> GetAllAsync(Expression<Func<Participation, bool>>? filter = null);
  Task<Participation?> GetByIdAsync(int userId, int roundId);
  Task<Participation> CreateAsync(Participation participation);
  Task<bool> UpdateAsync(Participation participation);
  Task<bool> DeleteAsync(int userId, int roundId);
}