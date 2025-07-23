using System.ComponentModel.DataAnnotations;

namespace T1694183.Shared.Data
{
    public class Product
    {
        public bool Discontinued { get; set; }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public decimal? Price { get; set; }

        public int Quantity { get; set; }

        [Required]
        public DateOnly? ReleaseDateOnly { get; set; }

        [Required]
        public DateTime? ReleaseDateTime { get; set; }
    }
}
