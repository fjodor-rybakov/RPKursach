using System.Collections.Generic;

namespace EntityDatabase.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        
        public List<Product> Product { get; set; }
    }
}