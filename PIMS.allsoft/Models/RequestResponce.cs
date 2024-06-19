namespace PIMS.allsoft.Models
{
    public class RequestResponce
    {
    }
    public class Categories
    {

        public int CategoryID { get; set; }
        public string Name { get; set; }

    }
    public class Products
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SKU { get; set; }        
        public List<int> CategoryIds { get; set; } = new List<int>();
    }

}
