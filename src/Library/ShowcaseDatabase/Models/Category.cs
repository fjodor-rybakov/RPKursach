using System.Collections.Generic;

namespace ShowcaseDatabase.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        
        public List<Product> Product { get; set; }
    }
}