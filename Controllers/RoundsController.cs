using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JokenpoApiRest.Controllers;

[ApiController]
[Route("api/rounds")]
public class RoundsController(
  IRoundService roundService,
  IParticipationService participationService,
  IUserService userService,
  IHandService handService,
  IHandRelationService handRelationService) : ControllerBase
{
  private readonly IRoundService _roundService = roundService;
  private readonly IParticipationService _participationService = participationService;
  private readonly IUserService _userService = userService;
  private readonly IHandService _handService = handService;
  private readonly IHandRelationService _handRelationService = handRelationService;

  [HttpGet("current")]
  public async Task<IActionResult> GetCurrentOpenRound()
  {
    var open = await _roundService.GetAllAsync(r => r.Status == RoundStatus.Open);
    var round = open.LastOrDefault();
    if (round == null) return NotFound(new { message = "Não há rodada aberta no momento" });

    // Carrega todas as participações da rodada
    var participations = await _participationService.GetAllAsync(p => p.RoundId == round.Id);

    // Carrega os jogadores que já jogaram, e os que não jogaram
    var played = participations.Where(p => p.HandId != null).ToList();
    var pending = participations.Where(p => p.HandId == null).ToList();
    var playedUsers = played.Select(p => p.User!.Name);
    var pendingUsers = pending.Select(p => p.User!.Name);

    // Retorna a rodada, os jogadores que jogaram e aqueles que ainda não jogaram
    return Ok(new { round, playedUsers, pendingUsers });
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var round = await _roundService.GetByIdAsync(id);
    if (round == null) return NotFound();

    // Carrega todas as participações da rodada
    var participations = await _participationService.GetAllAsync(p => p.RoundId == round.Id);

    // Carrega os jogadores que já jogaram, e os que não jogaram
    var played = participations.Where(p => p.HandId != null).ToList();
    var pending = participations.Where(p => p.HandId == null).ToList();
    var playedUsers = played.Select(p => p.User!.Name);
    var pendingUsers = pending.Select(p => p.User!.Name);

    // Retorna as rodadas, os jogadores que jogaram e aqueles que ainda não jogaram
    return Ok(new { round, playedUsers, pendingUsers });
  }

  [HttpPost]
  public async Task<IActionResult> CreateNewRound()
  {
    var open = await _roundService.GetAllAsync(r => r.Status == RoundStatus.Open);
    if (open.Any()) return Conflict(new { message = "Já existe uma rodada aberta." });

    var newRound = await _roundService.CreateAsync(new Round { });
    return CreatedAtAction(
      nameof(GetById),
      new { id = newRound.Id },
      newRound);
  }

  [HttpPost("{id}/participations")]
  public async Task<IActionResult> RegisterParticipation(
    int id,
    [FromBody] ParticipationDto dto)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    // Checar se a rodada existe, e está aberta
    var round = await _roundService.GetByIdAsync(id);
    if (round == null) return NotFound(new { message = "Rodada não encontrada" });
    if (round.Status == RoundStatus.Closed) return Conflict(new { message = "Rodada já finalizada" });

    // Checar se o usuário existe
    var user = await _userService.GetByIdAsync(dto.UserId);
    if (user == null) return NotFound(new { message = "usuário não cadastrado." });

    if (dto.HandId != null)
    {
      // Checar se a jogada é válida
      var hand = await _handService.GetByIdAsync(dto.HandId.Value);
      if (hand == null) return Conflict(new { message = "jogada inválida" });
    }

    // Checar se a participação já existe
    var exists = await _participationService.GetByIdAsync(dto.UserId, id);
    if (exists != null) return Conflict(new { message = "Usuário já cadastrado na rodada." });

    await _participationService.CreateAsync(
      new Participation { RoundId = id, UserId = dto.UserId, HandId = dto.HandId }
    );

    // Para não expor a jogada do usuário
    return NoContent();
  }

  [HttpPut("{id}/participations")]
  public async Task<IActionResult> UpdateParticipation(
    int id,
    [FromBody] ParticipationDto dto)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    // Checar se a rodada existe, e está aberta
    var round = await _roundService.GetByIdAsync(id);
    if (round == null) return NotFound(new { message = "Rodada não encontrada" });
    if (round.Status == RoundStatus.Closed) return Conflict(new { message = "Rodada já finalizada" });

    // Checar se o usuário existe
    var user = await _userService.GetByIdAsync(dto.UserId);
    if (user == null) return NotFound(new { message = "usuário não cadastrado." });

    if (dto.HandId != null)
    {
      // Checar se a jogada é válida
      var hand = await _handService.GetByIdAsync(dto.HandId.Value);
      if (hand == null) return Conflict(new { message = "jogada inválida" });
    }

    // Checar se a participação existe
    var existing = await _participationService.GetByIdAsync(dto.UserId, id);
    if (existing == null) return NotFound();

    existing.HandId = dto.HandId;
    await _participationService.UpdateAsync(existing);

    return NoContent();
  }

  [HttpDelete("{id}/participations/{userId}")]
  public async Task<IActionResult> DeleteParticipation(int id, int userId)
  {
    var round = await _roundService.GetByIdAsync(id);
    if (round == null) return NotFound(new { message = "Rodada não encontrada" });
    if (round.Status == RoundStatus.Closed) return Conflict(new { message = "Rodada já finalizada" });

    // Checar se o usuário existe
    var user = await _userService.GetByIdAsync(userId);
    if (user == null) return NotFound(new { message = "Usuário não cadastrado." });

    var exists = await _participationService.GetByIdAsync(userId, id);
    if (exists == null) return NotFound();

    await _participationService.DeleteAsync(userId, id);
    return NoContent();
  }

  [HttpPost("{id}/finalize")]
  public async Task<IActionResult> FinalizeRound(int id)
  {
    // Checa dados da rodada
    var round = await _roundService.GetByIdAsync(id);
    if (round == null) return NotFound();

    // Carrega todas as participações da rodada, e os jogadores
    var participations = await _participationService.GetAllAsync(p => p.RoundId == id);
    var played = participations.Where(p => p.HandId != null).ToList();
    var pending = participations.Where(p => p.HandId == null).ToList();

    // Verifica se existe alguma sem HandId
    if (pending.Count != 0)
    {
      // Exemplo de retorno com lista de UserIds pendentes
      var pendingNames = pending.Select(p => p.User!.Name);
      return BadRequest(new
      {
        Message = "Ainda faltam jogadas de alguns participantes.",
        Pending = pendingNames
      });
    }

    if (played.Count <= 1)
    {
      return Conflict(new { message = "Jogadores insuficientes" });
    }

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
    // Carregando as relações
    var handRelations = await _handRelationService.GetAllAsync();
    foreach (var hr in handRelations)
    {
      if (playedDict.TryGetValue(hr.WinnerHandId, out var winnerNames) && winnerNames.Count != 0
          && playedDict.ContainsKey(hr.LoserHandId))
      {
        playedDict[hr.LoserHandId] = [];
      }
    }

    // Montando a lista de quem ganhou
    var winners = new List<string>();
    foreach (var kv in playedDict)
    {
      if (kv.Value.Count != 0)
        winners.AddRange(kv.Value);
    }

    // Finaliza a rodada
    round.Status = RoundStatus.Closed;
    await _roundService.UpdateAsync(round);
    return Ok(new { winners });
  }
}