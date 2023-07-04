using Catalogs.Interfaces;


namespace Catalogs.Models
{
    public class CatalogDataModel : IEntity
    {
        public Guid Id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("Ids of data")]
        public CatalogModel DataOfCatalog { get; set; }
        public Guid IdOfCatalog { get; set; }
    }
}
