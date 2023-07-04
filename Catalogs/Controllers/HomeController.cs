using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Catalogs.Models;
using Catalogs.Interfaces;

namespace Catalogs.Controllers;

public class HomeController : Controller
{
    private ICatalogService _catalogService;

    public HomeController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index(string route = "")
    {
        CatalogDTO catalogModel = await _catalogService.GetCatalogDTOFromRoute(route);
        return View(catalogModel);
    }
   
    public async Task<IActionResult> FillWithDefaultCatalogs()
    {
        await _catalogService.FillWithDefaultCatalogs();
        return RedirectToAction("Index", "Home");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
