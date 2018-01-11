using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.TextManager.Interop
{
    [Guid("A50CF306-7BEE-4349-8789-DAE896A15E07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IVsTextManager6
    {
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int GetUserPreferences6([MarshalAs(UnmanagedType.LPArray)] [Out] VIEWPREFERENCES5[] pViewPrefs, [MarshalAs(UnmanagedType.LPArray)] [In] [Out] LANGPREFERENCES3[] pLangPrefs, [MarshalAs(UnmanagedType.LPArray)] [In] [Out] FONTCOLORPREFERENCES2[] pColorPrefs);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int SetUserPreferences6([MarshalAs(UnmanagedType.LPArray)] [In] VIEWPREFERENCES5[] pViewPrefs, [MarshalAs(UnmanagedType.LPArray)] [In] LANGPREFERENCES3[] pLangPrefs, [MarshalAs(UnmanagedType.LPArray)] [In] FONTCOLORPREFERENCES2[] pColorPrefs);
    }
}