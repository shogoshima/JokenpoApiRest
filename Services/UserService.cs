using JokenpoApiRest.Data;
using JokenpoApiRest.Models;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JokenpoApiRest.Services;

public class UserService(AppDbContext db) : IUserService
{
  private readonly AppDbContext _db = db;

  public async Task<User?> GetByIdAsync(int id)
  {
    return await _db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.Id == id);
  }

  public async Task<User?> GetByNameAsync(string name)
  {
    return await _db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.Name == name);
  }

  public async Task<User> CreateAsync(User user)
  {
    _db.Users.Add(user);
    await _db.SaveChangesAsync();
    return user;
  }

  public async Task<bool> UpdateAsync(User user)
  {
    var exists = await _db.Users.AnyAsync(u => u.Id == user.Id);
    if (!exists) return false;

    _db.Users.Update(user);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var user = await _db.Users.FindAsync(id);
    if (user == null) return false;

    _db.Users.Remove(user);
    await _db.SaveChangesAsync();
    return true;
  }

}