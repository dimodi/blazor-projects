namespace TelerikBlazorEF.Data
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public DateTime ReleaseDate { get; set; }

        public bool Discontinued { get; set; }
    }
}
