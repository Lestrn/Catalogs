using Catalogs.Interfaces;
using Catalogs.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace Catalogs.Services
{
    public class CatalogService : ICatalogService
    {
        private IEntityRepository<CatalogModel> _catalogRepository;
        private IEntityRepository<CatalogDataModel> _dataRepository;
        public CatalogService(IEntityRepository<CatalogModel> entityRepository, IEntityRepository<CatalogDataModel> dataRepository)
        {
            _catalogRepository = entityRepository;
            _dataRepository = dataRepository;
        }
        public async Task<CatalogDTO?> GetCatalogDTOFromRoute(string route)
        {
            var catalogWithData = _catalogRepository.Context.Catalog.Where(ctg => ctg.CatalogRoute == route).Join(_catalogRepository.Context.Data, ctg => ctg.Id, data => data.IdOfCatalog, (ctg, data) => new
            {
                CatalogId = ctg.Id,
                CatalogName = ctg.CatalogName,
                CatalogRoute = ctg.CatalogRoute,
                DataName = data.DataOfCatalog.CatalogName,
                DataRoute = data.DataOfCatalog.CatalogRoute
            });
            List<string> ctgDataName = new List<string>();
            List<string> ctgDataRoute = new List<string>();
            foreach (var data in catalogWithData)
            {
                ctgDataName.Add(data.DataName);
                ctgDataRoute.Add(data.DataRoute);
            }
            var catalogeWithoutData = (await _catalogRepository.Where(ctg => ctg.CatalogRoute == route)).FirstOrDefault();
            if (catalogeWithoutData == null)
            {
                return null;
            }
            CatalogDTO catalogDTO = new CatalogDTO() { Id = catalogeWithoutData.Id, CatalogName = catalogeWithoutData.CatalogName, CatalogRoute = catalogeWithoutData.CatalogRoute, DataName = ctgDataName, DataRoute = ctgDataRoute };
            return catalogDTO;
        }
        public async Task<bool> AddCatalog(string currentRoute, string catalogName)
        {
            #region Check if there is already catalog with the same name, if so dont add and return false
            var catalog = await GetCatalogDTOFromRoute(currentRoute);

            List<string> routNamesInCatalog = catalog.DataName;
            if (routNamesInCatalog.Contains(catalogName))
            {
                return false;
            }

            #endregion

            CatalogModel newCatalog = new CatalogModel { CatalogName = catalogName, CatalogRoute = $"{currentRoute}\\{catalogName}" };
            CatalogDataModel newData = new CatalogDataModel { DataOfCatalog = newCatalog, IdOfCatalog = catalog.Id };
            await _catalogRepository.AddAsync(newCatalog);
            await _dataRepository.AddAsync(newData);
            await _catalogRepository.SaveChangesAsync();
            return true;
        }
        public async Task<bool> FillWithDefaultCatalogs()
        {
            await AddCatalog("", "Creating Digital Images");
            await _catalogRepository.SaveChangesAsync();
            CatalogModel? creatingDigitalImages = _catalogRepository.Context.Catalog.FirstOrDefault(ctg => ctg.CatalogName == "Creating Digital Images");
            if (creatingDigitalImages == null)
            {
                return false;
            }

            await AddCatalog(creatingDigitalImages.CatalogRoute, "Resources");
            await AddCatalog(creatingDigitalImages.CatalogRoute, "Evidence");
            await AddCatalog(creatingDigitalImages.CatalogRoute, "Graphic Products");
            await _catalogRepository.SaveChangesAsync();
            CatalogModel? resources = _catalogRepository.Context.Catalog.FirstOrDefault(ctg => ctg.CatalogName == "Resources");
            CatalogModel? graphicProducts = _catalogRepository.Context.Catalog.FirstOrDefault(ctg => ctg.CatalogName == "Graphic Products");
            if (resources == null || graphicProducts == null)
            {
                return false;
            }

            await AddCatalog(resources.CatalogRoute, "Primary Sources");
            await AddCatalog(resources.CatalogRoute, "Secondary Sources");
            await AddCatalog(graphicProducts.CatalogRoute, "Process");
            await AddCatalog(graphicProducts.CatalogRoute, "Final Product");
            await _catalogRepository.SaveChangesAsync();
            return true;
        }
        public async Task<List<CatalogModel>> GetCatalogModels()
        {
            return await _catalogRepository.GetAllAsync();
        }
        public async Task ImportCatalog(IFormFile zipFile)
        {
            if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\Uploads"))
            {
                Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\Uploads");
            }
            if (zipFile != null && zipFile.Length > 0)
            {
                // Generate a unique filename for the uploaded .zip file
                var fileName = zipFile.FileName;

                // Save the .zip file to a desired location
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await zipFile.CopyToAsync(stream);
                }

                // Extract the contents of the .zip file
                var extractionPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                ZipFile.ExtractToDirectory(filePath, extractionPath);
                fileName = fileName[..^4];
                string catalogPath = $"{extractionPath}\\{fileName}";
                // Get the routes of all extracted files
                var routes = Directory.GetDirectories(catalogPath, "*", SearchOption.AllDirectories);
                await AddCatalog("", fileName);
                foreach (var fileRoute in routes)
                {
                    string routeForDb = fileRoute.Substring(extractionPath.Length);
                    await AddCatalog(Directory.GetParent(routeForDb).FullName.Substring(2), Path.GetFileName(routeForDb));
                }
                // Delete the .zip file and extracted folder
                System.IO.File.Delete(filePath);
                Directory.Delete(extractionPath, true);
            }
        }
    }
}
