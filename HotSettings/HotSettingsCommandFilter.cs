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
                        HotSettingsCommandHandler.Instance.ExecToggleLineNumbers(textView, languageServiceGuid);
                        return VSConstants.S_OK;
                    case Constants.ToggleNavigationBarCmdId:
                        HotSettingsCommandHandler.Instance.ExecToggleNavigationBar(textView, languageServiceGuid);
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
                    case Constants.ToggleNavigationBarCmdId:
                        HotSettingsCommandHandler.Instance.QueryStatusToggleNavigationBar(languageServiceGuid, prgCmds);
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
