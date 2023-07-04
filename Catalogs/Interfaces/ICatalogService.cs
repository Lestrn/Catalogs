using Catalogs.Models;

namespace Catalogs.Interfaces
{
    public interface ICatalogService
    {
        public Task<CatalogeDTO?> GetCatalogDTOFromRoute(string route);
        public Task<bool> AddCatalog(string currentRoute, string catalogName);
        public Task<bool> FillWithDefaultCatalogs();
        public Task<List<CatalogModel>> GetCatalogModels();
    }
}
