using System;
using System.Runtime.InteropServices;

namespace WordProSuite.Desktop.Interop
{
    public enum ext_ConnectMode { AfterStartup=0, Startup=1, External=2, CommandLine=3, Solution=4, UISetup=5 }
    public enum ext_DisconnectMode { HostShutdown=0, UserClosed=1 }

    [ComImport, Guid("B65AD801-ABAF-11D0-BB8B-00A0C90F2744"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDTExtensibility2
    {
        void OnConnection([MarshalAs(UnmanagedType.IDispatch)] object application, ext_ConnectMode connectMode,
            [MarshalAs(UnmanagedType.IDispatch)] object addInInst, ref Array custom);
        void OnDisconnection(ext_DisconnectMode removeMode, ref Array custom);
        void OnAddInsUpdate(ref Array custom);
        void OnStartupComplete(ref Array custom);
        void OnBeginShutdown(ref Array custom);
    }

    [ComImport, Guid("000C0396-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IRibbonExtensibility
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetCustomUI([MarshalAs(UnmanagedType.BStr)] string ribbonId);
    }

    [ComVisible(true), Guid("815A2449-62EE-4DC8-9C7F-8F56394D2B63"),
     InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IWordProRibbonCallbacks
    {
        [DispId(1)] void RibbonOnLoad([MarshalAs(UnmanagedType.IDispatch)] object ribbonUi);
        [DispId(2)] void RibbonOnAction([MarshalAs(UnmanagedType.IDispatch)] object control);
        [DispId(3)] string RibbonGetStatus([MarshalAs(UnmanagedType.IDispatch)] object control);
    }
}
