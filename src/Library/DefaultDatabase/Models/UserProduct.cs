namespace DefaultDatabase.Models
{
    public class UserProduct
    {
        public int Id { get; set; }
        public int ProductCount { get; set; }
        
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}