using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    [Table("company")]
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
    }
}