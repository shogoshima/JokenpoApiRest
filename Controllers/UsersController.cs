using Microsoft.AspNetCore.Mvc;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;

namespace JokenpoApiRest.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService, IParticipationService participationService) : ControllerBase
{
  private readonly IUserService _userService = userService;
  private readonly IParticipationService _participationService = participationService;

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(int id)
  {
    var user = await _userService.GetByIdAsync(id);
    if (user == null) return NotFound();
    return Ok(user);
  }

  [HttpGet("{id}/rounds")]
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

  [HttpPost]
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