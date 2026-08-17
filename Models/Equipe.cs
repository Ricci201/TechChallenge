using System.ComponentModel.DataAnnotations;

namespace TechChallenge;

public class Equipe
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da equipe é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres")]
    public string Nome { get; set; }

    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "A data de criação é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public bool Ativa { get; set; } = true;
    public ICollection<AlunoEquipe> AlunosEquipes { get; set; } = new List<AlunoEquipe>();
    public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}