using JokenpoApiRest.Data;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;

namespace JokenpoApiRest.Services;

public class RoundFinalizerService(
  IHandRelationService handRelationService) : IRoundFinalizerService
{
  private readonly IHandRelationService _handRelationService = handRelationService;

  public async Task<RoundResultDto> ComputeWinners(IEnumerable<Participation> participations)
  {
    // Calcular o resultado da rodada
    // Criando um dicionário em que a chave é o id da jogada
    // e o valor é uma lista dos usuários que jogaram aquela jogada
    var playedDict = participations
        .Where(p => p.HandId != null)
        .GroupBy(p => p.HandId!.Value)
        .ToDictionary(
          g => g.Key,
          g => g.Select(p => p.User!.Name).ToList()
        );

    // Conjunto de todas as mãos jogadas 
    var playedHands = playedDict.Keys.ToHashSet();

    // Carregando as relações entre as jogadas
    var handRelations = await _handRelationService.GetAllAsync();

    // Calcular quais mãos foram derrotadas
    var defeatedHands = handRelations
    .Where(hr =>
        playedHands.Contains(hr.WinnerHandId) &&
        playedHands.Contains(hr.LoserHandId)
    )
    .Select(hr => hr.LoserHandId)
    .ToHashSet();

    // As mãos vencedoras são as que não estão em defeatedHands
    var winningHands = playedHands.Except(defeatedHands).ToList();

    // Montar lista de nomes vencedores
    var winners = winningHands
        .SelectMany(handId => playedDict[handId])
        .ToList();

    // Criando a mensagem
    string message;
    if (winners.Count == 1)
      message = "Temos um vencedor!";
    else if (winners.Count > 1)
      message = "Empate!";
    else
      message = "Sem vencedores.";

    // Gerando a lista de participações final
    List<ParticipationResultDto> participationResults = [.. participations
        .Select(p => new ParticipationResultDto
        {
          Name = p.User!.Name,
          HandName = p.Hand!.Name
        })];

    return new RoundResultDto
    {
      Message = message,
      Winners = winners,
      Participations = participationResults
    };
  }
}