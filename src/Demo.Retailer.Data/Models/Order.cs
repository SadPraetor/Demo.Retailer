namespace Demo.Retailer.Data
{
    public record Order(int UserId)
    {
        public int Id { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public List<LineItem>? LineItems {get; set;}
    }
}
