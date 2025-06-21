using JokenpoApiRest.Models;

namespace JokenpoApiRest.Services.Interfaces;

// Criando funções a mais para a interface sem muita necessidade
// pois vou popular diretamente esses dados
// mas mais pela boa prática
public interface IHandService
{
  Task<IEnumerable<Hand>> GetAllAsync();
  Task<Hand?> GetByIdAsync(int id);
  Task<Hand> CreateAsync(Hand hand);
  Task<bool> UpdateAsync(Hand hand);
  Task<bool> DeleteAsync(int id);
}