using Microsoft.EntityFrameworkCore;
using JokenpoApiRest.Models;

namespace JokenpoApiRest.Data;

/// <summary>
/// Criação da classe utilizada para manusear o banco de dados. 
/// Sendo herdado de DbContext, será possível utilizar as ferramentas do EntityFrameworkCore
/// </summary>
/// <param name="options">Configurações de conexão</param>
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

    // Populando a tabela de jogadas possíveis
    modelBuilder.Entity<Hand>().HasData(
      new Hand { Id = 1, Name = "Pedra" },
      new Hand { Id = 2, Name = "Papel" },
      new Hand { Id = 3, Name = "Tesoura" },
      new Hand { Id = 4, Name = "Spock" },
      new Hand { Id = 5, Name = "Lagarto" }
    );

    // Populando a tabela de relações entre jogadas
    modelBuilder.Entity<HandRelation>().HasData(
      new { WinnerHandId = 1, LoserHandId = 3 }, // Pedra ganha de tesoura
      new { WinnerHandId = 1, LoserHandId = 5 }, // Pedra ganha de lagarto
      new { WinnerHandId = 2, LoserHandId = 1 }, // Papel ganha de pedra
      new { WinnerHandId = 2, LoserHandId = 4 }, // Papel ganha de spock
      new { WinnerHandId = 3, LoserHandId = 2 }, // Tesoura ganha de papel
      new { WinnerHandId = 3, LoserHandId = 5 }, // Tesoura ganha de lagarto
      new { WinnerHandId = 4, LoserHandId = 1 }, // Spock ganha de pedra
      new { WinnerHandId = 4, LoserHandId = 3 }, // Spock ganha de tesoura
      new { WinnerHandId = 5, LoserHandId = 4 }, // Lagarto ganha de spock
      new { WinnerHandId = 5, LoserHandId = 2 }  // Lagarto ganha de papel
    );
  }

}