using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JokenpoApiRest.Data;
using JokenpoApiRest.Models;

namespace JokenpoApiRest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(AppDbContext db) : ControllerBase
{
  private readonly AppDbContext _db = db;

  // Teste
  [HttpGet]
  public async Task<ActionResult<IEnumerable<User>>> Get()
      => await _db.Users.ToListAsync();
}