using System;
using System.Runtime.InteropServices;

namespace WordProSuite.Desktop.Interop
{
    public enum ext_ConnectMode
    {
        AfterStartup = 0,
        Startup = 1,
        External = 2,
        CommandLine = 3,
        Solution = 4,
        UISetup = 5
    }

    public enum ext_DisconnectMode
    {
        HostShutdown = 0,
        UserClosed = 1
    }

    // IDTExtensibility2 is an Automation/IDispatch COM interface.
    // Using InterfaceIsIUnknown here gives Office the wrong v-table layout and
    // can make Word report that the COM add-in caused a serious problem.
    [ComImport]
    [Guid("B65AD801-ABAF-11D0-BB8B-00A0C90F2744")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDTExtensibility2
    {
        [DispId(1)]
        void OnConnection(
            [In, MarshalAs(UnmanagedType.IDispatch)] object application,
            [In] ext_ConnectMode connectMode,
            [In, MarshalAs(UnmanagedType.IDispatch)] object addInInst,
            [In, Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);

        [DispId(2)]
        void OnDisconnection(
            [In] ext_DisconnectMode removeMode,
            [In, Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);

        [DispId(3)]
        void OnAddInsUpdate(
            [In, Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);

        [DispId(4)]
        void OnStartupComplete(
            [In, Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);

        [DispId(5)]
        void OnBeginShutdown(
            [In, Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);
    }

    [ComImport]
    [Guid("000C0396-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IRibbonExtensibility
    {
        [DispId(1)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetCustomUI([In, MarshalAs(UnmanagedType.BStr)] string ribbonId);
    }

    [ComVisible(true)]
    [Guid("815A2449-62EE-4DC8-9C7F-8F56394D2B63")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IWordProRibbonCallbacks
    {
        [DispId(1)]
        void RibbonOnLoad([MarshalAs(UnmanagedType.IDispatch)] object ribbonUi);

        [DispId(2)]
        void RibbonOnAction([MarshalAs(UnmanagedType.IDispatch)] object control);

        [DispId(3)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string RibbonGetStatus([MarshalAs(UnmanagedType.IDispatch)] object control);
    }
}
