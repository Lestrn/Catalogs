using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Catalogs.Models;
using Catalogs.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Catalogs.Enums;

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
            await Notification("There was an error, you might have used corrupted file or didnt use .zip", NotificationTypes.error);
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
        if(zipFile == null)
        {
            await Notification("The file wasnt chosen!", NotificationTypes.error);
            return RedirectToAction("Index", "Home");
        }
        try
        {
            await _catalogService.ImportCatalog(zipFile);
        }
        catch
        {
            await Notification("There was an error, you might have used corrupted file or didnt use .zip", NotificationTypes.error);
        }
        return RedirectToAction("Index", "Home");

    }
    public async Task<IActionResult> DownloadCatalog(string catalogRoute)
    {
        if (catalogRoute.IsNullOrEmpty())
        {
            await Notification("You have to be in catalog to download it", NotificationTypes.error);
            return RedirectToAction("Index", "Home");
        }
        string catalogName = catalogRoute.Split('\\')[1];
        return File(await _catalogService.DownloadCatalog(catalogName), "application/zip", $"{catalogName}.zip");
    }
    public Task Notification(string message, NotificationTypes notificationType)
    {
        TempData["NotificationMessage"] = message;
        TempData["NotificationType"] = notificationType.ToString();
        return Task.CompletedTask;
    }

}
