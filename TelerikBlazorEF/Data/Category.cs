using System.ComponentModel.DataAnnotations;

namespace TelerikBlazorEF.Data
{
	public class Category
	{
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
