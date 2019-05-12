namespace ShowcaseDatabase.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public string ImagePath { get; set; }
        
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}