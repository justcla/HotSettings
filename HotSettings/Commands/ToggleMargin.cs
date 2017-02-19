//------------------------------------------------------------------------------
// <copyright file="ToggleMargin.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Settings;
using EnvDTE80;
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
        // Editor Settings CmdIds
        public const int ToggleNavigationBarCmdId = 0x1041;
        public const int ToggleCodeLensCmdId = 0x1042;
        public const int ToggleIndentGuidesCmdId = 0x1043;
        public const int ToggleHighlightCurrentLineCmdId = 0x1050;
        public const int ToggleAutoDelimiterHighlightingCmdId = 0x1051;
        public const int ToggleProcedureLineSeparatorCmdId = 0x1052;
        public const int ToggleIntelliSensePopUpCmdId = 0x1053;
        public const int ToggleLineEndingsCmdId = 0x1054;
        public const int ToggleHighlightSymbolsCmdId = 0x1055;
        public const int ToggleHighlightKeywordsCmdId = 0x1056;
        public const int ToggleIntelliSenseSquigglesCmdId = 0x1057;
        // Scrollbar Settings CmdIds
        public const int ToggleShowChangesCmdId = 0x1071;
        public const int ToggleShowMarksCmdId = 0x1072;
        public const int ToggleShowErrorsCmdId = 0x1073;
        public const int ToggleShowCaretPositionCmdId = 0x1074;
        public const int ToggleShowDiffsCmdId = 0x1080;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("c75f116c-9249-4984-8d82-d3c6025afb17");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private ShellSettingsManager SettingsManager;
        private WritableSettingsStore Settings;

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
                // Editor Settings Commands
                commandService.AddCommand(CreateCommand(CommandSet, ToggleNavigationBarCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleCodeLensCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleIndentGuidesCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleHighlightCurrentLineCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleAutoDelimiterHighlightingCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleProcedureLineSeparatorCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleIntelliSensePopUpCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleLineEndingsCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleHighlightSymbolsCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleHighlightKeywordsCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleIntelliSenseSquigglesCmdId, this.MenuItemCallback));
                // Scrollbar Settings Commands
                commandService.AddCommand(CreateCommand(CommandSet, ToggleShowChangesCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleShowMarksCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleShowErrorsCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleShowCaretPositionCmdId, this.MenuItemCallback));
                commandService.AddCommand(CreateCommand(CommandSet, ToggleShowDiffsCmdId, this.MenuItemCallback));
            }

            SettingsManager  = new ShellSettingsManager(package);
            Settings = SettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        private OleMenuCommand CreateToggleLUTCommand()
        {
            OleMenuCommand ToggleLiveUnitTestingCommand = CreateOLECommand(CommandSet, ToggleLiveUnitTestingCmdId, this.MenuItemCallback);
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
            bool newCheckedState = !command.Checked;

            // Dispatch the action
            switch (command.CommandID.ID)
            {
                case ToggleIndicatorMarginCmdId:
                    UpdateSetting("TextEditor", "General", "MarginIndicatorBar", newCheckedState);
                    break;
                case ToggleLineNumbersCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowLineNumbers", newCheckedState);
                    break;
                case ToggleQuickActionsCmdId:
                    // TODO: Implement this - Not yet available by VS2017
                    break;
                case ToggleSelectionMarginCmdId:
                    UpdateSetting("TextEditor", "General", "SelectionMargin", newCheckedState);
                    break;
                case ToggleTrackChangesCmdId:
                    UpdateSetting("TextEditor", "General", "TrackChanges", newCheckedState);
                    break;
                case ToggleDiffMarginCmdId:
                    // TODO: Implement this
                    break;
                case ToggleOutliningCmdId:
                    // TODO: Implement this - Like Toggle Live Unit Testing
                    break;
                case ToggleLiveUnitTestingCmdId:
                    ToggleLiveUnitTesting.ToggleLUT(sender, e);
                    break;
                case ToggleAnnotateCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowAnnotations", newCheckedState); // TODO: Get this working
                    break;

                // Editor Settings
                case ToggleNavigationBarCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowNavigationBar", newCheckedState);
                    break;
                case ToggleCodeLensCmdId:
                    UpdateSetting("TextEditor", "CodeLens", "EnableCodeLens", newCheckedState);
                    break;
                case ToggleIndentGuidesCmdId:
                    UpdateSetting("TextEditor", "General", "IndentGuides", newCheckedState);
                    break;
                case ToggleHighlightCurrentLineCmdId:
                    UpdateSetting("TextEditor", "General", "HighlightCurrentLine", newCheckedState);
                    break;
                case ToggleAutoDelimiterHighlightingCmdId:
                    UpdateSetting("TextEditor", "General", "AutoDelimiterHighlighting", newCheckedState);
                    break;
                case ToggleProcedureLineSeparatorCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "DisplayLineSeparators", newCheckedState);
                    break;
                case ToggleIntelliSensePopUpCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "AutoListMembers", newCheckedState);
                    UpdateSetting("TextEditor", "AllLanguages", "AutoListParams", newCheckedState);
                    break;
                case ToggleLineEndingsCmdId:
                    UpdateSetting("TextEditor", "General", "TrackChanges", newCheckedState);
                    break;
                case ToggleHighlightSymbolsCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "HighlightReferences", newCheckedState);
                    break;
                case ToggleHighlightKeywordsCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "EnableHighlightRelatedKeywords", newCheckedState);
                    break;
                //case ToggleIntelliSenseSquigglesCmdId:
                    //UpdateSetting("TextEditor", "Basic", "TrackChanges", newCheckedState);
                    //break;
                // Scrollbar Settings
                case ToggleShowChangesCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowChanges", newCheckedState);
                    break;
                case ToggleShowMarksCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowMarks", newCheckedState);
                    break;
                case ToggleShowErrorsCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowErrors", newCheckedState);
                    break;
                case ToggleShowCaretPositionCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowCaretPosition", newCheckedState);
                    break;
                case ToggleShowDiffsCmdId:
                    // Not implemented yet. Would hook into Git Diff Margin scrollbar setting.
                    break;
            }

            // Update state of checkbox
            command.Checked = newCheckedState;
        }

        private void UpdateSetting(string category, string page, string settingName, bool value)
        {
            try
            {
                DTE2 _DTE2 = (DTE2)ServiceProvider.GetService(typeof(DTE));
                // Example: _DTE2.Properties["TextEditor", "General"].Item("TrackChanges").Value = true;
                Properties properties = _DTE2.Properties[category, page];
                properties.Item(settingName).Value = value;
            }
            catch (Exception)
            {
                // Do nothing
            }
        }

        public void PrintProperties()
        {
            Debug.WriteLine("===== General =========");
            PrintItems("TextEditor", "General");
            Debug.WriteLine("===== CSharp =========");
            PrintItems("TextEditor", "CSharp");
            Debug.WriteLine("===== CSharp-Specific =========");
            PrintItems("TextEditor", "CSharp-Specific");
            Debug.WriteLine("===== All Languages =========");
            PrintItems("TextEditor", "AllLanguages");
        }

        private void PrintItems(string category, string page)
        {
            DTE2 _DTE2 = (DTE2)ServiceProvider.GetService(typeof(DTE));
            Properties properties = _DTE2.Properties[category, page];

            foreach (Property prop in properties)
            {
                try
                {
                    Debug.WriteLine(prop.Name);
                }
                catch (Exception ex)
                {
                    // Do nothing
                }
            }
        }

        private void AlertMsg(string alertMessage)
        {
            VsShellUtilities.ShowMessageBox(this.ServiceProvider,
                alertMessage,
                "HotSettings Info",
                OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
