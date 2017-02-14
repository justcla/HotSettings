using System;

namespace HotSettings
{
    class Constants
    {
        // See HotSettings.vsct file for Symdol IDs
        public static readonly Guid HotSettingsCmdSetGuid = new Guid("c75f116c-9249-4984-8d82-d3c6025afb17");
        public const int EditorMarginContextMenuId = 0x1100;
        public const int ToggleIndicatorMarginCmdId = 0x1021;
        public const int ToggleLineNumbersCmdId = 0x1022;
        public const int ToggleQuickActionsCmdId = 0x1023;
        public const int ToggleSelectionMarginCmdId = 0x1024;
        public const int ToggleTrackChangesCmdId = 0x1025;
        public const int ToggleDiffMarginCmdId = 0x1026;
        public const int ToggleOutliningCmdId = 0x1027;
        public const int ToggleLiveUnitTestingCmdId = 0x1028;       // Duplicate of below
        public const int ToggleLUTCmdId = 0x1028;
        public const int ToggleAnnotateCmdId = 0x1029;

        // Predefined LUT CmdIDs
        //public static Guid LutCmdGroupGuid = new Guid("1E198C22-5980-4E7E-92F3-F73168D1FB63");  // GuidID = 146
        //public const uint StartLutCmdId = 16897;
        //public const uint StopLutCmdId = 16900;
    }
}
