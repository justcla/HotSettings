using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Operations;

namespace HotSettings
{
    internal sealed class HotSettingsCommandFilter : IOleCommandTarget
    {
        private readonly IWpfTextView textView;
        private Guid languageServiceGuid;

        public HotSettingsCommandFilter(IWpfTextView textView, Guid languageServiceGuid)
        {
            this.textView = textView;
            this.languageServiceGuid = languageServiceGuid;
        }

        public IOleCommandTarget Next { get; internal set; }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            // Command handling
            if (pguidCmdGroup == Constants.HotSettingsCmdSetGuid)
            {
                // Dispatch to the correct command handler
                switch (nCmdID)
                {
                    case Constants.ToggleLineNumbersCmdId:
                        return HotSettingsCommandHandler.Instance.ExecToggleLineNumbers(textView, languageServiceGuid);
                            //HandleCommand(textView, classifier, GetShellCommandDispatcher(), editorOperations);
                    //case Constants.ExpandSelectionCmdId:
                    //    return ExpandSelection.Instance.HandleCommand(textView, true);
                    //case Constants.ShrinkSelectionCmdId:
                    //    return ExpandSelection.Instance.HandleCommand(textView, false);
                    //case Constants.FormatCodeCmdId:
                    //    return FormatCode.Instance.HandleCommand(textView, GetShellCommandDispatcher());
                    //case Constants.DuplicateSelectionCmdId:
                    //    return DuplicateSelection.HandleCommand(textView, classifier, GetShellCommandDispatcher(), editorOperations);
                    //case Constants.DuplicateSelectionReverseCmdId:
                    //    return DuplicateSelection.HandleCommand(textView, classifier, GetShellCommandDispatcher(), editorOperations, true);
                    //case Constants.JoinLinesCmdId:
                    //    return JoinLines.HandleCommand(textView, classifier, GetShellCommandDispatcher(), editorOperations);
                    //case Constants.MoveMemberUpCmdId:
                    //    return MoveMemberUp.Instance.HandleCommand(textView, GetShellCommandDispatcher(), editorOperations);
                    //case Constants.MoveMemberDownCmdId:
                    //    return MoveMemberDown.Instance.HandleCommand(textView, GetShellCommandDispatcher(), editorOperations);
                    //case Constants.GoToPreviousMemberCmdId:
                    //    return MoveCursorToAdjacentMember.MoveToPreviousMember(textView, editorOperations);
                    //case Constants.GoToNextMemberCmdId:
                    //    return MoveCursorToAdjacentMember.MoveToNextMember(textView, editorOperations);
                }
            }

            // No commands called. Pass to next command handler.
            if (Next != null)
            {
                return Next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            // Command handling registration
            if (pguidCmdGroup == Constants.HotSettingsCmdSetGuid && cCmds == 1)
            {
                switch (prgCmds[0].cmdID)
                {
                    case Constants.ToggleLineNumbersCmdId:
                        HotSettingsCommandHandler.Instance.QueryStatusToggleLineNumbers(languageServiceGuid, prgCmds);
                        return VSConstants.S_OK;
                    //case Constants.ExpandSelectionCmdId:
                    //case Constants.FormatCodeCmdId:
                    //case Constants.DuplicateSelectionCmdId:
                    //case Constants.DuplicateSelectionReverseCmdId:
                    //case Constants.MoveMemberUpCmdId:
                    //case Constants.MoveMemberDownCmdId:
                    //case Constants.GoToPreviousMemberCmdId:
                    case Constants.GoToNextMemberCmdId:
                        prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;
                        return VSConstants.S_OK;
                }
            }

            if (Next != null)
            {
                return Next.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }
            return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;
        }

    }
}
