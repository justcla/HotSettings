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
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using EnvDTE;

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
                commandService.AddCommand(CreateCommand(CommandSet, ToggleLiveUnitTestingCmdId, this.ToggleLUT));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleAnnotateCmdId, this.MenuItemCallback));
            }
        }

        private MenuCommand CreateCommand(Guid commandSet, int commandId, EventHandler handler)
        {
            var menuCommandID = new CommandID(commandSet, commandId);
            return new MenuCommand(handler, menuCommandID);
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

        private bool IsLiveUnitTestingRunning()
        {
            return IsCommandAvailable("Test.LiveUnitTesting.Stop");
        }

        private void ToggleLUT(object sender, EventArgs e)
        {
            MenuCommand command = (MenuCommand)sender;

            // Show a message box to prove we were here
            string message = string.Format(CultureInfo.CurrentCulture, "Turn Live Unit Testing {0}", command.Checked ? "off" : "on");
            string title = "Toggle Live Unit Testing";
            //VsShellUtilities.ShowMessageBox(this.ServiceProvider, message, title, OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            // Now perform the action
            if (!command.Checked && !IsLiveUnitTestingRunning())
            {
                ToggleLUTRunningState(command);  // Will Start LUT
            }
            else if (command.Checked && IsLiveUnitTestingRunning())
            {
                ToggleLUTRunningState(command); // Will Stop LUT
            }

            // Update state of checkbox
            command.Checked = !command.Checked;  //IsLiveUnitTestingRunning();  - This happens too slowly to be effective.
        }

        public int ToggleLUTRunningState(MenuCommand command)
        {
            // TODO: Fetch correct constants for LUT commands and cmdGroup
            Guid cmdGroup = new Guid("1E198C22-5980-4E7E-92F3-F73168D1FB63");  // GuidID = 146
            const uint startLutCmdId = 16897;
            const uint stopLutCmdId = 16900;

            // Call command to Start or Stop LiveUnitTesting depending on current state
            uint cmdID = IsLiveUnitTestingRunning() ? stopLutCmdId : startLutCmdId;
            //uint cmdID = command.Checked ? stopLutCmdId : startLutCmdId;
            int hr = GetShellCommandDispatcher().Exec(ref cmdGroup, cmdID, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Get the SUIHostCommandDispatcher from the global service provider.
        /// </summary>
        private IOleCommandTarget GetShellCommandDispatcher()
        {
            return this.ServiceProvider.GetService(typeof(SUIHostCommandDispatcher)) as IOleCommandTarget;
        }

        private TInterface GetGlobalService<TService, TInterface>()
            where TService : class
            where TInterface : class
        => (TInterface)this.ServiceProvider.GetService(typeof(TService));

        private DTE GetDTE()
            => GetGlobalService<SDTE, DTE>();

        private bool IsCommandAvailable(string commandName)
            => GetDTE().Commands.Item(commandName).IsAvailable;

        private void ExecuteCommand(string commandName, string args = "")
            => GetDTE().ExecuteCommand(commandName, args);

    }
}
