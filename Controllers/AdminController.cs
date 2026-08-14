using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TechChallenge.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await _userManager.Users
            .OrderBy(usuario => usuario.Email)
            .ToListAsync();

        var model = new List<UsuarioAdminListaViewModel>();

        foreach (var usuario in usuarios)
        {
            var perfil = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault() ?? "Sem perfil";

            model.Add(new UsuarioAdminListaViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email ?? usuario.UserName ?? "Sem e-mail",
                Perfil = perfil
            });
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> CriarUsuario()
    {
        return View(new UsuarioAdminCriarViewModel
        {
            PerfisDisponiveis = await ObterPerfisAsync()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarUsuario(UsuarioAdminCriarViewModel model)
    {
        if (!await _roleManager.RoleExistsAsync(model.Perfil))
        {
            ModelState.AddModelError(nameof(model.Perfil), "Selecione um perfil válido.");
        }

        if (await _userManager.FindByEmailAsync(model.Email) != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Já existe um usuário com este e-mail.");
        }

        if (!ModelState.IsValid)
        {
            model.PerfisDisponiveis = await ObterPerfisAsync(model.Perfil);
            return View(model);
        }

        var usuario = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var resultadoCriacao = await _userManager.CreateAsync(usuario, model.Senha);
        if (!resultadoCriacao.Succeeded)
        {
            AdicionarErros(resultadoCriacao);
            model.PerfisDisponiveis = await ObterPerfisAsync(model.Perfil);
            return View(model);
        }

        var resultadoPerfil = await _userManager.AddToRoleAsync(usuario, model.Perfil);
        if (!resultadoPerfil.Succeeded)
        {
            await _userManager.DeleteAsync(usuario);
            AdicionarErros(resultadoPerfil);
            model.PerfisDisponiveis = await ObterPerfisAsync(model.Perfil);
            return View(model);
        }

        TempData["Success"] = "Usuário criado com sucesso.";
        return RedirectToAction(nameof(Usuarios));
    }

    [HttpGet]
    public async Task<IActionResult> EditarUsuario(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        var perfil = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault() ?? string.Empty;

        return View(new UsuarioAdminEditarViewModel
        {
            Id = usuario.Id,
            Email = usuario.Email ?? usuario.UserName ?? string.Empty,
            Perfil = perfil,
            PerfisDisponiveis = await ObterPerfisAsync(perfil)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarUsuario(UsuarioAdminEditarViewModel model)
    {
        var usuario = await _userManager.FindByIdAsync(model.Id);
        if (usuario == null)
        {
            return NotFound();
        }

        var usuarioComMesmoEmail = await _userManager.FindByEmailAsync(model.Email);
        if (usuarioComMesmoEmail != null && usuarioComMesmoEmail.Id != usuario.Id)
        {
            ModelState.AddModelError(nameof(model.Email), "Já existe um usuário com este e-mail.");
        }

        if (!await _roleManager.RoleExistsAsync(model.Perfil))
        {
            ModelState.AddModelError(nameof(model.Perfil), "Selecione um perfil válido.");
        }

        if (usuario.Id == _userManager.GetUserId(User) &&
            !string.Equals(model.Perfil, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Perfil), "Você não pode remover o próprio acesso de administrador.");
        }

        if (!ModelState.IsValid)
        {
            model.PerfisDisponiveis = await ObterPerfisAsync(model.Perfil);
            return View(model);
        }

        usuario.UserName = model.Email;
        usuario.Email = model.Email;

        var resultadoAtualizacao = await _userManager.UpdateAsync(usuario);
        if (!resultadoAtualizacao.Succeeded)
        {
            AdicionarErros(resultadoAtualizacao);
            model.PerfisDisponiveis = await ObterPerfisAsync(model.Perfil);
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.NovaSenha))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var resultadoSenha = await _userManager.ResetPasswordAsync(usuario, token, model.NovaSenha);
            if (!resultadoSenha.Succeeded)
            {
                AdicionarErros(resultadoSenha);
                model.PerfisDisponiveis = await ObterPerfisAsync(model.Perfil);
                return View(model);
            }
        }

        var perfisAtuais = await _userManager.GetRolesAsync(usuario);
        var resultadoRemocao = await _userManager.RemoveFromRolesAsync(usuario, perfisAtuais);
        if (!resultadoRemocao.Succeeded)
        {
            AdicionarErros(resultadoRemocao);
            model.PerfisDisponiveis = await ObterPerfisAsync(model.Perfil);
            return View(model);
        }

        var resultadoPerfil = await _userManager.AddToRoleAsync(usuario, model.Perfil);
        if (!resultadoPerfil.Succeeded)
        {
            await _userManager.AddToRolesAsync(usuario, perfisAtuais);
            AdicionarErros(resultadoPerfil);
            model.PerfisDisponiveis = await ObterPerfisAsync(model.Perfil);
            return View(model);
        }

        TempData["Success"] = "Usuário atualizado com sucesso.";
        return RedirectToAction(nameof(Usuarios));
    }

    [HttpGet]
    public async Task<IActionResult> ExcluirUsuario(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        if (usuario.Id == _userManager.GetUserId(User))
        {
            TempData["Error"] = "Você não pode excluir o próprio usuário.";
            return RedirectToAction(nameof(Usuarios));
        }

        var perfil = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault() ?? "Sem perfil";

        return View(new UsuarioAdminExcluirViewModel
        {
            Id = usuario.Id,
            Email = usuario.Email ?? usuario.UserName ?? "Sem e-mail",
            Perfil = perfil
        });
    }

    [HttpPost, ActionName(nameof(ExcluirUsuario))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirUsuarioConfirmado(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
        {
            TempData["Error"] = "Usuário não encontrado.";
            return RedirectToAction(nameof(Usuarios));
        }

        if (usuario.Id == _userManager.GetUserId(User))
        {
            TempData["Error"] = "Você não pode excluir o próprio usuário.";
            return RedirectToAction(nameof(Usuarios));
        }

        var resultado = await _userManager.DeleteAsync(usuario);
        if (!resultado.Succeeded)
        {
            TempData["Error"] = string.Join(" ", resultado.Errors.Select(erro => erro.Description));
            return RedirectToAction(nameof(Usuarios));
        }

        TempData["Success"] = "Usuário removido com sucesso.";
        return RedirectToAction(nameof(Usuarios));
    }

    public async Task<IActionResult> Roles()
    {
        var roles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .ToListAsync();

        return View(roles);
    }

    [HttpGet]
    public IActionResult CriarRole()
    {
        return View(new PerfilAdminViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarRole(PerfilAdminViewModel model)
    {
        if (await _roleManager.RoleExistsAsync(model.Nome))
        {
            ModelState.AddModelError(nameof(model.Nome), "Este perfil já existe.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await _roleManager.CreateAsync(new IdentityRole(model.Nome));
        if (!resultado.Succeeded)
        {
            AdicionarErros(resultado);
            return View(model);
        }

        TempData["Success"] = "Perfil criado com sucesso.";
        return RedirectToAction(nameof(Roles));
    }

    [HttpGet]
    public async Task<IActionResult> EditarRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return View(new PerfilAdminViewModel
        {
            Id = role.Id,
            Nome = role.Name ?? string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarRole(PerfilAdminViewModel model)
    {
        var role = await _roleManager.FindByIdAsync(model.Id ?? string.Empty);
        if (role == null)
        {
            return NotFound();
        }

        var roleComMesmoNome = await _roleManager.FindByNameAsync(model.Nome);
        if (roleComMesmoNome != null && roleComMesmoNome.Id != role.Id)
        {
            ModelState.AddModelError(nameof(model.Nome), "Este perfil já existe.");
        }

        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(model.Nome, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Nome), "O perfil Admin é necessário para acessar este painel e não pode ser renomeado.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        role.Name = model.Nome;
        var resultado = await _roleManager.UpdateAsync(role);
        if (!resultado.Succeeded)
        {
            AdicionarErros(resultado);
            return View(model);
        }

        TempData["Success"] = "Perfil atualizado com sucesso.";
        return RedirectToAction(nameof(Roles));
    }

    [HttpGet]
    public async Task<IActionResult> ExcluirRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        var totalUsuarios = (await _userManager.GetUsersInRoleAsync(role.Name!)).Count;

        return View(new PerfilAdminExcluirViewModel
        {
            Id = role.Id,
            Nome = role.Name ?? string.Empty,
            TotalUsuarios = totalUsuarios
        });
    }

    [HttpPost, ActionName(nameof(ExcluirRole))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirRoleConfirmado(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            TempData["Error"] = "Perfil não encontrado.";
            return RedirectToAction(nameof(Roles));
        }

        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "O perfil Admin é necessário para acessar este painel e não pode ser removido.";
            return RedirectToAction(nameof(Roles));
        }

        if ((await _userManager.GetUsersInRoleAsync(role.Name!)).Count > 0)
        {
            TempData["Error"] = "Não é possível remover um perfil que ainda possui usuários vinculados.";
            return RedirectToAction(nameof(Roles));
        }

        var resultado = await _roleManager.DeleteAsync(role);
        if (!resultado.Succeeded)
        {
            TempData["Error"] = string.Join(" ", resultado.Errors.Select(erro => erro.Description));
            return RedirectToAction(nameof(Roles));
        }

        TempData["Success"] = "Perfil removido com sucesso.";
        return RedirectToAction(nameof(Roles));
    }

    private async Task<List<SelectListItem>> ObterPerfisAsync(string? perfilSelecionado = null)
    {
        var roles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .ToListAsync();

        return roles.Select(role => new SelectListItem
        {
            Value = role.Name,
            Text = role.Name,
            Selected = role.Name == perfilSelecionado
        }).ToList();
    }

    private void AdicionarErros(IdentityResult resultado)
    {
        foreach (var erro in resultado.Errors)
        {
            ModelState.AddModelError(string.Empty, erro.Description);
        }
    }
}
