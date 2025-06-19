using JokenpoApiRest.Models;

namespace JokenpoApiRest.Services.Interfaces;

public interface IUserService
{
  Task<User?> GetByIdAsync(int id);
  Task<User?> GetByNameAsync(string name);
  Task<User> CreateAsync(User user);
  Task<bool> UpdateAsync(User user);
  Task<bool> DeleteAsync(int id);
}