using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Operations;
using System.Windows;
#pragma warning disable 0649

namespace HotCommands
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal sealed class CommandFilterTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        private IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        private IClassifierAggregatorService _aggregatorFactory;

        [Import]
        private SVsServiceProvider _globalServiceProvider;

        [Import(typeof(IEditorOperationsFactoryService))]
        private IEditorOperationsFactoryService _editorOperationsFactory;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            //IWpfTextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            var textViewHost = EditorAdaptersFactoryService.GetWpfTextViewHost(textViewAdapter);
            var lineNumberMargin = textViewHost.GetTextViewMargin("LineNumber");
            var glyphMargin = textViewHost.GetTextViewMargin("Glyph");
            var leftSelectionMargin = textViewHost.GetTextViewMargin("LeftSelection");  // Selection margin - inherited by all left margins
            var outliningMargin = textViewHost.GetTextViewMargin("Outlining");
            var spacerMargin = textViewHost.GetTextViewMargin("Spacer");  // Selection margin ?
            var leftMargin = textViewHost.GetTextViewMargin("Left");
            FrameworkElement frameworkElement = lineNumberMargin.VisualElement;
            //frameworkElement.AddHandler(routedEvent, handler);
        }
    }
}