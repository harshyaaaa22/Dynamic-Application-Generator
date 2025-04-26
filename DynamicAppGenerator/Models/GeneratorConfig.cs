using System.ComponentModel.DataAnnotations;

namespace DynamicAppGenerator.Models
{
    public class GeneratorConfig
    {
        public string ApplicationName { get; set; }
        public string OutputPath { get; set; } = "C:\\GeneratedApps2";


        [Required]
        [Display(Name = "Select Layout")]
        public string SelectedLayout { get; set; }

        public bool IncludeAuthentication { get; set; } = true;
        public bool IncludeAuthorization { get; set; } = true;
    }
}
