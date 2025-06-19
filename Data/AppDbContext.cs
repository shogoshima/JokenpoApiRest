using Microsoft.EntityFrameworkCore;
using JokenpoApiRest.Models;

namespace JokenpoApiRest.Data;

// Criação da classe utilizada para manusear o banco de dados
// Ele aceita configurações de conexão (string, provedor, etc.)
// Sendo herdado de DbContext, será possível utilizar as ferramentas do EntityFrameworkCore
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  // Declarando as tabelas
  public DbSet<User> Users { get; set; }
  public DbSet<Round> Rounds { get; set; }
  public DbSet<Participation> Participations { get; set; }
  public DbSet<Hand> Hands { get; set; }
  public DbSet<HandRelation> HandRelations { get; set; }

  // Quando o modelo for criado, rodará isso
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Declarando chave composta em participação
    modelBuilder.Entity<Participation>()
        .HasKey(p => new { p.UserId, p.RoundId });

    // Declarando chave composta na relação de mãos
    modelBuilder.Entity<HandRelation>()
        .HasKey(p => new { p.WinnerHandId, p.LoserHandId });

    // O nome do usuário é UNIQUE
    modelBuilder.Entity<User>()
        .HasIndex(u => u.Name)
        .IsUnique();

    // O nome da mão é UNIQUE
    modelBuilder.Entity<Hand>()
        .HasIndex(h => h.Name)
        .IsUnique();
  }

}