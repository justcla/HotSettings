//------------------------------------------------------------------------------
// <copyright file="ToggleMargin.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace HotSettings
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ToggleMargin
    {
        /// <summary>
        /// Command IDs. Note: These must be in sync with symbols in VSCT file.
        /// </summary>
        //public const int ToggleIndicatorMarginCommandId = 0x1021;  // Comes from VSCT file - Symbols
        //public const int ToggleSelectionMarginCommandId = 0x1022;
        public const int ToggleIndicatorMarginCmdId = 0x1021;
        public const int ToggleLineNumbersCmdId = 0x1022;
        public const int ToggleQuickActionsCmdId = 0x1023;
        public const int ToggleSelectionMarginCmdId = 0x1024;
        public const int ToggleTrackChangesCmdId = 0x1025;
        public const int ToggleDiffMarginCmdId = 0x1026;
        public const int ToggleOutliningCmdId = 0x1027;
        public const int ToggleLiveUnitTestingCmdId = 0x1028;
        public const int ToggleAnnotateCmdId = 0x1029;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("c75f116c-9249-4984-8d82-d3c6025afb17");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleMargin"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ToggleMargin(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                commandService.AddCommand(CreateCommand(CommandSet, ToggleIndicatorMarginCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleLineNumbersCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleQuickActionsCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleSelectionMarginCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleTrackChangesCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleDiffMarginCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleOutliningCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateToggleLUTCommand());
                commandService.AddCommand(CreateCommand(CommandSet, ToggleAnnotateCmdId, this.MenuItemCallback));
            }
        }

        private OleMenuCommand CreateToggleLUTCommand()
        {
            OleMenuCommand ToggleLiveUnitTestingCommand = CreateOLECommand(CommandSet, ToggleLiveUnitTestingCmdId, ToggleLiveUnitTesting.ToggleLUT);
            ToggleLiveUnitTestingCommand.BeforeQueryStatus += ToggleLiveUnitTesting.OnBeforeQueryStatus;
            return ToggleLiveUnitTestingCommand;
        }

        private MenuCommand CreateCommand(Guid commandSet, int commandId, EventHandler handler)
        {
            var menuCommandID = new CommandID(commandSet, commandId);
            return new MenuCommand(handler, menuCommandID);
        }

        private OleMenuCommand CreateOLECommand(Guid commandSet, int commandId, EventHandler handler)
        {
            CommandID menuCommandID = new CommandID(commandSet, commandId);
            return new OleMenuCommand(handler, menuCommandID);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ToggleMargin Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private System.IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ToggleMargin(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            MenuCommand command = (MenuCommand)sender;
            string message = string.Format(CultureInfo.CurrentCulture, "Turn {0} margin {1}", command.Checked? "off" : "on", command.CommandID.ID);
            string title = "Toggle Margin";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            // Now perform the action
            //TODO: Update settings for visibility of selected margin

            // Update state of checkbox
            command.Checked = !command.Checked;
        }

    }
}
