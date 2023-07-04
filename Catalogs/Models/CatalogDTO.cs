namespace Catalogs.Models
{
    public class CatalogeDTO
    {
        public Guid Id { get; set; }
        public string CatalogName { get; set; }
        public string CatalogRoute { get; set; }
        public List<string> DataName { get; set; }
        public List<string> DataRoute { get; set; }
    }
}
