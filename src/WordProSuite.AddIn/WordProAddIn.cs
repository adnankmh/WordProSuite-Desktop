using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WordProSuite.Desktop.Commands;
using WordProSuite.Desktop.Infrastructure;
using WordProSuite.Desktop.Interop;
using WordProSuite.Desktop.Ribbon;

namespace WordProSuite.Desktop
{
    [ComVisible(true), Guid("79D9E91D-88D5-4C41-B805-82D64D1348B2"),
     ProgId("WordProSuite.Desktop.AddIn"), ClassInterface(ClassInterfaceType.None),
     ComDefaultInterface(typeof(IWordProRibbonCallbacks))]
    public sealed class WordProAddIn : IDTExtensibility2, IRibbonExtensibility, IWordProRibbonCallbacks
    {
        private object ribbonUi;
        public string GetCustomUI(string ribbonId) { Logger.Info("GetCustomUI: " + ribbonId); return RibbonXml.Value; }
        public void RibbonOnLoad(object ui) { ribbonUi = ui; Logger.Info("Ribbon loaded"); }
        public void RibbonOnAction(object control)
        {
            try { dynamic c = control; CommandRouter.Execute((string)c.Tag); }
            catch (Exception ex) { Logger.Error("Ribbon callback failed", ex); MessageBox.Show(ex.Message, "WordPro Suite"); }
        }
        public string RibbonGetStatus(object control) => WordContext.Application == null ? "غير متصل" : "جاهز";
        public void OnConnection(object application, ext_ConnectMode mode, object addInInst, ref Array custom)
        { WordContext.Application = application; Logger.Info("OnConnection " + mode); }
        public void OnDisconnection(ext_DisconnectMode mode, ref Array custom)
        { Logger.Info("OnDisconnection " + mode); WordContext.Application = null; ribbonUi = null; }
        public void OnAddInsUpdate(ref Array custom) => Logger.Info("OnAddInsUpdate");
        public void OnStartupComplete(ref Array custom) => Logger.Info("OnStartupComplete");
        public void OnBeginShutdown(ref Array custom) => Logger.Info("OnBeginShutdown");
    }
}
