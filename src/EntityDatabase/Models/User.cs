using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    [Table("user")]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PaymentCard { get; set; }
        public string RoleId { get; set; }
    }
}