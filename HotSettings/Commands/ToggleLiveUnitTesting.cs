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
        //private static Guid LutCmdGroupGuid = new Guid("1E198C22-5980-4E7E-92F3-F73168D1FB63");  // GuidID = 146
        //private static readonly uint StartLutCmdId = 16897;
        //private static readonly uint StopLutCmdId = 16900;

        public static void ToggleLUT(object sender, EventArgs e)
        {
            MenuCommand command = (MenuCommand)sender;

            // Protect against inconsistent state. Don't toggle if the item is already in the correct state.
            if (!command.Checked && !IsLiveUnitTestingRunning()
                || command.Checked && IsLiveUnitTestingRunning())
            {
                ToggleLUTRunningState();
            }

            // Update state of checkbox
            command.Checked = !command.Checked;  //IsLiveUnitTestingRunning();  - This happens too slowly to be effective.
        }

        public static int ToggleLUTRunningState()
        {
            // Call command to Start or Stop LiveUnitTesting depending on current state

            uint cmdID = IsLiveUnitTestingRunning() ? Constants.StopLutCmdId : Constants.StartLutCmdId;
            //uint cmdID = command.Checked ? stopLutCmdId : startLutCmdId;

            int hr = ShellUtil.GetShellCommandDispatcher().Exec(ref Constants.LutCmdGroupGuid, cmdID, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);

            return VSConstants.S_OK;
        }

        public static bool IsLiveUnitTestingRunning()
        {
            //return IsCommandAvailable(Constants.LutCmdGroupGuid, Constants.StopLutCmdId);
            return ShellUtil.IsCommandAvailableDTE("Test.LiveUnitTesting.Stop");
        }

        private static bool IsCommandAvailable(Guid cmdGroupGuid, uint theCmd)
        {
            OLECMD oleCmd = new OLECMD() { cmdID = theCmd, cmdf = 0 };
            OLECMD[] prgCmds = new OLECMD[] { oleCmd };
            IntPtr pCmdText = default(IntPtr);
            int hr = ShellUtil.GetShellCommandDispatcher().QueryStatus(ref cmdGroupGuid, 1, prgCmds, pCmdText);
            bool isEnabled = (oleCmd.cmdf & (uint)OLECMDF.OLECMDF_ENABLED) != 0;
            return hr == VSConstants.S_OK && isEnabled;
        }

    }
}
