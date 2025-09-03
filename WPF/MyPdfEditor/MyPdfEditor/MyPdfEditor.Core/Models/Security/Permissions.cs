namespace MyPdfEditor.Core.Models.Security
{
    public class Permissions
    {
        public bool CanPrint { get; set; } = true;
        public bool CanModify { get; set; } = true;
        public bool CanCopy { get; set; } = true;
        public bool CanAddAnnotations { get; set; } = true;
        public bool CanFillForms { get; set; } = true;
        public bool CanExtractContent { get; set; } = true;
        public bool CanAssemble { get; set; } = true;
        public bool CanPrintHighQuality { get; set; } = true;

        public static Permissions FullPermissions => new Permissions
        {
            CanPrint = true,
            CanModify = true,
            CanCopy = true,
            CanAddAnnotations = true,
            CanFillForms = true,
            CanExtractContent = true,
            CanAssemble = true,
            CanPrintHighQuality = true
        };

        public static Permissions ReadOnly => new Permissions
        {
            CanPrint = true,
            CanModify = false,
            CanCopy = true,
            CanAddAnnotations = false,
            CanFillForms = false,
            CanExtractContent = true,
            CanAssemble = false,
            CanPrintHighQuality = true
        };

        public static Permissions NoPermissions => new Permissions
        {
            CanPrint = false,
            CanModify = false,
            CanCopy = false,
            CanAddAnnotations = false,
            CanFillForms = false,
            CanExtractContent = false,
            CanAssemble = false,
            CanPrintHighQuality = false
        };
    }
}