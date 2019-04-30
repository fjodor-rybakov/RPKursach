namespace EntityDatabase.Models
{
    public class PurchaseHistory
    {
        public int Id { get; set; }
        public int ProductCount { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}