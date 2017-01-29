using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Qwerty.Vsix.Ambienter.Dialogs
{
    public class MessageBoxes
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _title = $"{Vsix.Name} version {Vsix.Version}";

        public MessageBoxes(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static int OkButton = 1;
        public static int CancelButton = 2;

        public int ShowAcceptWarningMessage(string message)
        {
            return VsShellUtilities.ShowMessageBox(
                _serviceProvider,
                message,
                _title,
                OLEMSGICON.OLEMSGICON_WARNING,
                OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public int ShowInfoMessage(string message)
        {
            return VsShellUtilities.ShowMessageBox(
                _serviceProvider,
                message,
                _title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public int ShowErrorMessage(string message)
        {
            return VsShellUtilities.ShowMessageBox(
                _serviceProvider,
                message,
                _title,
                OLEMSGICON.OLEMSGICON_CRITICAL,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

    }
}
