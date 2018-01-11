using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.TextManager.Interop
{
    [TypeIdentifier("96B36253-76A4-4DF5-9071-34CD1B5A5EFF", "Microsoft.VisualStudio.TextManager.Interop.VIEWPREFERENCES5")]
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct VIEWPREFERENCES5
    {
        public uint fVisibleWhitespace;
        public uint fSelectionMargin;
        public uint fAutoDelimiterHighlight;
        public uint fGoToAnchorAfterEscape;
        public uint fDragDropEditing;
        public uint fUndoCaretMovements;
        public uint fOvertype;
        public uint fDragDropMove;
        public uint fWidgetMargin;
        public uint fReadOnly;
        public uint fActiveInModalState;
        public uint fClientDragDropFeedback;
        public uint fTrackChanges;
        public uint uCompletorSize;
        public uint fDetectUTF8;
        public int lEditorEmulation;
        public uint fHighlightCurrentLine;
        public uint fShowBlockStructure;
        public uint fEnableCodingConventions;
        public uint fEnableClickGotoDef;
        public uint uModifierKey;
        public uint fOpenDefInPeek;
    }
}