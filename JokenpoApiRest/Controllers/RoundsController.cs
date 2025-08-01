using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JokenpoApiRest.Controllers;

/// <summary>
/// Gerencia operações relacionadas às rodadas:
/// <list type="bullet">
/// <item>
/// <description>Consultar rodada</description>
/// </item>
/// <item>
/// <description>Criar nova rodada</description>
/// </item>
/// <item>
/// <description>Registar, atualizar e excluir participações</description>
/// </item>
/// <item>
/// <description>Finalizar rodada e calcular vencedores</description>
/// </item>
/// </list>
/// </summary>
[ApiController]
[Route("api/rounds")]
public class RoundsController(
  IRoundService roundService,
  IParticipationService participationService,
  IUserService userService,
  IHandService handService,
  IRoundFinalizerService roundFinalizerService) : ControllerBase
{
  private readonly IRoundService _roundService = roundService;
  private readonly IParticipationService _participationService = participationService;
  private readonly IUserService _userService = userService;
  private readonly IHandService _handService = handService;
  private readonly IRoundFinalizerService _roundFinalizerService = roundFinalizerService;

  /// <summary>
  /// Obtém a rodada aberta atual, junto com usuários que já jogaram e os pendentes
  /// </summary>
  /// <returns>
  /// Erro ou rodada atual com participantes associados.
  /// </returns>
  [HttpGet("current")]
  [ProducesResponseType(typeof(RoundDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetCurrentOpenRound()
  {
    var open = await _roundService.GetAllAsync(r => r.Status == RoundStatus.Open);
    var round = open.LastOrDefault();
    if (round == null) return NotFound(new { message = "Não há rodada aberta no momento" });

    // Carrega todas as participações da rodada
    var participations = await _participationService.GetAllAsync(p => p.RoundId == round.Id);

    // Carrega os nomes dos jogadores que já jogaram, e dos que não jogaram
    var played = participations.Where(p => p.HandId != null).ToList().Select(p => p.User!.Name);
    var pending = participations.Where(p => p.HandId == null).ToList().Select(p => p.User!.Name);

    // Retorna a rodada, os jogadores que jogaram e aqueles que ainda não jogaram
    return Ok(new RoundDto { Data = round, PlayedUsers = played, PendingUsers = pending });
  }

  /// <summary>
  /// Obtém uma rodada pelo seu identificador, incluindo status e listas de usuários
  /// </summary>
  /// <returns>
  /// Erro ou a rodada com participantes associados.
  /// </returns>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(RoundDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById(int id)
  {
    var round = await _roundService.GetByIdAsync(id);
    if (round == null) return NotFound();

    // Carrega todas as participações da rodada
    var participations = await _participationService.GetAllAsync(p => p.RoundId == round.Id);

    // Carrega os nomes dos jogadores que já jogaram, e dos que não jogaram
    var played = participations.Where(p => p.HandId != null).ToList().Select(p => p.User!.Name);
    var pending = participations.Where(p => p.HandId == null).ToList().Select(p => p.User!.Name);

    // Retorna as rodadas, os jogadores que jogaram e aqueles que ainda não jogaram
    return Ok(new RoundDto { Data = round, PlayedUsers = played, PendingUsers = pending });
  }

  /// <summary>
  /// Cria uma nova rodada aberta.
  /// </summary>
  /// <returns>
  /// Erro, ou a nova rodada criada.
  /// </returns>
  [HttpPost]
  [ProducesResponseType(typeof(Round), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
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

  /// <summary>
  /// Registra a participação de um usuário em uma rodada
  /// </summary>
  /// <returns>
  /// Erro, ou 204 No Content se sucesso.
  /// </returns>
  [HttpPost("{id}/participations")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    // Criar participação
    await _participationService.CreateAsync(
      new Participation { RoundId = id, UserId = dto.UserId, HandId = dto.HandId }
    );

    // Para não expor a jogada do usuário
    return NoContent();
  }

  /// <summary>
  /// Atualiza a jogada de um usuário em uma rodada.
  /// </summary>
  /// <returns>
  /// Erro, ou 204 No Content se sucesso.
  /// </returns>
  [HttpPut("{id}/participations")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    // Atualizar participação
    existing.HandId = dto.HandId;
    await _participationService.UpdateAsync(existing);

    // Para não expor a jogada do usuário
    return NoContent();
  }

  /// <summary>
  /// Remove a participação de um usuário em uma rodada.
  /// </summary>
  /// <returns>
  /// Erro, ou 204 No Content se sucesso.
  /// </returns>
  [HttpDelete("{id}/participations/{userId}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<IActionResult> DeleteParticipation(int id, int userId)
  {
    var round = await _roundService.GetByIdAsync(id);
    if (round == null) return NotFound(new { message = "Rodada não encontrada" });
    if (round.Status == RoundStatus.Closed) return Conflict(new { message = "Rodada já finalizada" });

    // Checar se o usuário existe
    var user = await _userService.GetByIdAsync(userId);
    if (user == null) return NotFound(new { message = "Usuário não cadastrado." });

    // Checar se a participação existe
    var exists = await _participationService.GetByIdAsync(userId, id);
    if (exists == null) return NotFound();

    // Remover a participação
    await _participationService.DeleteAsync(userId, id);

    // Para não expor a jogada do usuário
    return NoContent();
  }

  /// <summary>
  /// Finaliza uma rodada, calcula e retorna os vencedores.
  /// </summary>
  /// <returns>
  /// Erro, ou um objeto com os vencedores.
  /// </returns>
  [HttpPost("{id}/finalize")]
  [ProducesResponseType(typeof(RoundResultDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<IActionResult> FinalizeRound(int id)
  {
    // Checa dados da rodada
    var round = await _roundService.GetByIdAsync(id);
    if (round == null) return NotFound();

    // Carrega todas as participações da rodada, e os jogadores
    var participations = await _participationService.GetAllAsync(p => p.RoundId == id);

    // Verifica se existe alguma participação sem HandId
    var pending = participations.Where(p => p.HandId == null).ToList();
    if (pending.Count != 0)
    {
      // Exemplo de retorno com lista de UserIds pendentes
      var pendingNames = pending.Select(p => p.User!.Name);
      return BadRequest(new
      {
        message = "Ainda faltam jogadas de alguns participantes.",
        pending = pendingNames
      });
    }

    if (participations.ToList().Count <= 1)
    {
      return Conflict(new { message = "Jogadores insuficientes para calcular resultado." });
    }

    // Calcular o resultado
    RoundResultDto roundResulDto = await _roundFinalizerService.ComputeWinners(participations);

    // Finaliza a rodada
    round.Status = RoundStatus.Closed;
    await _roundService.UpdateAsync(round);

    return Ok(roundResulDto);
  }
}