using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Settings;
using EnvDTE80;
using EnvDTE;

namespace HotSettings
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class HotSettingsCommandHandler
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private SettingsStore SettingsStore;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static HotSettingsCommandHandler Instance { get; private set; }

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
            Instance = new HotSettingsCommandHandler(package);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotSettingsCommandHandler"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private HotSettingsCommandHandler(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            ShellSettingsManager settingsManager = new ShellSettingsManager(package);
            SettingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);

            CreateCommands();
        }

        private void CreateCommands()
        {
            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                // Editor Margin Settings Commands
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleIndicatorMarginCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleLineNumbersCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleQuickActionsCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleSelectionMarginCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleTrackChangesCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleDiffMarginCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleOutliningCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleLiveUnitTestingCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleAnnotateCmdId));
                // Editor Settings Commands
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleNavigationBarCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleCodeLensCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleIndentGuidesCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleHighlightCurrentLineCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleAutoDelimiterHighlightingCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleProcedureLineSeparatorCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleIntelliSensePopUpCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleLineEndingsCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleHighlightSymbolsCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleHighlightKeywordsCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleIntelliSenseSquigglesCmdId));
                // Scrollbar Settings Commands
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowChangesCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowMarksCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowErrorsCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowCaretPositionCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowDiffsCmdId));
            }
        }

        private OleMenuCommand CreateHotSettingsCommand(int commandId)
        {
            Guid commandSet = Constants.HotSettingsCmdSetGuid;
            EventHandler execHandler = this.MenuItemCallback;
            EventHandler queryStatusHandler = this.OnBeforeQueryStatus;

            OleMenuCommand oleMenuCommand = CreateOleMenuCommand(commandSet, commandId, execHandler);
            oleMenuCommand.BeforeQueryStatus += queryStatusHandler;

            return oleMenuCommand;
        }

        private static OleMenuCommand CreateOleMenuCommand(Guid commandSet, int commandId, EventHandler handler)
        {
            CommandID menuCommandID = new CommandID(commandSet, commandId);
            return new OleMenuCommand(handler, menuCommandID);
        }

        /// <summary>
        /// This function is run whenever VS is considering displaying the command.
        /// For menu items, it runs it before opening the menu. For toolbar items, it seems to run is frequently.
        /// Here you can change the visibility of a menu item. Also set its Checked state, and other properties (like Enabled).
        /// </summary>
        public void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = (OleMenuCommand)sender;
            switch ((uint)command.CommandID.ID)
            {
                case Constants.ToggleIndicatorMarginCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Indicator Margin");
                    break;
                case Constants.ToggleLineNumbersCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor/All Languages", "Line numbers");     // TODO: Detect language of current file
                    break;
                case Constants.ToggleQuickActionsCmdId:
                    this.HideItem(sender);
                    break;
                case Constants.ToggleSelectionMarginCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Selection Margin");
                    break;
                case Constants.ToggleTrackChangesCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Track Changes");
                    break;
                case Constants.ToggleDiffMarginCmdId:
                    this.HideItem(sender);
                    break;
                case Constants.ToggleOutliningCmdId:
                    break;
                case Constants.ToggleLiveUnitTestingCmdId:
                    ToggleLiveUnitTesting.OnBeforeQueryStatus(sender, e);
                    break;
                case Constants.ToggleAnnotateCmdId:
                // Editor Settings
                case Constants.ToggleNavigationBarCmdId:
                    break;
                case Constants.ToggleCodeLensCmdId:
                    this.HideItem(sender);
                    break;
                case Constants.ToggleIndentGuidesCmdId:
                    this.HideItem(sender);
                    break;
                case Constants.ToggleHighlightCurrentLineCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Highlight Current Line");
                    break;
                case Constants.ToggleAutoDelimiterHighlightingCmdId:
                    break;
                case Constants.ToggleProcedureLineSeparatorCmdId:
                    break;
                case Constants.ToggleIntelliSensePopUpCmdId:
                    break;
                case Constants.ToggleLineEndingsCmdId:
                    break;
                case Constants.ToggleHighlightSymbolsCmdId:
                    break;
                case Constants.ToggleHighlightKeywordsCmdId:
                    break;
                case Constants.ToggleIntelliSenseSquigglesCmdId:
                // Scrollbar Settings
                case Constants.ToggleShowChangesCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor/All Languages/Scroll Bars", "Show Changes");
                    break;
                case Constants.ToggleShowMarksCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor/All Languages/Scroll Bars", "Show Marks");
                    break;
                case Constants.ToggleShowErrorsCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor/All Languages/Scroll Bars", "Show Errors");
                    break;
                case Constants.ToggleShowCaretPositionCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor/All Languages/Scroll Bars", "Show Caret Position");
                    break;
                case Constants.ToggleShowDiffsCmdId:
                    this.HideItem(sender);
                    break;
            }
        }

        private void HideItem(object sender)
        {
            MenuCommand command = (MenuCommand)sender;
            command.Enabled = false;
            command.Visible = false;
        }

        private void HandleQueryStatusCheckedUserProperty(object sender, string collectionPath, string propertyName)
        {
            try
            {
                // Read property value
                var value = SettingsStore.GetBoolean(collectionPath, propertyName);
                // Set Checked state
                ((MenuCommand)sender).Checked = value;

                // TODO: Set enabled and visible state
                // command.Enabled = ?
                // command.Visible = ?
            }
            catch (Exception)
            {
                // Do nothing
            }
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
                case Constants.ToggleIndicatorMarginCmdId:
                    UpdateSetting("TextEditor", "General", "MarginIndicatorBar", newCheckedState);
                    break;
                case Constants.ToggleLineNumbersCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowLineNumbers", newCheckedState);
                    break;
                case Constants.ToggleQuickActionsCmdId:
                    // TODO: Implement this - Not yet available by VS2017
                    break;
                case Constants.ToggleSelectionMarginCmdId:
                    UpdateSetting("TextEditor", "General", "SelectionMargin", newCheckedState);
                    break;
                case Constants.ToggleTrackChangesCmdId:
                    UpdateSetting("TextEditor", "General", "TrackChanges", newCheckedState);
                    break;
                case Constants.ToggleDiffMarginCmdId:
                    // TODO: Implement this
                    break;
                case Constants.ToggleOutliningCmdId:
                    // TODO: Implement this - Like Toggle Live Unit Testing
                    break;
                case Constants.ToggleLiveUnitTestingCmdId:
                    ToggleLiveUnitTesting.ToggleLUT(sender, e);
                    break;
                case Constants.ToggleAnnotateCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowAnnotations", newCheckedState); // TODO: Get this working
                    break;

                // Editor Settings
                case Constants.ToggleNavigationBarCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowNavigationBar", newCheckedState);
                    break;
                case Constants.ToggleCodeLensCmdId:
                    UpdateSetting("TextEditor", "CodeLens", "EnableCodeLens", newCheckedState);
                    break;
                case Constants.ToggleIndentGuidesCmdId:
                    UpdateSetting("TextEditor", "General", "IndentGuides", newCheckedState);
                    break;
                case Constants.ToggleHighlightCurrentLineCmdId:
                    UpdateSetting("TextEditor", "General", "HighlightCurrentLine", newCheckedState);
                    break;
                case Constants.ToggleAutoDelimiterHighlightingCmdId:
                    UpdateSetting("TextEditor", "General", "AutoDelimiterHighlighting", newCheckedState);
                    break;
                case Constants.ToggleProcedureLineSeparatorCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "DisplayLineSeparators", newCheckedState);
                    break;
                case Constants.ToggleIntelliSensePopUpCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "AutoListMembers", newCheckedState);
                    UpdateSetting("TextEditor", "AllLanguages", "AutoListParams", newCheckedState);
                    break;
                case Constants.ToggleLineEndingsCmdId:
                    UpdateSetting("TextEditor", "General", "TrackChanges", newCheckedState);
                    break;
                case Constants.ToggleHighlightSymbolsCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "HighlightReferences", newCheckedState);
                    break;
                case Constants.ToggleHighlightKeywordsCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "EnableHighlightRelatedKeywords", newCheckedState);
                    break;
                //case Constants.ToggleIntelliSenseSquigglesCmdId:
                //UpdateSetting("TextEditor", "Basic", "TrackChanges", newCheckedState);
                //break;
                // Scrollbar Settings
                case Constants.ToggleShowChangesCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowChanges", newCheckedState);
                    break;
                case Constants.ToggleShowMarksCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowMarks", newCheckedState);
                    break;
                case Constants.ToggleShowErrorsCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowErrors", newCheckedState);
                    break;
                case Constants.ToggleShowCaretPositionCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowCaretPosition", newCheckedState);
                    break;
                case Constants.ToggleShowDiffsCmdId:
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

    }
}
