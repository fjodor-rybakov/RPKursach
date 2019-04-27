using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string PaymentCard { get; set; }
        
        public List<UserProduct> UserProducts { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}