using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Operations;
using System;
#pragma warning disable 0649

namespace HotSettings
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class HotSettingsCommandsTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        private IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        private IClassifierAggregatorService _aggregatorFactory;

        [Import]
        private SVsServiceProvider _globalServiceProvider;

        [Import(typeof(IEditorOperationsFactoryService))]
        private IEditorOperationsFactoryService _editorOperationsFactory;

        private IVsTextManager6 TextManager;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            Guid langServiceGuid = GetLanguageServiceGuid(textView);

            TextManager = (IVsTextManager6)_globalServiceProvider.GetService(typeof(SVsTextManager));

            HotSettingsCommandFilter commandFilter = new HotSettingsCommandFilter(textView, langServiceGuid, TextManager);
            textViewAdapter.AddCommandFilter(commandFilter, out IOleCommandTarget next);

            commandFilter.Next = next;
        }

        private Guid GetLanguageServiceGuid(IWpfTextView textView)
        {
            // Get the TextBuffer from the TextView.
            IVsTextBuffer textBuffer = EditorAdaptersFactoryService.GetBufferAdapter(textView.TextBuffer);
            // Handle NullReferenceException caused when Diff window (and others?) have no TextBuffer
            if (textBuffer == null) return Guid.Empty;

            // Fetch the language Guid
            textBuffer.GetLanguageServiceID(out Guid langServiceGuid);
            return langServiceGuid;
        }
    }
}
