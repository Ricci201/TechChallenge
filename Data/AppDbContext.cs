using Microsoft.EntityFrameworkCore;
using TechChallenge.Models;

namespace TechChallenge;

public class AppDbContext : DbContext
{
    //Herança no Método Construtor 
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    // DbSet -> Representação da Tabela no Sistema
    // Para cada model do sistema que for virar uma tabela no Banco de Dados, deverá ser adicionado um Db
    public DbSet<Aluno> Alunos {get; set;}
}
