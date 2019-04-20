using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    [Table("basket")]
    public class Basket
    {
        public int Id { get; set; }
        
        public Product Product { get; set; }
        
        public List<User> Users { get; set; }
    }
}