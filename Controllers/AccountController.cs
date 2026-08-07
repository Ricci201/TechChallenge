using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechChallenge;

namespace TechChallenge.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    // =====================================
    // GET: /Account/Register
    // Exibe formulário de cadastro
    // =====================================
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // =====================================
    // POST: /Account/Register
    // Processa cadastro do usuário
    // =====================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError(string.Empty, "Já existe um usuário cadastrado com este email.");
            return View(model);
        }

        if (model.Role != "Aluno" && model.Role != "Professor")
        {
            ModelState.AddModelError(nameof(model.Role), "Selecione uma role válida.");
            return View(model);
        }

        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role), "A role selecionada não está cadastrada no sistema.");
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, model.Senha);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    // =====================================
    // GET: /Account/Login
    // Exibe formulário de login
    // =====================================
    [HttpGet]
    public IActionResult Login(
        string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }



    // =====================================
    // POST: /Account/Login
    // Realiza autenticação
    // =====================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Senha,
            model.Lembrar,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }



    // =====================================
    // POST: /Account/Logout
    // Encerra sessão do usuário
    // =====================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(
            "Index",
            "Home"
        );
    }



    // =====================================
    // GET: /Account/AccessDenied
    // Página sem permissão
    // =====================================
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

}