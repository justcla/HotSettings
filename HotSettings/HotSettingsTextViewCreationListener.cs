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
using HotSettings;

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
            // Add the Right-Click context menu to the Left Margin of the Editor window
            AddEditorMarginContextMenu(textViewAdapter);

            // Add a Command Filter for ToggleLUT Command (so that we can access QueryStatus)
            AddCommandFilter(textViewAdapter);
        }

        private void AddCommandFilter(IVsTextView textViewAdapter)
        {
            //IWpfTextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            ToggleMarginCommandTarget toggleMarginCommand = new ToggleMarginCommandTarget(_globalServiceProvider);
            IOleCommandTarget next;
            textViewAdapter.AddCommandFilter(toggleMarginCommand, out next);
            toggleMarginCommand.Next = next;
        }

        private void AddEditorMarginContextMenu(IVsTextView textViewAdapter)
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
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowContextMenu((FrameworkElement)sender, e);
        }

        private void ShowContextMenu(FrameworkElement frameworkElement, MouseButtonEventArgs mouseButtonEvent)
        {
            const string guidVSPackageContextMenuCmdSet = "c75f116c-9249-4984-8d82-d3c6025afb17";
            //const string guidVSPackageContextMenuCmdSet = HotSettings.Constants.HotSettingsCmdSetGuid.ToString();
            const int MyContextMenuId = HotSettings.Constants.EditorMarginContextMenuId; //  0x1100;

            IVsUIShell uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (uiShell == null)
            {
                // TODO: Log error - Unable to access UIShell
                return;
            }

            System.Guid contextMenuGuid = new System.Guid(guidVSPackageContextMenuCmdSet);
            POINTS[] points = GetPointsFromMouseEvent(frameworkElement, mouseButtonEvent);

            // TODO: error handling
            uiShell.ShowContextMenu(0, ref contextMenuGuid, MyContextMenuId, points, null);
        }

        private static POINTS[] GetPointsFromMouseEvent(FrameworkElement frameworkElement, MouseButtonEventArgs mouseButtonEvent)
        {
            Point relativePoint = mouseButtonEvent.GetPosition(frameworkElement);
            Point screenPoint = frameworkElement.PointToScreen(relativePoint);
            POINTS point = new POINTS();
            point.x = (short)screenPoint.X;
            point.y = (short)screenPoint.Y;
            return new[] { point };
        }

    }
}