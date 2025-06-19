using JokenpoApiRest.Models;

namespace JokenpoApiRest.Services.Interfaces;

public interface IParticipationService
{
  Task<IEnumerable<Participation>> GetAllAsync();
  Task<Participation?> GetByIdAsync(int roundId, int userId);
  Task<Participation> CreateAsync(Participation participation);
  Task<bool> UpdateAsync(Participation participation);
  Task<bool> DeleteAsync(int roundId, int userId);
}