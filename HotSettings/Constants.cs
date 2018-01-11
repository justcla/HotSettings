using System;

namespace HotSettings
{
    class Constants
    {
        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        // See HotSettings.vsct file for Symdol IDs
        public static readonly Guid HotSettingsCmdSetGuid = new Guid("c75f116c-9249-4984-8d82-d3c6025afb17");

        public const int EditorMarginContextMenuId = 0x1100;

        /// <summary>
        /// Command IDs. Note: These must be in sync with symbols in VSCT file.
        /// </summary>
        public const int ToggleIndicatorMarginCmdId = 0x1021;
        public const int ToggleLineNumbersCmdId = 0x1022;
        public const int ToggleQuickActionsCmdId = 0x1023;
        public const int ToggleSelectionMarginCmdId = 0x1024;
        public const int ToggleTrackChangesCmdId = 0x1025;
        public const int ToggleDiffMarginCmdId = 0x1026;
        public const int ToggleOutliningCmdId = 0x1027;
        public const int ToggleLiveUnitTestingCmdId = 0x1028;
        public const int ToggleAnnotateCmdId = 0x1029;
        // Editor Settings CmdIds
        public const int ToggleNavigationBarCmdId = 0x1041;
        public const int ToggleCodeLensCmdId = 0x1042;
        public const int ToggleStructureGuideLinesCmdId = 0x1043;
        public const int ToggleHighlightCurrentLineCmdId = 0x1050;
        public const int ToggleAutoDelimiterHighlightingCmdId = 0x1051;
        public const int ToggleProcedureLineSeparatorCmdId = 0x1052;
        public const int ToggleIntelliSensePopUpCmdId = 0x1053;
        public const int ToggleLineEndingsCmdId = 0x1054;
        public const int ToggleHighlightSymbolsCmdId = 0x1055;
        public const int ToggleHighlightKeywordsCmdId = 0x1056;
        public const int ToggleIntelliSenseSquigglesCmdId = 0x1057;
        // Scrollbar Settings CmdIds
        public const int ToggleShowScrollbarMarkersCmdId = 0x1070;
        public const int ToggleShowChangesCmdId = 0x1071;
        public const int ToggleShowMarksCmdId = 0x1072;
        public const int ToggleShowErrorsCmdId = 0x1073;
        public const int ToggleShowCaretPositionCmdId = 0x1074;
        public const int ToggleShowDiffsCmdId = 0x1080;
        // Distraction free mode CmdIds
        public const int ToggleCleanEditorCmdId = 0x1110;
        public const int ToggleCleanMarginsCmdId = 0x1120;

        // Solution Explorer Commands
        public const int ToggleTrackActiveItemCmdId = 0x1210;

    }
}
