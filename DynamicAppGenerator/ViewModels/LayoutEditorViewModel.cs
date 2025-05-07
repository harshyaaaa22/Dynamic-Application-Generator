using System.ComponentModel.DataAnnotations;

namespace DynamicAppGenerator.ViewModels
{
    public class LayoutEditorViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Layout Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "HTML Content")]
        public string HtmlContent { get; set; }

        [Display(Name = "CSS Content")]
        public string CssContent { get; set; }

        [Display(Name = "JavaScript Content")]
        public string JsContent { get; set; }

        public bool IsNew => string.IsNullOrEmpty(Id);
    }
}
