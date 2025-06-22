using JokenpoApiRest.Models;

namespace JokenpoApiRest.Services.Interfaces;

public interface IRoundFinalizerService
{
  Task<RoundResultDto> ComputeWinners(IEnumerable<Participation> participations);
}