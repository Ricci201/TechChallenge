using System.ComponentModel.DataAnnotations;

namespace TechChallenge;

public class Professor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do professor é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A especialidade é obrigatória")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "A especialidade deve ter entre 3 e 50 caracteres")]
    public string Especialidade { get; set; }

    [Required(ErrorMessage = "A data de contratação é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime DataContratacao { get; set; }

    public bool Ativo { get; set; } = true;
    public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}