using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotSettings
{
    class Constants
    {
        //public static readonly Guid HotCommandsGuid = new Guid("1023dc3d-550c-46b8-a3ec-c6b03431642c");
        //public const uint DuplicateSelectionCmdId = 0x1019;
        //public const uint DuplicateSelectionReverseCmdId = 0x1020;
        //public const uint ToggleCommentCmdId = 0x1021;
        //public const uint ExpandSelectionCmdId = 0x1022;
        //public const uint ShrinkSelectionCmdId = 0x1023;
        //public const uint FormatCodeCmdId = 0x1027;
        //public const uint MoveMemberUpCmdId = 0x1031;
        //public const uint MoveMemberDownCmdId = 0x1032;
        //public const uint GoToPreviousMemberCmdId = 0x1033;
        //public const uint GoToNextMemberCmdId = 0x1034;
        //public const uint JoinLinesCmdId = 0x1040;

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
        public static Guid LutCmdGroupGuid = new Guid("1E198C22-5980-4E7E-92F3-F73168D1FB63");  // GuidID = 146
        public const uint StartLutCmdId = 16897;
        public const uint StopLutCmdId = 16900;
    }
}
