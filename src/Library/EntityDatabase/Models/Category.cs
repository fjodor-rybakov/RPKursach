using System.Collections.Generic;

namespace EntityDatabase.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        
        public List<Product> Products { get; set; }
    }
}