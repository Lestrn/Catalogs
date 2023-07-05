using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Catalogs.Models;
using Catalogs.Interfaces;
using Microsoft.IdentityModel.Tokens;

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
        CatalogDTO? catalogModel = await _catalogService.GetCatalogDTOFromRoute(route);
        if (catalogModel == null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(catalogModel);
    }

    public async Task<IActionResult> FillWithDefaultCatalogs()
    {
        await _catalogService.FillWithDefaultCatalogs();
        return RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> ImportCatalog(IFormFile zipFile)
    {
        try
        {
            await _catalogService.ImportCatalog(zipFile);
        }
        catch
        {
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Index", "Home");

    }
    public async Task<IActionResult> DownloadCatalog(string catalogName)
    {
        if(catalogName.IsNullOrEmpty())
        {
            return RedirectToAction("Index", "Home");
        }
        return File(await _catalogService.DownloadCatalog(catalogName), "application/zip", $"{catalogName}.zip");
    }
}
