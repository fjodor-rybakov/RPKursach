using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.Models
{
    [Table("role")]
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        
        public List<User> Users { get; set; }
    }
}