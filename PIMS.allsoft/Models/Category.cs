namespace PIMS.allsoft.Models
{
    public class Category
    {

        public int CategoryID { get; set; }
        public string Name { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        //public ICollection<ProductCategory>? ProductCategories { get; set; }
    }
}
