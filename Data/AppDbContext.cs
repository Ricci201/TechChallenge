using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechChallenge.Models;

namespace TechChallenge;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    // Construtor do Contexto do EF Core
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /*
        DbSet -> Representação das tabelas no banco de dados.

        Cada Model que deve virar uma tabela precisa
        ser declarado aqui.
    */

    public DbSet<Aluno> Alunos { get; set; }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Professor> Professores { get; set; }

    public DbSet<Equipe> Equipes { get; set; }

    public DbSet<AlunoEquipe> AlunosEquipes { get; set; }

    public DbSet<Projeto> Projetos { get; set; }


    // Configurações das entidades
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Chave composta da tabela de relacionamento AlunoEquipe
        modelBuilder.Entity<AlunoEquipe>()
            .HasKey(ae => new 
            { 
                ae.AlunoId, 
                ae.EquipeId 
            });
    }
}