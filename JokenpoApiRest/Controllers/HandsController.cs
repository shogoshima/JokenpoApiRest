using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JokenpoApiRest.Controllers;

[ApiController]
[Route("api/hands")]
public class HandsController(IHandService handService) : ControllerBase
{
  private readonly IHandService _handService = handService;

  /// <summary>
  /// Lista as jogadas disponíveis
  /// </summary>
  /// <returns>A lista de jogadas disponíveis</returns>
  [HttpGet]
  [ProducesResponseType(typeof(List<Hand>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetAll()
  {
    var hands = await _handService.GetAllAsync();
    if (hands == null) return NotFound();
    return Ok(hands);
  }
}