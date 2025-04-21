namespace DynamicAppGenerator.Models
{
    public class GeneratorConfig
    {
        public string ApplicationName { get; set; }
        public string OutputPath { get; set; } = "C:\\GeneratedApps2";
       
        public bool IncludeAuthentication { get; set; } = true;
        public bool IncludeAuthorization { get; set; } = true;
    }
}
