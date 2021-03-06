﻿using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Qwerty.Vsix.Ambienter.Dialogs;
using Qwerty.Vsix.Ambienter.Helpers;

namespace Qwerty.Vsix.Ambienter.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CreateAmbients
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAmbients"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CreateAmbients(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandId = new CommandID(PackageGuids.guidCreateAmbientsCmdSet, CommandId);
                var menuItem = new MenuCommand(CreateAmbientsForProject, menuCommandId);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CreateAmbients Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CreateAmbients(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void CreateAmbientsForProject(object sender, EventArgs e)
        {
            var engine = new AmbienterEngine(ServiceProvider);
            var messageBoxes = new MessageBoxes(ServiceProvider);

            //Check if VS is running in administrative mode
            var isAdministrativeMode = engine.IsAdministrativeMode();
            if (!isAdministrativeMode)
            {
                messageBoxes.ShowErrorMessage("This extention needs Administrative Mode to run!");
                return;
            }

            if (!AmbienterPackage.Options.SupressWarning)
            {
                var userResponse = messageBoxes.ShowAcceptWarningMessage(AmbienterConstants.Warning);
                if (userResponse != MessageBoxes.OkButton) return;
            }

            //TODO: Check that this project is a Web project or Web site
            //Get Project Info
            var projectInfo = engine.GetProjectInfo();
            if (!projectInfo.Success)
            {
                messageBoxes.ShowErrorMessage(projectInfo.Message);
                return;
            }

            var projectName = projectInfo.ProjectName;
            var projectPath = projectInfo.ProjectPath;
            var siteName = $"{projectName}{AmbienterPackage.Options.DefaultTldName}";

            //TODO: Let user change options
            //Temporarily display option settings
            if (!AmbienterPackage.Options.SupressInfo)
            {
                var infoMessage =
                    $"Default Options"
                    + $"\r\nDefaultTldName  :{AmbienterPackage.Options.DefaultTldName}"
                    + $"\r\nUpdateHostsFile :{AmbienterPackage.Options.UpdateHostsFile}"
                    + $"\r\nOpenInBrowser   :{AmbienterPackage.Options.OpenInBrowser}"
                    + $"\r\n\r\nThis will create site '{siteName}' in IIS"
                    + $"\r\n\r\nDo you want to continnue?";
                var userResponse = messageBoxes.ShowAcceptWarningMessage(infoMessage);
                if (userResponse != MessageBoxes.OkButton) return;
            }
            

            //TODO: Check if IIS site already exists
            //Create IIS site
            engine.CreateWebSite(siteName, projectPath);

            //TODO: Check if sitename already exists in hosts file
            //Add to hosts file
            if (AmbienterPackage.Options.UpdateHostsFile)
            {
                engine.UpdateHostsFile(projectName, siteName);
            }

            //TODO: Add selection for which browser to open
            //Open in default browser
            if (AmbienterPackage.Options.OpenInBrowser)
            {
                engine.OpenInBrowser(siteName);
            }
        }
    }
}
