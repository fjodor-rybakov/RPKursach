using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        
        public List<Product> Product { get; set; }
    }
}