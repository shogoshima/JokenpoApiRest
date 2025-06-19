using JokenpoApiRest.Models;

namespace JokenpoApiRest.Services.Interfaces;

public interface IRoundService
{
  Task<IEnumerable<Round>> GetAllAsync();
  Task<Round?> GetByIdAsync(int id);
  Task<Round> CreateAsync(Round round);
  Task<bool> UpdateAsync(Round round);
  Task<bool> DeleteAsync(int id);
}