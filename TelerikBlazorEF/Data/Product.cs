using System.ComponentModel.DataAnnotations;

namespace TelerikBlazorEF.Data
{
    public class Product
    {
        public Category Category { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public bool Discontinued { get; set; }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public decimal? Price { get; set; }

        public int Quantity { get; set; }

        [Required]
        public DateTime? ReleaseDate { get; set; }
    }
}
