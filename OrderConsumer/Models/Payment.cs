namespace OrderConsumer.Models
{
    public class Payment
    {        
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
