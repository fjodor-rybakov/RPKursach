using System.Collections.Generic;

namespace EntityDatabase.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PaymentCard { get; set; }
        
        public List<UserProduct> UserProducts { get; set; }
        public Role Role { get; set; }
    }
}