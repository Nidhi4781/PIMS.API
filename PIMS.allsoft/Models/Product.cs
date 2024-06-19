namespace PIMS.allsoft.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SKU { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
       // public ICollection<ProductCategory> ProductCategories { get; set; }
        //public ICollection<Inventory> Inventories { get; set; }
    }
    public class PriceAdjustment
    {
        public List<int> ProductIds { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public bool IsPercentage { get; set; }
    }

}
