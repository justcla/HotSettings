using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Operations;
using HotCommands;
using System.ComponentModel.Design;

namespace HotSettings
{
    internal sealed class CommandFilter : IOleCommandTarget
    {
        private readonly IWpfTextView textView;
        private readonly IClassifier classifier;
        private readonly SVsServiceProvider globalServiceProvider;
        private IEditorOperations editorOperations;

        public CommandFilter(IWpfTextView textView, IClassifierAggregatorService aggregatorFactory,
            SVsServiceProvider globalServiceProvider, IEditorOperationsFactoryService editorOperationsFactory)
        {
            this.textView = textView;
            classifier = aggregatorFactory.GetClassifier(textView.TextBuffer);
            this.globalServiceProvider = globalServiceProvider;
            editorOperations = editorOperationsFactory.GetEditorOperations(textView);
        }

        public IOleCommandTarget Next { get; internal set; }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            // Command handling registration
            if (pguidCmdGroup == Constants.HotSettingsCmdSetGuid && cCmds == 1)
            {
                switch (prgCmds[0].cmdID)
                {
                    case Constants.ToggleLUTCmdId:
                        HandleLUTQueryStatus(prgCmds);
                        return VSConstants.S_OK;
                    //case Constants.FormatCodeCmdId:
                    //    prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;
                    //    return VSConstants.S_OK;
                }
            }

            if (Next != null)
            {
                return Next.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }
            return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;
        }

        private void HandleLUTQueryStatus(OLECMD[] prgCmds)
        {
            prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_ENABLED;

            bool isLUTRunning = ToggleLiveUnitTesting.IsLiveUnitTestingRunning();
            // OLECMDF.OLECMDF_LATCHED = The command is an on - off toggle and is currently on
            if (isLUTRunning)
            {
                prgCmds[0].cmdf |= (uint)OLECMDF.OLECMDF_LATCHED; 
            } else
            {
                // Remove Latched flag if it is present
                if (prgCmds[0].cmdf == (prgCmds[0].cmdf | (uint)OLECMDF.OLECMDF_LATCHED))
                {
                    prgCmds[0].cmdf ^= (uint)OLECMDF.OLECMDF_LATCHED;
                }
            }

            // Set the checked state of the MenuCommand
            //OleMenuCommandService commandService = this.globalServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            OleMenuCommandService commandService = ServiceProvider.GlobalProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var toggleLUTCommandObj = commandService.FindCommand(new CommandID(Constants.HotSettingsCmdSetGuid, Constants.ToggleLUTCmdId));
                if (toggleLUTCommandObj != null)
                {
                    toggleLUTCommandObj.Checked = isLUTRunning;
                }
            }
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            // Command handling
            if (pguidCmdGroup == Constants.HotSettingsCmdSetGuid)
            {
                // Dispatch to the correct command handler
                switch (nCmdID)
                {
                    //case Constants.FormatCodeCmdId:
                    //    return FormatCode.Instance.HandleCommand(textView, GetShellCommandDispatcher());
                    case Constants.ToggleLUTCmdId:
                        return ToggleLiveUnitTesting.ToggleLUTRunningState();
                }
            }

            // No commands called. Pass to next command handler.
            if (Next != null)
            {
                return Next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;
        }

        ///// <summary>
        ///// Get the SUIHostCommandDispatcher from the global service provider.
        ///// </summary>
        //private IOleCommandTarget GetShellCommandDispatcher()
        //{
        //    return globalServiceProvider.GetService(typeof(SUIHostCommandDispatcher)) as IOleCommandTarget;
        //}
    }
}
