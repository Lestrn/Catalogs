namespace Catalogs.Models
{
    public class CatalogeDTO
    {
        public Guid Id { get; set; }
        public string CatalogeName { get; set; }
        public string CatalogeRoute { get; set; }
        public List<string> DataName { get; set; }
        public List<string> DataRoute { get; set; }
    }
}
