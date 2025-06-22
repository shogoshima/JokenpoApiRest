using JokenpoApiRest.Data;
using JokenpoApiRest.Services;
using JokenpoApiRest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ler conexão do env
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Criando o dbcontext para conectar ao postgres
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Adicionando serviços
// Cada instância será criada uma vez por requisição HTTP
builder.Services.AddScoped<IParticipationService, ParticipationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoundService, RoundService>();
builder.Services.AddScoped<IHandService, HandService>();
builder.Services.AddScoped<IHandRelationService, HandRelationService>();
builder.Services.AddScoped<IRoundFinalizerService, RoundFinalizerService>();

// Registra supoerte a controllers com atributos (essencial para webapi)
builder.Services.AddControllers();

// Documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurando swaggers
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapeando os controllers criados
app.MapControllers();

app.Run();
