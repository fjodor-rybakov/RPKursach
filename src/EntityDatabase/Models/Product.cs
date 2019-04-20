using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    [Table("product")]
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        
        public Company Company { get; set; }
        public Category Category { get; set; }
        
        public List<Basket> Baskets { get; set; }
    }
}