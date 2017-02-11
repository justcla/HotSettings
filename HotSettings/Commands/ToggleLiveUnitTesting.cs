using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;

namespace HotSettings
{
    public class ToggleLiveUnitTesting
    {
        // Constants for LUT commands
        private static Guid LutCmdGroupGuid = new Guid("1E198C22-5980-4E7E-92F3-F73168D1FB63");  // GuidID = 146
        private static readonly uint StartLutCmdId = 16897;
        private static readonly uint StopLutCmdId = 16900;

        public static void ToggleLUT(object sender, EventArgs e)
        {
            MenuCommand command = (MenuCommand)sender;

            // Protect against inconsistent state. Don't toggle if the item is already in the correct state.
            if (!command.Checked && !IsLiveUnitTestingRunning()
                || command.Checked && IsLiveUnitTestingRunning())
            {
                ToggleLUTRunningState(command);
            }

            // Update state of checkbox
            command.Checked = !command.Checked;  //IsLiveUnitTestingRunning();  - This happens too slowly to be effective.
        }

        private static bool IsLiveUnitTestingRunning()
        {
            return DTEUtil.IsCommandAvailable("Test.LiveUnitTesting.Stop");
        }

        private static int ToggleLUTRunningState(MenuCommand command)
        {
            // Call command to Start or Stop LiveUnitTesting depending on current state

            uint cmdID = IsLiveUnitTestingRunning() ? StopLutCmdId : StartLutCmdId;
            //uint cmdID = command.Checked ? stopLutCmdId : startLutCmdId;

            int hr = DTEUtil.GetShellCommandDispatcher().Exec(ref LutCmdGroupGuid, cmdID, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);

            return VSConstants.S_OK;
        }

    }
}
