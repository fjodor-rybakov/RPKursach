using System.Collections.Generic;

namespace EntityDatabase.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        
        public List<User> Users { get; set; }
    }
}