using System.ComponentModel.DataAnnotations;

namespace TelerikBlazorEF.Data
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int? CategoryId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [Required]
        public DateTime? ReleaseDate { get; set; }

        public bool Discontinued { get; set; }
    }
}
