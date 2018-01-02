using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.OLE.Interop;

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
        private IEditorOptionsFactoryService OptionsService;
        private IVsTextManager2 TextManager;
        //private IVsEditorAdaptersFactoryService EditorAdaptersFactoryService;

        public static OleMenuCommand ToggleShowMarksCmd;

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
            this.package = package ?? throw new ArgumentNullException("package");

            ShellSettingsManager settingsManager = new ShellSettingsManager(package);
            SettingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);

            OptionsService = ServicesUtil.GetMefService<IEditorOptionsFactoryService>(this.ServiceProvider);
            TextManager = (IVsTextManager2)ServiceProvider.GetService(typeof(SVsTextManager));

            RegisterGlobalCommands();
        }

        private void RegisterGlobalCommands()
        {
            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                // Editor Margin Settings Commands
                //commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleIndicatorMarginCmdId));
                //commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleLineNumbersCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleQuickActionsCmdId));
                //commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleSelectionMarginCmdId));
                //commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleTrackChangesCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleDiffMarginCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleOutliningCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleLiveUnitTestingCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleAnnotateCmdId));
                // Editor Settings Commands
                //commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleNavigationBarCmdId));
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
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowScrollbarMarkersCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowChangesCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowMarksCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowErrorsCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowCaretPositionCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleShowDiffsCmdId));
                // Distraction Free mode
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleCleanEditorCmdId));
                commandService.AddCommand(CreateHotSettingsCommand(Constants.ToggleCleanMarginsCmdId));
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

        public static OleMenuCommand CreateOleMenuCommand(Guid commandSet, int commandId, EventHandler handler)
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
                //case Constants.ToggleIndicatorMarginCmdId:
                //    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Indicator Margin");
                //    break;
                //case Constants.ToggleLineNumbersCmdId:
                //    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor\\CSharp", "Line Numbers");
                //    break;
                case Constants.ToggleQuickActionsCmdId:
                    this.HideItem(sender);
                    break;
                //case Constants.ToggleSelectionMarginCmdId:
                //    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Selection Margin");
                //    break;
                //case Constants.ToggleTrackChangesCmdId:
                //    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Track Changes");
                //    break;
                case Constants.ToggleDiffMarginCmdId:
                    this.HideItem(sender);
                    break;
                case Constants.ToggleOutliningCmdId:
                    this.HandleOutliningQueryStatus(sender);
                    break;
                case Constants.ToggleLiveUnitTestingCmdId:
                    ToggleLiveUnitTesting.OnBeforeQueryStatus(sender, e);
                    break;
                case Constants.ToggleAnnotateCmdId:
                    //this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor\\CSharp", "Show Blame");
                    break;
                // Editor Settings
                case Constants.ToggleNavigationBarCmdId:
                    this.HandleNavBarQueryStatus(sender);
                    break;
                case Constants.ToggleCodeLensCmdId:
                    this.HandleToggleCodeLensQueryStatus(sender);
                    break;
                case Constants.ToggleIndentGuidesCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Indent Guides");
                    // Hide if not VS2017+
                    //this.HideItem(sender);
                    break;
                case Constants.ToggleHighlightCurrentLineCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Highlight Current Line");
                    break;
                case Constants.ToggleAutoDelimiterHighlightingCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor", "Auto Delimiter Highlighting");
                    break;
                case Constants.ToggleProcedureLineSeparatorCmdId:
                    break;
                case Constants.ToggleIntelliSensePopUpCmdId:
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor\\CSharp", "Auto List Members");
                    this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor\\CSharp", "Auto List Params");
                    //this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor\\CSharp", "Hide Advanced Auto List Members");
                    break;
                case Constants.ToggleLineEndingsCmdId:
                    break;
                case Constants.ToggleHighlightSymbolsCmdId:
                    break;
                case Constants.ToggleHighlightKeywordsCmdId:
                    break;
                //case Constants.ToggleIntelliSenseSquigglesCmdId:
                // Scrollbar Settings
                case Constants.ToggleShowScrollbarMarkersCmdId:
                    // Turn off all Scrollbar markers with "ShowAnnotations"
                    //this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor\\CSharp", "ShowAnnotations");
                    MenuCommand menuCommand = ((MenuCommand)sender);
                    //menuCommand.Visible = true;
                    //menuCommand.Enabled = true;
                    menuCommand.Checked = SettingsStore.GetBoolean("Text Editor\\CSharp", "ShowAnnotations");
                    break;
                case Constants.ToggleShowChangesCmdId:
                    HandleScrollbarQueryStatus(sender, "ShowChanges");
                    break;
                case Constants.ToggleShowMarksCmdId:
                    HandleScrollbarQueryStatus(sender, "ShowMarks");
                    break;
                case Constants.ToggleShowErrorsCmdId:
                    HandleScrollbarQueryStatus(sender, "ShowErrors");
                    break;
                case Constants.ToggleShowCaretPositionCmdId:
                    HandleScrollbarQueryStatus(sender, "ShowCaretPosition");
                    break;
                case Constants.ToggleShowDiffsCmdId:
                    this.HideItem(sender);
                    break;
                    // Don't need query status for DistractionFree items.
                    //case Constants.ToggleCleanEditorCmdId:
                    //    break;
                    //case Constants.ToggleCleanMarginsCmdId:
                    //    break;
            }
        }

        private void HideItem(object sender)
        {
            MenuCommand command = (MenuCommand)sender;
            command.Enabled = false;
            command.Visible = false;
        }

        private void HandleScrollbarQueryStatus(object sender, string markerName)
        {
            // Only enable scrollbar options if ShowAnnotations is true.
            var showScrollbarMarkers = SettingsStore.GetBoolean("Text Editor\\CSharp", "ShowAnnotations");
            MenuCommand menuCommand = ((MenuCommand)sender);
            menuCommand.Enabled = showScrollbarMarkers;

            // Update checked state based on state of specific marker
            if (!showScrollbarMarkers)
            {
                UpdateCheckedState(sender, false);
            }
            else
            {
                this.HandleQueryStatusCheckedUserProperty(sender, "Text Editor\\CSharp", markerName);
            }
        }

        private void HandleQueryStatusCheckedUserProperty(object sender, string collectionPath, string propertyName)
        {
            try
            {
                // Read property value
                var value = SettingsStore.GetBoolean(collectionPath, propertyName);
                UpdateCheckedState(sender, value);
            }
            catch (Exception)
            {
                // Do nothing
            }
        }

        private void HandleNavBarQueryStatus(object sender)
        {
            LANGPREFERENCES2[] langPrefs = new LANGPREFERENCES2[] { new LANGPREFERENCES2() };
            VIEWPREFERENCES2[] viewPrefs = new VIEWPREFERENCES2[] { new VIEWPREFERENCES2() };
            langPrefs[0].guidLang = new Guid(0x8239bec4, 0xee87, 0x11d0, 0x8c, 0x98, 0x0, 0xc0, 0x4f, 0xc2, 0xab, 0x22); // guidDefaultFileType

            Marshal.ThrowExceptionForHR(TextManager.GetUserPreferences2(viewPrefs, null, langPrefs, null));
            LANGPREFERENCES2 lp = langPrefs[0];
            bool enabled = lp.fDropdownBar == 1;
            UpdateCheckedState(sender, enabled);
        }

        private void HandleOutliningQueryStatus(object sender)
        {
            var enabled = ShellUtil.IsCommandAvailable("Edit.StopOutlining");
            var disabled = ShellUtil.IsCommandAvailable("Edit.StartAutomaticOutlining");
            UpdateCheckedState(sender, enabled);
        }

        private void HandleToggleCodeLensQueryStatus(object sender)
        {
            var enabled = (bool)OptionsService.GlobalOptions.GetOptionValue("IsCodeLensEnabled");
            UpdateCheckedState(sender, enabled);
        }

        private void HandleToggleCodeLensAction(object sender, bool checkedState)
        {
            this.OptionsService.GlobalOptions.SetOptionValue("IsCodeLensEnabled", checkedState);
        }

        private static void UpdateCheckedState(object sender, bool checkedState)
        {
            // Set Checked state
            MenuCommand menuCommand = ((MenuCommand)sender);
            if (menuCommand.Checked == checkedState)
            {
                // Command is already in correct state. No-op.
                return;
            }
            menuCommand.Checked = checkedState;
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
                    if (newCheckedState)
                        ShellUtil.ExecuteCommand("Edit.StartAutomaticOutlining");
                    else
                        ShellUtil.ExecuteCommand("Edit.StopOutlining");
                    break;
                case Constants.ToggleLiveUnitTestingCmdId:
                    ToggleLiveUnitTesting.ToggleLUT(sender, e);
                    break;
                case Constants.ToggleAnnotateCmdId:
                    //UpdateSetting("TextEditor", "AllLanguages", "ShowBlame", newCheckedState); // TODO: Get this working
                    break;

                // Editor Settings
                case Constants.ToggleNavigationBarCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "ShowNavigationBar", newCheckedState);
                    command.Checked = newCheckedState;
                    break;
                case Constants.ToggleCodeLensCmdId:
                    HandleToggleCodeLensAction(sender, newCheckedState);
                    break;
                case Constants.ToggleIndentGuidesCmdId:
                    UpdateSetting("TextEditor", "General", "IndentGuides", newCheckedState);
                    command.Checked = newCheckedState;
                    break;
                case Constants.ToggleHighlightCurrentLineCmdId:
                    UpdateSetting("TextEditor", "General", "HighlightCurrentLine", newCheckedState);
                    break;
                case Constants.ToggleAutoDelimiterHighlightingCmdId:
                    UpdateSetting("TextEditor", "General", "AutoDelimiterHighlighting", newCheckedState);
                    command.Checked = newCheckedState;
                    break;
                case Constants.ToggleProcedureLineSeparatorCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "DisplayLineSeparators", newCheckedState);
                    command.Checked = newCheckedState;
                    break;
                case Constants.ToggleIntelliSensePopUpCmdId:
                    UpdateSetting("TextEditor", "AllLanguages", "AutoListMembers", newCheckedState);
                    UpdateSetting("TextEditor", "AllLanguages", "AutoListParams", newCheckedState);
                    break;
                case Constants.ToggleLineEndingsCmdId:
                    UpdateSetting("TextEditor", "General", "LineEndings", newCheckedState);
                    command.Checked = newCheckedState;
                    break;
                case Constants.ToggleHighlightSymbolsCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "HighlightReferences", newCheckedState);
                    command.Checked = newCheckedState;
                    break;
                case Constants.ToggleHighlightKeywordsCmdId:
                    UpdateSetting("TextEditor", "CSharp-Specific", "EnableHighlightRelatedKeywords", newCheckedState);
                    command.Checked = newCheckedState;
                    break;
                //case Constants.ToggleIntelliSenseSquigglesCmdId:
                //UpdateSetting("TextEditor", "Basic", "TrackChanges", newCheckedState);
                //break;
                // Scrollbar Settings
                case Constants.ToggleShowScrollbarMarkersCmdId:
                    UpdateSetting("TextEditor", "CSharp", "ShowAnnotations", newCheckedState); // Turns off all scrollbar markers
                    break;
                case Constants.ToggleShowChangesCmdId:
                    UpdateSetting("TextEditor", "CSharp", "ShowChanges", newCheckedState);
                    break;
                case Constants.ToggleShowMarksCmdId:
                    UpdateSetting("TextEditor", "CSharp", "ShowMarks", newCheckedState);
                    break;
                case Constants.ToggleShowErrorsCmdId:
                    UpdateSetting("TextEditor", "CSharp", "ShowErrors", newCheckedState);
                    break;
                case Constants.ToggleShowCaretPositionCmdId:
                    UpdateSetting("TextEditor", "CSharp", "ShowCaretPosition", newCheckedState);
                    break;
                case Constants.ToggleShowDiffsCmdId:
                    // Not implemented yet. Would hook into Git Diff Margin scrollbar setting.
                    break;
                case Constants.ToggleCleanEditorCmdId:
                    ExecuteToggleCleanEditor();
                    break;
                case Constants.ToggleCleanMarginsCmdId:
                    MessageBox.Show("Toggle Margins");
                    // Not implemented yet. Would turn off all visible margins, or restore all previously visible margins
                    break;
            }

            // Update state of checkbox
            //command.Checked = newCheckedState;
        }

        private void ExecuteToggleCleanEditor()
        {
            MessageBox.Show("Toggle Editor");
            // Not implemented yet. Would turn off all editor adornments (not margins)
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

        private void EnableAndCheckCommand(OLECMD[] prgCmds, bool isEnabled)
        {
            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_SUPPORTED;
            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;
            if (isEnabled)
            {
                prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_LATCHED;
            }
        }

        public void QueryStatusToggleIndicatorMargin(OLECMD[] prgCmds)
        {
            EnableAndCheckCommand(prgCmds, IsIndicatorMarginEnabled());
        }

        public void QueryStatusToggleLineNumbers(Guid langServiceGuid, OLECMD[] prgCmds)
        {
            EnableAndCheckCommand(prgCmds, IsLineNumbersEnabled(langServiceGuid));
        }

        public void QueryStatusToggleSelectionMargin(OLECMD[] prgCmds)
        {
            EnableAndCheckCommand(prgCmds, IsSelectionMarginEnabled());
        }

        public void QueryStatusToggleTrackChanges(OLECMD[] prgCmds)
        {
            //EnableAndCheckCommand(prgCmds, IsSelectionMarginEnabled());
            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_SUPPORTED;
            VIEWPREFERENCES2 viewPrefs = GetViewPreferences();
            if (IsSelectionMarginEnabled(viewPrefs))
            {
                prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;
            }
            if (IsTrackChangesEnabled(viewPrefs))
            {
                prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_LATCHED;
            }
        }

        public void QueryStatusToggleNavigationBar(Guid langServiceGuid, OLECMD[] prgCmds)
        {
            EnableAndCheckCommand(prgCmds, IsNavBarEnabled(langServiceGuid));
        }

        public void ExecToggleLineNumbers(IWpfTextView textView, Guid langServiceGuid)
        {
            // Get the language preferences
            LANGPREFERENCES2 langPrefs = GetLanguagePreferences(langServiceGuid);
            bool enabled = IsLineNumbersEnabled(langPrefs);
            // Update the Line Numbers state (toggle)
            langPrefs.fLineNumbers = (uint)(enabled ? 0 : 1);
            // Save the update to the langPrefs
            SetLangPrefererences(langPrefs);
        }

        public void ExecToggleIndicatorMargin(IWpfTextView textView)
        {
            // Get the view preferences
            VIEWPREFERENCES2 viewPrefs = GetViewPreferences();
            bool enabled = IsIndicatorMarginEnabled(viewPrefs);
            viewPrefs.fWidgetMargin = (uint)(enabled ? 0 : 1);
            // Save the update to the viewPrefs
            SetViewPrefererences(viewPrefs);
        }

        public void ExecToggleSelectionMargin(IWpfTextView textView)
        {
            // Get the view preferences
            VIEWPREFERENCES2 viewPrefs = GetViewPreferences();
            bool enabled = IsSelectionMarginEnabled(viewPrefs);
            viewPrefs.fSelectionMargin = (uint)(enabled ? 0 : 1);
            // Save the update to the viewPrefs
            SetViewPrefererences(viewPrefs);
        }

        private bool IsSelectionMarginEnabled()
        {
            return IsSelectionMarginEnabled(GetViewPreferences());
        }

        private bool IsSelectionMarginEnabled(VIEWPREFERENCES2 viewPrefs)
        {
            return viewPrefs.fSelectionMargin == 1;
        }

        public void ExecToggleTrackChanges(IWpfTextView textView)
        {
            // Get the view preferences
            var viewPrefs = GetViewPreferences();
            bool enabled = IsTrackChangesEnabled(viewPrefs);
            viewPrefs.fTrackChanges = (uint)(enabled ? 0 : 1);
            // Save the update to the viewPrefs
            SetViewPrefererences(viewPrefs);
        }

        private bool IsTrackChangesEnabled()
        {
            return IsTrackChangesEnabled(GetViewPreferences());
        }

        private bool IsTrackChangesEnabled(VIEWPREFERENCES2 viewPrefs)
        {
            return viewPrefs.fTrackChanges == 1;
        }

        private bool IsIndicatorMarginEnabled()
        {
            return IsIndicatorMarginEnabled(GetViewPreferences());
        }

        private bool IsIndicatorMarginEnabled(VIEWPREFERENCES2 viewPrefs)
        {
            return viewPrefs.fWidgetMargin == 1;
        }

        private bool IsLineNumbersEnabled(LANGPREFERENCES2 langPrefs)
        {
            return langPrefs.fLineNumbers == 1;
        }

        private bool IsLineNumbersEnabled(Guid langServiceGuid)
        {
            return IsLineNumbersEnabled(GetLanguagePreferences(langServiceGuid));
        }

        private VIEWPREFERENCES2 GetViewPreferences()
        {
            VIEWPREFERENCES2[] viewPrefs = new VIEWPREFERENCES2[] { new VIEWPREFERENCES2() };
            Marshal.ThrowExceptionForHR(TextManager.GetUserPreferences2(viewPrefs, null, null, null));
            return viewPrefs[0];
        }

        private void SetViewPrefererences(VIEWPREFERENCES2 viewPrefs)
        {
            Marshal.ThrowExceptionForHR(TextManager.SetUserPreferences2(new VIEWPREFERENCES2[] { viewPrefs }, null, null, null));
        }

        private LANGPREFERENCES2 GetLanguagePreferences(Guid langServiceGuid)
        {
            LANGPREFERENCES2[] langPrefs = new LANGPREFERENCES2[] { new LANGPREFERENCES2() };
            langPrefs[0].guidLang = langServiceGuid;
            Marshal.ThrowExceptionForHR(TextManager.GetUserPreferences2(null, null, langPrefs, null));
            return langPrefs[0];
        }
        private void SetLangPrefererences(LANGPREFERENCES2 langPrefs)
        {
            Marshal.ThrowExceptionForHR(TextManager.SetUserPreferences2(null, null, new LANGPREFERENCES2[] { langPrefs }, null));
        }

        public void ExecToggleNavigationBar(IWpfTextView textView, Guid langServiceGuid)
        {
            // Get the language preferences
            LANGPREFERENCES2 langPrefs = GetLanguagePreferences(langServiceGuid);
            bool enabled = IsNavBarEnabled(langPrefs);
            // Update the state (toggle)
            langPrefs.fDropdownBar = (uint)(enabled ? 0 : 1);
            // Save the update to the langPrefs
            SetLangPrefererences(langPrefs);
        }

        private bool IsNavBarEnabled(Guid langServiceGuid)
        {
            return IsNavBarEnabled(GetLanguagePreferences(langServiceGuid));
        }

        private static bool IsNavBarEnabled(LANGPREFERENCES2 langPrefs)
        {
            return langPrefs.fDropdownBar == 1;
        }

    }
}
