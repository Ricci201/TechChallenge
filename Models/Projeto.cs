using System.ComponentModel.DataAnnotations;

namespace TechChallenge;

public class Projeto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do projeto é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; }

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "A data de início é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime DataInicio { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DataFim { get; set; }

    [Range(0, 100, ErrorMessage = "A pontuação deve estar entre 0 e 100")]
    public int Pontuacao { get; set; }

    public int ProfessorId { get; set; }
    public int CategoriaId { get; set; }
    public int EquipeId { get; set; }
    public Professor? Professor { get; set; }
    public Categoria? Categoria { get; set; }
    public Equipe? Equipe { get; set; }
}
