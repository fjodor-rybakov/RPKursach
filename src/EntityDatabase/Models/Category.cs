using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        
        public List<Product> Products { get; set; }
    }
}