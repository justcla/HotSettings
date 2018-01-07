using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace HotSettings
{
    internal sealed class HotSettingsCommandFilter : IOleCommandTarget
    {
        private readonly IWpfTextView textView;
        private Guid languageServiceGuid;

        private IVsTextManager4 TextManager;

        // Map<LanguageGuid, RestoreActions>
        private static Dictionary<Guid, Dictionary<string, object>> RestoreSettingsMap = new Dictionary<Guid, Dictionary<string, object>>();

        public HotSettingsCommandFilter(IWpfTextView textView, Guid languageServiceGuid, IVsTextManager4 textManager)
        {
            this.textView = textView;
            this.languageServiceGuid = languageServiceGuid;
            TextManager = textManager;
        }

        public IOleCommandTarget Next { get; internal set; }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            // Command handling registration
            if (pguidCmdGroup == Constants.HotSettingsCmdSetGuid && cCmds == 1)
            {
                switch (prgCmds[0].cmdID)
                {
                    // Editor Margins
                    case Constants.ToggleCleanMarginsCmdId:
                        QueryStatusToggleCleanMargins(prgCmds, pCmdText);
                        return VSConstants.S_OK;
                    case Constants.ToggleIndicatorMarginCmdId:
                        QueryStatusToggleIndicatorMargin(prgCmds);
                        return VSConstants.S_OK;
                    case Constants.ToggleLineNumbersCmdId:
                        QueryStatusToggleLineNumbers(languageServiceGuid, prgCmds);
                        return VSConstants.S_OK;
                    case Constants.ToggleSelectionMarginCmdId:
                        QueryStatusToggleSelectionMargin(prgCmds);
                        return VSConstants.S_OK;
                    case Constants.ToggleTrackChangesCmdId:
                        QueryStatusToggleTrackChanges(prgCmds);
                        return VSConstants.S_OK;

                    // Editor Navigational Aids
                    case Constants.ToggleNavigationBarCmdId:
                        QueryStatusToggleNavigationBar(languageServiceGuid, prgCmds);
                        return VSConstants.S_OK;
                    //case Constants.ToggleCodeLensCmdId:
                    //    QueryStatusToggleCodeLens(languageServiceGuid, prgCmds);
                    //    return VSConstants.S_OK;
                    case Constants.ToggleIndentGuidesCmdId:
                        QueryStatusToggleIndentGuides(prgCmds);
                        return VSConstants.S_OK;
                }
            }

            if (Next != null)
            {
                return Next.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }
            return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            // Command handling
            if (pguidCmdGroup == Constants.HotSettingsCmdSetGuid)
            {
                // Dispatch to the correct command handler
                switch (nCmdID)
                {
                    // Editor Margins
                    case Constants.ToggleCleanMarginsCmdId:
                        ExecToggleCleanMargins(textView);
                        return VSConstants.S_OK;
                    case Constants.ToggleIndicatorMarginCmdId:
                        ExecToggleIndicatorMargin();
                        return VSConstants.S_OK;
                    case Constants.ToggleLineNumbersCmdId:
                        ExecToggleLineNumbers(languageServiceGuid);
                        return VSConstants.S_OK;
                    case Constants.ToggleSelectionMarginCmdId:
                        ExecToggleSelectionMargin(textView);
                        return VSConstants.S_OK;
                    case Constants.ToggleTrackChangesCmdId:
                        ExecToggleTrackChanges(textView);
                        return VSConstants.S_OK;

                    // Editor Navigational Aids
                    case Constants.ToggleNavigationBarCmdId:
                        ExecToggleNavigationBar(textView, languageServiceGuid);
                        return VSConstants.S_OK;
                    case Constants.ToggleIndentGuidesCmdId:
                        ExecToggleIndentGuides(textView);
                        return VSConstants.S_OK;
                }
            }

            // No commands called. Pass to next command handler.
            if (Next != null)
            {
                return Next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;
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

        internal void QueryStatusToggleCleanMargins(OLECMD[] prgCmds, IntPtr pCmdText)
        {
            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_SUPPORTED;
            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;

            // Toggle the text on the menu command - Hide/Restore editor margins
            GetAllUserPreferences(languageServiceGuid, out var viewPrefs, out var langPrefs);
            if (AllMarginsAreHidden(viewPrefs, langPrefs))
            {
                SetOleCmdText(pCmdText, "Restore Hidden &Margins");
            } else
            {
                SetOleCmdText(pCmdText, "Hide Editor &Margins");
            }
        }

        public void SetOleCmdText(IntPtr pCmdText, string text)
        {
            OLECMDTEXT CmdText = (OLECMDTEXT)Marshal.PtrToStructure(pCmdText, typeof(OLECMDTEXT));
            char[] buffer = text.ToCharArray();
            IntPtr pText = (IntPtr)((long)pCmdText + (long)Marshal.OffsetOf(typeof(OLECMDTEXT), "rgwz"));
            IntPtr pCwActual = (IntPtr)((long)pCmdText + (long)Marshal.OffsetOf(typeof(OLECMDTEXT), "cwActual"));
            // The max chars we copy is our string, or one less than the buffer size, since we need a null at the end.
            int maxChars = (int)Math.Min(CmdText.cwBuf - 1, buffer.Length);
            Marshal.Copy(buffer, 0, pText, maxChars);
            // append a null
            Marshal.WriteInt16((IntPtr)((long)pText + (long)maxChars * 2), (Int16)0);
            // write out the length + null char
            Marshal.WriteInt32(pCwActual, maxChars + 1);
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
            VIEWPREFERENCES3 viewPrefs = GetViewPreferences();
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

        internal void QueryStatusToggleIndentGuides(OLECMD[] prgCmds)
        {
            EnableAndCheckCommand(prgCmds, IsIndentGuidesEnabled());
        }

        internal void ExecToggleCleanMargins(IWpfTextView textView)
        {
            GetAllUserPreferences(languageServiceGuid, out var viewPrefs, out var langPrefs);
            if (AllMarginsAreHidden(viewPrefs, langPrefs))
            {
                RestoreMargins();
            } else
            {
                SaveCurrentViewAndLangPrefs(viewPrefs, langPrefs);
                HideAllEditorMargins(viewPrefs, langPrefs);
            }
        }

        private bool AllMarginsAreHidden(VIEWPREFERENCES3 viewPrefs, LANGPREFERENCES3 langPrefs)
        {
            // Check for BreakPoint, LineNumbers, SelectionMargin, TrackChanges
            if (IsIndicatorMarginEnabled(viewPrefs)) return false;
            if (IsLineNumbersEnabled(langPrefs)) return false;
            if (IsSelectionMarginEnabled(viewPrefs)) return false;
            return true;
        }

        private void HideAllEditorMargins(VIEWPREFERENCES3 viewPrefs, LANGPREFERENCES3 langPrefs)
        {
            // Mark all editor margins as hidden
            viewPrefs.fWidgetMargin = 0;
            langPrefs.fLineNumbers = 0;
            viewPrefs.fSelectionMargin = 0;
            SaveAllUserPreferences(viewPrefs, langPrefs);
        }

        private void SaveCurrentViewAndLangPrefs(VIEWPREFERENCES3 viewPrefs, LANGPREFERENCES3 langPrefs)
        {
            // Create a Map of settings to restore to
            Dictionary<string, object> restoreSettings = new Dictionary<string, object>
            {
                // Save the viewPrefs and langPrefs
                ["ViewPrefs"] = viewPrefs,       // Holds Breakpoint, SelectionMargin
                ["LangPrefs"] = langPrefs       // Holds LineNumbers
            };

            // Save the RestoreActions to the global map against the given language type
            RestoreSettingsMap[languageServiceGuid] = restoreSettings;
        }

        private void RestoreMargins()
        {
            // Get old user settings
            Dictionary<string, object> restoreSettings = RestoreSettingsMap[languageServiceGuid];
            if (restoreSettings == null) return;    // TODO: Check this
            VIEWPREFERENCES3 oldViewPrefs = (VIEWPREFERENCES3)restoreSettings["ViewPrefs"];
            LANGPREFERENCES3 oldLangPrefs = (LANGPREFERENCES3)restoreSettings["LangPrefs"];
            // Fetch the latest settings
            GetAllUserPreferences(languageServiceGuid, out VIEWPREFERENCES3 viewPrefs, out LANGPREFERENCES3 langPrefs);
            // Update all the margins to be the same as the restore settings
            viewPrefs.fWidgetMargin = oldViewPrefs.fWidgetMargin;
            viewPrefs.fSelectionMargin = oldViewPrefs.fSelectionMargin;
            langPrefs.fLineNumbers = oldLangPrefs.fLineNumbers;
            // Save the new settings
            SaveAllUserPreferences(viewPrefs, langPrefs);

            //if (!(bool)RestoreSettingsMap.TryGetValue(languageServiceGuid, out List<Action> restoreActions)) return;
            // Execute each action in the RestoreActions set
            //restoreActions.ForEach(x => x());
        }

        private void SaveAllUserPreferences(VIEWPREFERENCES3 viewPrefs, LANGPREFERENCES3 langPrefs)
        {
            Marshal.ThrowExceptionForHR(TextManager.SetUserPreferences4(new VIEWPREFERENCES3[] { viewPrefs }, new LANGPREFERENCES3[] { langPrefs }, null));
        }

        private void EnableIndicatorMargin()
        {
            var viewPrefs = GetViewPreferences();
            if (!IsSelectionMarginEnabled(viewPrefs))
            {
                SetIndicatorMarginEnabled(viewPrefs, true);
            }
        }

        private void EnableLineNumbers()
        {
            var langPrefs = GetLanguagePreferences(languageServiceGuid);
            if (!IsLineNumbersEnabled(langPrefs))
            {
                SetLineNumbersEnabled(true);
            }
        }

        private void EnableSelectionMargin()
        {
            var viewPrefs = GetViewPreferences();
            if (!IsSelectionMarginEnabled(viewPrefs))
            {
                SetSelectionMarginEnabled(viewPrefs, true);
            }
        }

        private void EnableTrackChanges()
        {
            var viewPrefs = GetViewPreferences();
            if (!IsSelectionMarginEnabled(viewPrefs))
            {
                SetTrackChangesEnabled(viewPrefs, true);
            }
        }

        public void ExecToggleSelectionMargin(IWpfTextView textView)
        {
            // Get the view preferences
            VIEWPREFERENCES3 viewPrefs = GetViewPreferences();
            bool enabled = IsSelectionMarginEnabled(viewPrefs);
            SetSelectionMarginEnabled(viewPrefs, !enabled);
        }

        public void ExecToggleLineNumbers(Guid langServiceGuid)
        {
            // Get the language preferences
            LANGPREFERENCES3 langPrefs = GetLanguagePreferences(langServiceGuid);
            bool enabled = IsLineNumbersEnabled(langPrefs);
            SetLineNumbersEnabled(langPrefs, !enabled);
        }



        private void SetLineNumbersEnabled(bool enabled)
        {
            SetLineNumbersEnabled(GetLanguagePreferences(languageServiceGuid), enabled);
        }

        private void SetLineNumbersEnabled(LANGPREFERENCES3 langPrefs, bool enabled)
        {
            // Update the Line Numbers state
            langPrefs.fLineNumbers = (uint)(enabled ? 1 : 0);
            // Save the update to the langPrefs
            SetLangPrefererences(langPrefs);
        }

        public void ExecToggleIndicatorMargin()
        {
            // Get the view preferences
            VIEWPREFERENCES3 viewPrefs = GetViewPreferences();
            bool enabled = IsIndicatorMarginEnabled(viewPrefs);
            SetIndicatorMarginEnabled(!enabled);
        }

        public void SetIndicatorMarginEnabled(bool bShow)
        {
            // Get the view preferences
            SetIndicatorMarginEnabled(GetViewPreferences(), bShow);
        }

        public void SetIndicatorMarginEnabled(VIEWPREFERENCES3 viewPrefs, bool bShow)
        {
            // Set the value
            viewPrefs.fWidgetMargin = (uint)(bShow ? 1 : 0);
            // Save the update to the viewPrefs
            SetViewPrefererences(viewPrefs);
        }

        private void SetSelectionMarginEnabled(VIEWPREFERENCES3 viewPrefs, bool enabled)
        {
            viewPrefs.fSelectionMargin = (uint)(enabled ? 1 : 0);
            // Save the update to the viewPrefs
            SetViewPrefererences(viewPrefs);
        }

        private bool IsSelectionMarginEnabled()
        {
            return IsSelectionMarginEnabled(GetViewPreferences());
        }

        private bool IsSelectionMarginEnabled(VIEWPREFERENCES3 viewPrefs)
        {
            return viewPrefs.fSelectionMargin == 1;
        }

        public void ExecToggleTrackChanges(IWpfTextView textView)
        {
            // Get the view preferences
            var viewPrefs = GetViewPreferences();
            bool enabled = IsTrackChangesEnabled(viewPrefs);
            SetTrackChangesEnabled(viewPrefs, !enabled);
        }

        private void SetTrackChangesEnabled(VIEWPREFERENCES3 viewPrefs, bool enabled)
        {
            viewPrefs.fTrackChanges = (uint)(enabled ? 1 : 0);
            // Save the update to the viewPrefs
            SetViewPrefererences(viewPrefs);
        }

        public void ExecToggleIndentGuides(IWpfTextView textView)
        {
            // Get the view preferences
            var viewPrefs = GetViewPreferences();
            bool enabled = IsIndentGuidesEnabled();
            // TODO: Find property to update indent guides setting
            //viewPrefs.fStructureGuidelines = (uint)(enabled ? 0 : 1);
            // Save the update to the viewPrefs
            SetViewPrefererences(viewPrefs);
        }

        private bool IsTrackChangesEnabled()
        {
            return IsTrackChangesEnabled(GetViewPreferences());
        }

        private bool IsTrackChangesEnabled(VIEWPREFERENCES3 viewPrefs)
        {
            return viewPrefs.fTrackChanges == 1;
        }

        private bool IsIndicatorMarginEnabled()
        {
            return IsIndicatorMarginEnabled(GetViewPreferences());
        }

        private bool IsIndicatorMarginEnabled(VIEWPREFERENCES3 viewPrefs)
        {
            return viewPrefs.fWidgetMargin == 1;
        }

        private bool IsLineNumbersEnabled(LANGPREFERENCES3 langPrefs)
        {
            return langPrefs.fLineNumbers == 1;
        }

        private bool IsLineNumbersEnabled(Guid langServiceGuid)
        {
            return IsLineNumbersEnabled(GetLanguagePreferences(langServiceGuid));
        }

        private bool IsIndentGuidesEnabled()
        {
            var viewPrefs = GetViewPreferences();
            // TODO: Read value of Show Structure Guides / Indent Guidelines
            return true;
        }

        private VIEWPREFERENCES3 GetViewPreferences()
        {
            VIEWPREFERENCES3[] viewPrefs = new VIEWPREFERENCES3[] { new VIEWPREFERENCES3() };
            Marshal.ThrowExceptionForHR(TextManager.GetUserPreferences4(viewPrefs, null, null));
            return viewPrefs[0];
        }

        private void SetViewPrefererences(VIEWPREFERENCES3 viewPrefs)
        {
            Marshal.ThrowExceptionForHR(TextManager.SetUserPreferences4(new VIEWPREFERENCES3[] { viewPrefs }, null, null));
        }

        private void GetAllUserPreferences(Guid langServiceGuid, out VIEWPREFERENCES3 viewPrefs, out LANGPREFERENCES3 langPrefs)
        {
            LANGPREFERENCES3[] langPrefsArr = new LANGPREFERENCES3[] { new LANGPREFERENCES3() };
            VIEWPREFERENCES3[] viewPrefsArr = new VIEWPREFERENCES3[] { new VIEWPREFERENCES3() };
            langPrefsArr[0].guidLang = langServiceGuid;
            Marshal.ThrowExceptionForHR(TextManager.GetUserPreferences4(viewPrefsArr, langPrefsArr, null));
            langPrefs = langPrefsArr[0];
            viewPrefs = viewPrefsArr[0];
        }

        private LANGPREFERENCES3 GetLanguagePreferences(Guid langServiceGuid)
        {
            LANGPREFERENCES3[] langPrefs = new LANGPREFERENCES3[] { new LANGPREFERENCES3() };
            langPrefs[0].guidLang = langServiceGuid;
            Marshal.ThrowExceptionForHR(TextManager.GetUserPreferences4(null, langPrefs, null));
            return langPrefs[0];
        }

        private void SetLangPrefererences(LANGPREFERENCES3 langPrefs)
        {
            Marshal.ThrowExceptionForHR(TextManager.SetUserPreferences4(null, new LANGPREFERENCES3[] { langPrefs }, null));
        }

        public void ExecToggleNavigationBar(IWpfTextView textView, Guid langServiceGuid)
        {
            // Get the language preferences
            LANGPREFERENCES3 langPrefs = GetLanguagePreferences(langServiceGuid);
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

        private static bool IsNavBarEnabled(LANGPREFERENCES3 langPrefs)
        {
            return langPrefs.fDropdownBar == 1;
        }



    }
}
