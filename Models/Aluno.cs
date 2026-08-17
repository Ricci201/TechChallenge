using System.ComponentModel.DataAnnotations;

namespace TechChallenge;

public class Aluno
{
    [Required(ErrorMessage = "O campo ID é obrigatório")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do aluno é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }

    [Required(ErrorMessage = "A data de nascimento é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime DataNascimento { get; set; }

    [Required(ErrorMessage = "A data de cadastro é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime DataCadastro { get; set; }

    public bool Ativo { get; set; }
}
