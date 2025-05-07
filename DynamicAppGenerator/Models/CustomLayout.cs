using System.ComponentModel.DataAnnotations;

namespace DynamicAppGenerator.Models
{
    public class CustomLayout
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public string? HtmlContent { get; set; }

        public string? CssContent { get; set; }

        public string? JsContent { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
