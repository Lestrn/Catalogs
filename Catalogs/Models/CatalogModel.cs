using Catalogs.Interfaces;

namespace Catalogs.Models
{
    public class CatalogModel : IEntity
    {
        public Guid Id { get; set; }
        public string CatalogName { get; set; }
        public string CatalogRoute { get; set; }
        public List<CatalogDataModel> Data { get; set; }    
    }
}
