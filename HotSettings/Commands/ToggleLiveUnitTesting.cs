using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace HotSettings
{
    public class ToggleLiveUnitTesting
    {
        // Constants for LUT commands
        private static Guid LutCmdGroupGuid = new Guid("1E198C22-5980-4E7E-92F3-F73168D1FB63");  // GuidID = 146
        private static readonly uint StartLutCmdId = 16897;
        private static readonly uint StopLutCmdId = 16900;

        public static void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = (OleMenuCommand)sender;
            switch ((uint)command.CommandID.ID)
            {
                case Constants.ToggleLiveUnitTestingCmdId:
                    command.Checked = IsLiveUnitTestingRunning();
                    break;
            }
        }

        public static void ToggleLUT(object sender, EventArgs e)
        {
            // Protect against inconsistent state. Don't toggle if the item is already in the correct state.
            MenuCommand command = (MenuCommand)sender;
            if (!command.Checked && IsLiveUnitTestingRunning()
                || command.Checked && !IsLiveUnitTestingRunning())
            {
                // Already in desired state. Do not toggle.
                return;
            }

            // Now call action to Toggle LUT.
            ToggleLUTRunningState();
        }

        private static bool IsLiveUnitTestingRunning()
        {
            return ShellUtil.IsCommandAvailable("Test.LiveUnitTesting.Stop");
        }

        private static int ToggleLUTRunningState()
        {
            // Call command to Start or Stop LiveUnitTesting depending on current state
            uint cmdID = IsLiveUnitTestingRunning() ? StopLutCmdId : StartLutCmdId;
            return ShellUtil.GetShellCommandDispatcher().Exec(ref LutCmdGroupGuid, cmdID, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);
        }

    }
}
