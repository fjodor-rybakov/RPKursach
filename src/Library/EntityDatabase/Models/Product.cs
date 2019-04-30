using System.Collections.Generic;

namespace EntityDatabase.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
        public List<UserProduct> UserProducts { get; set; }
    }
}