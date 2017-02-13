//------------------------------------------------------------------------------
// <copyright file="FormatCode.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Editor;
using OleInterop = Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace HotCommands
{
    /// <summary>
    /// Command handler for FormatCode
    /// </summary>
    internal sealed class FormatCode
    {

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        public Package package;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FormatCode Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        protected IServiceProvider ServiceProvider
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
            Instance = new FormatCode { package = package };
        }

        //public int HandleCommand(IWpfTextView textView)
        //{
        //    return VSConstants.S_OK;
        //}

        public int HandleCommand(IWpfTextView textView, OleInterop.IOleCommandTarget commandTarget)
        {
            Guid cmdGroup = VSConstants.VSStd2K;

            // Execute FormatSelection or FormatDocument depending on current state of selected code
            uint cmdID = IsCursorOnly(textView) ? (uint)VSConstants.VSStd2KCmdID.FORMATDOCUMENT : (uint)VSConstants.VSStd2KCmdID.FORMATSELECTION;
            commandTarget.Exec(ref cmdGroup, cmdID, (uint)OleInterop.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);

            return VSConstants.S_OK;
        }

        private bool IsCursorOnly(IWpfTextView textView)
        {
            if (textView.Selection.SelectedSpans.Count > 1) return false;
            // Only one selection. Check if there is any selected content.
            return textView.Selection.SelectedSpans[0].Length == 0;
        }
    }
}