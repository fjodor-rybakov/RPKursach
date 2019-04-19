using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    [Table("product")]
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int CompanyId { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
    }
}