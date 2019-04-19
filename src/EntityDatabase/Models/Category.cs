using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    [Table("category")]
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }
}