using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechChallenge;

public class UsuarioAdminListaViewModel
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Perfil { get; init; } = "Sem perfil";
}

public class UsuarioAdminCriarViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "A senha deve ter ao menos {2} caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Senha), ErrorMessage = "As senhas não coincidem.")]
    [Display(Name = "Confirmar senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o perfil.")]
    [Display(Name = "Perfil")]
    public string Perfil { get; set; } = string.Empty;

    public List<SelectListItem> PerfisDisponiveis { get; set; } = [];
}

public class UsuarioAdminEditarViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "A nova senha deve ter ao menos {2} caracteres.")]
    [Display(Name = "Nova senha")]
    public string? NovaSenha { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não coincidem.")]
    [Display(Name = "Confirmar nova senha")]
    public string? ConfirmarNovaSenha { get; set; }

    [Required(ErrorMessage = "Selecione o perfil.")]
    [Display(Name = "Perfil")]
    public string Perfil { get; set; } = string.Empty;

    public List<SelectListItem> PerfisDisponiveis { get; set; } = [];
}

public class UsuarioAdminExcluirViewModel
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Perfil { get; init; } = "Sem perfil";
}

public class PerfilAdminViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do perfil.")]
    [StringLength(256, ErrorMessage = "O nome do perfil pode ter no máximo {1} caracteres.")]
    [Display(Name = "Nome do perfil")]
    public string Nome { get; set; } = string.Empty;
}

public class PerfilAdminExcluirViewModel
{
    public string Id { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public int TotalUsuarios { get; init; }
}
