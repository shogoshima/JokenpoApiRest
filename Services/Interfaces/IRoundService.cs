using JokenpoApiRest.Models;
using System.Linq.Expressions;

namespace JokenpoApiRest.Services.Interfaces;

public interface IRoundService
{
  Task<IEnumerable<Round>> GetAllAsync(Expression<Func<Round, bool>>? filter = null);
  Task<Round?> GetByIdAsync(int id);
  Task<Round> CreateAsync(Round round);
  Task<bool> UpdateAsync(Round round);
  Task<bool> DeleteAsync(int id);
}