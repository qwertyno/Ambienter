using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace Qwerty.Vsix.Ambienter.Dialogs
{
    public class AmbienterOptions : DialogPage
    {
        [Category("General")]
        [DisplayName("Supress warning message")]
        [Description("Supresses the warning message when using the extion.")]
        [DefaultValue(false)]
        public bool SupressWarning { get; set; } = false;

        [Category("General")]
        [DisplayName("Supress info message")]
        [Description("Supresses the info message showing settings.")]
        [DefaultValue(false)]
        public bool SupressInfo { get; set; } = false;

        [Category("General")]
        [DisplayName("Default TLD name")]
        [Description("Default Top Level Domain (TLD) name. Suggested value is .local")]
        [DefaultValue(".local")]
        //TODO: Make sure the tld name is in correct format
        public string DefaultTldName { get; set; } = ".local";


        [Category("General")]
        [DisplayName("Update hosts file")]
        [Description("Update hosts file with the newly created site binding")]
        [DefaultValue(true)]
        public bool UpdateHostsFile { get; set; } = true;

        [Category("General")]
        [DisplayName("Open in default browser")]
        [Description("Open the new site in the default browser.")]
        [DefaultValue(true)]
        public bool OpenInBrowser { get; set; } = true;
    }
}
