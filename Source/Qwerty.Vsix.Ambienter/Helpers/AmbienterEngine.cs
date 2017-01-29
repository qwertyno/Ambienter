using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Web.Administration;
using Qwerty.Vsix.Ambienter.Dialogs;
using Process = System.Diagnostics.Process;

namespace Qwerty.Vsix.Ambienter.Helpers
{
    public class AmbienterEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageBoxes _messageBoxes;

        public AmbienterEngine(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _messageBoxes = new MessageBoxes(serviceProvider);
        }

        public bool IsAdministrativeMode()
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            var administrativeMode = principal.IsInRole(WindowsBuiltInRole.Administrator);

            return administrativeMode;
        }

        public ProjectInfo GetProjectInfo()
        {
            var dte = _serviceProvider.GetService(typeof(SDTE)) as DTE;
            if (dte == null) return new ProjectInfo { Success = false, Message = "Missing DTE" };

            if (dte.SelectedItems.Count != 1)
            {
                return new ProjectInfo { Success = false, Message = "Only one project may be selected!" };
            }

            //GetObjectType();

            foreach (SelectedItem selectedItem in dte.SelectedItems)
            {
                var solutionName = Path.GetFileNameWithoutExtension(dte.Solution.FullName);
                var selectedItemName = selectedItem.Name;

                var selectedProjectItem = selectedItem.ProjectItem;
                if (selectedProjectItem != null)
                {
                    var errorMessage = $"This is not a project: '{selectedItemName}' in [{solutionName}].";
                    return new ProjectInfo { Success = false, Message = errorMessage };
                }

                var selectedProject = selectedItem.Project;
                if (selectedProject == null)
                {
                    var errorMessage = $"This is not a project: '{selectedItemName}' in [{solutionName}].";
                    return new ProjectInfo { Success = false, Message = errorMessage };
                }

                //var selectedKind = selectedProject.Kind;

                var fullPathProperty = selectedProject.Properties.Item("FullPath");
                if (fullPathProperty == null)
                {
                    var errorMessage = $"No path property: '{selectedItemName}' in [{solutionName}].";
                    return new ProjectInfo { Success = false, Message = errorMessage };
                }

                var fullPath = fullPathProperty.Value.ToString();
                //var message =
                //    $"Project path of '{selectedItemName}'"
                //    + $"{Environment.NewLine}({selectedKind}) in [{solutionName}]:"
                //    + $"{Environment.NewLine}'{fullPath}'. "
                //    + $"{Environment.NewLine}DefaultTLD = {AmbienterPackage.Options.DefaultTldName}"
                //    + $"{Environment.NewLine}UpdateHostFile = {AmbienterPackage.Options.UpdateHostsFile}";
                //ShowMessage(message);
                return new ProjectInfo
                {
                    Success = true,
                    ProjectName = selectedItemName,
                    ProjectPath = fullPath
                };
            }
            return new ProjectInfo { Success = false, Message = "Unkown error or no item selected" };
        }

        public bool CreateWebSite(string siteName, string sitePath)
        {
            var iisManager = new ServerManager();
            iisManager.Sites.Add(siteName, "http", $"*:80:{siteName.ToLower()}", sitePath);
            //TODO: Create application pool
            iisManager.CommitChanges();
            return true;
        }

        public bool UpdateHostsFile(string projectName, string siteName)
        {
            var hostFilePathAndName =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts");

            if (!File.Exists(hostFilePathAndName))
            {
                var errorMessage = $"Could not find {hostFilePathAndName}";
                Trace.WriteLine(errorMessage);
                _messageBoxes.ShowErrorMessage(errorMessage);
                return false;
            }

            try
            {
                using (var w = File.AppendText(hostFilePathAndName))
                {
                    w.WriteLine(string.Empty);
                    w.WriteLine($"# {projectName}");
                    w.WriteLine($"\t127.0.0.1\t{siteName.ToLower()}");
                    //w.WriteLine($"\t::1\t\t\t{siteName.ToLower()}");
                }
            }
            catch (Exception e)
            {
                var errorMessage = $"Could not update .hosts file: {e}";
                Trace.WriteLine(errorMessage);
                _messageBoxes.ShowErrorMessage(errorMessage);
                return false;
            }

            //ShowMessage($"Updated .hosts file with {siteName}");
            return true;
        }

        public void OpenInBrowser(string siteName)
        {
            Process.Start($"http://{siteName}");
        }

    }
}
