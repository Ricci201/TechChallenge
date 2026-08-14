namespace TechChallenge;

public class Aluno
{
    public int Id { get; set; } //Primary Key, a chave primária da tabela com seu identificador único
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateTime DataNascimento { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
}
