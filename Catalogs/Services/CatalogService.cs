using Catalogs.Interfaces;
using Catalogs.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

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
        public Task<CatalogeDTO> GetCatalogDTOFromRoute(string route)
        {
            var catalogWithData = _catalogRepository.Context.Catalog.Where(ctg => ctg.CatalogRoute == route).Join(_catalogRepository.Context.Data, ctg => ctg.Id, data => data.IdOfCatalog, (ctg, data) => new
            {
                CatalogeId = ctg.Id,
                CatalogeName = ctg.CatalogName,
                CatalogeRoute = ctg.CatalogRoute,
                DataName = data.DataOfCatalog.CatalogName,
                DataRoute = data.DataOfCatalog.CatalogRoute
            });
            string ctgRoute = "", ctgName = "";
            Guid id = Guid.Empty;
            foreach (var data in catalogWithData)
            {
                id = data.CatalogeId;
                ctgRoute = data.CatalogeRoute;
                ctgName = data.CatalogeName;
                break;
            }
            List<string> ctgDataName = new List<string>();
            List<string> ctgDataRoute = new List<string>();
            foreach (var data in catalogWithData)
            {
                ctgDataName.Add(data.DataName);
                ctgDataRoute.Add(data.DataRoute);
            }
            CatalogeDTO catalogDTO = new CatalogeDTO() {Id = id, CatalogeName = ctgName, CatalogeRoute = ctgRoute, DataName = ctgDataName, DataRoute = ctgDataRoute };
            return Task.FromResult(catalogDTO);
        }
        public async Task<bool> AddCatalog(string currentRoute, string catalogName)
        {
            var catalog = await GetCatalogDTOFromRoute(currentRoute);
            #region Check if there is already catalog with the same name, if so dont add and return false
            var catalogWithData = _catalogRepository.Context.Catalog.Where(ctg => ctg.CatalogRoute == currentRoute).Join(_catalogRepository.Context.Data, ctg => ctg.Id, data => data.IdOfCatalog, (ctg, data) => new
            {
                DataName = data.DataOfCatalog.CatalogName
            });

            List<string> routNamesInCatalog = new List<string>();
            foreach (var data in catalogWithData)
            {
                routNamesInCatalog.Add(data.DataName);
            }

            if(routNamesInCatalog.Contains(catalogName))
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
            if(resources == null || graphicProducts == null)
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
    }
}
