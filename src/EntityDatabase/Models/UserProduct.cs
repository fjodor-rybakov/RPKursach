namespace EntityDatabase.Models
{
    public class UserProduct
    {
        public int Id { get; set; }
        
        public Product Product { get; set; }
        
        public User User { get; set; }
    }
}