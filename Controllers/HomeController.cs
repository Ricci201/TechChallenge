using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Models;

namespace TechChallenge.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
    
        return View();
    }
    public IActionResult About()
    {
        //imagine que os dados abaixo vieram de um banco de dados
            // viewdata e viewbag serve para transportar os dados do controller para o view
        
        ViewData["Nome"] = "Rayane";
        ViewData["Idade"] = 17;

        ViewBag.Cidade = "Jaú";
        ViewBag.UF = "SP";
        
        return View();

    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
