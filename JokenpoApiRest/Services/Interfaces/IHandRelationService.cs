using JokenpoApiRest.Models;

namespace JokenpoApiRest.Services.Interfaces;

public interface IHandRelationService
{
  Task<IEnumerable<HandRelation>> GetAllAsync();
  Task<HandRelation> CreateAsync(HandRelation handRelation);
  Task<bool> DeleteAsync(int winnerHandId, int loserHandId);
}