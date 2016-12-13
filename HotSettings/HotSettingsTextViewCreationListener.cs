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
using System.Windows.Input;
using Microsoft.VisualStudio.Shell.Interop;

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
            //System.Windows.Controls.ContextMenu contextMenu = null;
            //lineNumberFrameworkElement.ContextMenu = contextMenu;

            //IWpfTextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            var textViewHost = EditorAdaptersFactoryService.GetWpfTextViewHost(textViewAdapter);
            //var lineNumberMargin = textViewHost.GetTextViewMargin("LineNumber");
            //var glyphMargin = textViewHost.GetTextViewMargin("Glyph");
            //var leftSelectionMargin = textViewHost.GetTextViewMargin("LeftSelection");  // Selection margin - inherited by all left margins
            //var outliningMargin = textViewHost.GetTextViewMargin("Outlining");
            //var spacerMargin = textViewHost.GetTextViewMargin("Spacer");  // Selection margin ?
            var leftMargin = textViewHost.GetTextViewMargin("Left");

            leftMargin.VisualElement.MouseRightButtonUp += OnMouseRightButtonUp;
            //leftSelectionMargin.VisualElement.MouseRightButtonUp += OnMouseRightButtonUp;
            //glyphMargin.VisualElement.MouseRightButtonUp += OnMouseRightButtonUp;
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowContextMenu((FrameworkElement) sender, e);
        }

        private void ShowContextMenu(FrameworkElement sender, MouseButtonEventArgs e)
        {
            const string guidVSPackageContextMenuCmdSet = "c75f116c-9249-4984-8d82-d3c6025afb17";
            const int MyContextMenuId = 0x1100;

            IVsUIShell uiShell;
            System.Guid contextMenuGuid = new System.Guid(guidVSPackageContextMenuCmdSet);
            System.Windows.Point relativePoint;
            System.Windows.Point screenPoint;
            POINTS point;
            POINTS[] points;

            uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;

            if (uiShell != null)
            {
                relativePoint = e.GetPosition(sender);
                screenPoint = sender.PointToScreen(relativePoint);

                point = new POINTS();
                point.x = (short)screenPoint.X;
                point.y = (short)screenPoint.Y;

                points = new[] { point };

                // TODO: error handling
                uiShell.ShowContextMenu(0, ref contextMenuGuid, MyContextMenuId, points, null);
            }
        }

    }
}