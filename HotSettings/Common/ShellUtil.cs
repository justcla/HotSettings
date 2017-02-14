using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;

namespace HotSettings
{
    public class ShellUtil
    {
        /// <summary>
        /// Get the SUIHostCommandDispatcher from the global service provider.
        /// </summary>
        public static IOleCommandTarget GetShellCommandDispatcher()
        {
            return ServiceProvider.GlobalProvider.GetService(typeof(SUIHostCommandDispatcher)) as IOleCommandTarget;
        }

        public static TInterface GetGlobalService<TService, TInterface>()
            where TService : class
            where TInterface : class
        => (TInterface)ServiceProvider.GlobalProvider.GetService(typeof(TService));

        public static DTE GetDTE()
            => GetGlobalService<SDTE, DTE>();

        public static bool IsCommandAvailable(string commandName)
            => GetDTE().Commands.Item(commandName).IsAvailable;

        public static void ExecuteCommand(string commandName, string args = "")
            => GetDTE().ExecuteCommand(commandName, args);

    }
}
