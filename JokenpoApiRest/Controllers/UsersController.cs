using Microsoft.AspNetCore.Mvc;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;

namespace JokenpoApiRest.Controllers;

/// <summary>
/// Gerencia operações de usuário:
/// <list type="bullet">
/// <item>
/// <description>Criação ou obtenção por nome</description>
/// </item>
/// <item>
/// <description>Consulta de dados e rodadas associadas</description>
/// </item>
/// </list>
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService, IParticipationService participationService) : ControllerBase
{
  private readonly IUserService _userService = userService;
  private readonly IParticipationService _participationService = participationService;

  /// <summary>
  /// Obtém um usuário pelo seu identificador.
  /// </summary>
  /// <returns>O usuário encontrado ou 404 se não existir.</returns>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById(int id)
  {
    var user = await _userService.GetByIdAsync(id);
    if (user == null) return NotFound();
    return Ok(user);
  }

  /// <summary>
  /// Lista todas as rodadas em que o usuário participou, com os seguintes atributos:
  /// <list type="bullet">
  /// <item>
  /// <description>Data de criação</description>
  /// </item>
  /// <item>
  /// <description>Status (0 se estiver aberto, 1 se estiver finalizado)</description>
  /// </item>
  /// </list>
  /// </summary>
  /// <returns>
  /// Lista de <see cref="Round"/> distintas, ou 404 se o usuário não existir
  /// </returns>
  [HttpGet("{id}/rounds")]
  [ProducesResponseType(typeof(IEnumerable<Round>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetRounds(int id)
  {
    var user = await _userService.GetByIdAsync(id);
    if (user == null) return NotFound();

    var participations = await _participationService.GetAllAsync(p => p.UserId == id);

    var rounds = participations
        .Select(p => p.Round)
        .Distinct()
        .ToList();

    return Ok(rounds);
  }

  /// <summary>
  /// Cria um novo usuário ou retorna um já existente
  /// </summary>
  /// <param name="dto">Dados de entrada para criação do usuário</param>
  /// <returns>
  /// 200 OK com usuário existente, ou 201 Created com o novo usuário.
  /// </returns>
  [HttpPost]
  [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> CreateOrGet([FromBody] UserDto dto)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var exists = await _userService.GetByNameAsync(dto.Name);
    if (exists != null)
      return Ok(exists); // retorna usuário já existente

    // Usuário não existe, vou criar
    var newUser = await _userService.CreateAsync(new User { Name = dto.Name });
    // retorna 201 Created com header Location apontando para GET /api/users/{newUser.Id}
    return CreatedAtAction(
      nameof(GetById),         // Action
      new { id = newUser.Id }, // Rota para preencher os parâmetros da action (ex: /users/{id}/rounds)
      newUser                  // Corpo da resposta
    );
  }
}