using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WordProSuite.Desktop.Commands;
using WordProSuite.Desktop.Infrastructure;
using WordProSuite.Desktop.Interop;
using WordProSuite.Desktop.Ribbon;

namespace WordProSuite.Desktop
{
    [ComVisible(true)]
    [Guid("79D9E91D-88D5-4C41-B805-82D64D1348B2")]
    [ProgId("WordProSuite.Desktop.AddIn")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IWordProRibbonCallbacks))]
    public sealed class WordProAddIn : IDTExtensibility2, IRibbonExtensibility, IWordProRibbonCallbacks
    {
        private object ribbonUi;

        public WordProAddIn()
        {
            // Never throw from a COM class constructor. Office disables an
            // add-in when activation throws before OnConnection.
            try { Logger.Info("COM class constructed"); } catch { }
        }

        public string GetCustomUI(string ribbonId)
        {
            try
            {
                Logger.Info("GetCustomUI: " + (ribbonId ?? "(null)"));
                return RibbonXml.Value;
            }
            catch (Exception ex)
            {
                Logger.Error("GetCustomUI failed", ex);
                return string.Empty;
            }
        }

        public void RibbonOnLoad(object ui)
        {
            try
            {
                ribbonUi = ui;
                Logger.Info("Ribbon loaded");
            }
            catch (Exception ex)
            {
                Logger.Error("RibbonOnLoad failed", ex);
            }
        }

        public void RibbonOnAction(object control)
        {
            try
            {
                dynamic c = control;
                CommandRouter.Execute((string)c.Tag);
            }
            catch (Exception ex)
            {
                Logger.Error("Ribbon callback failed", ex);
                MessageBox.Show(ex.Message, "WordPro Suite");
            }
        }

        public string RibbonGetStatus(object control)
        {
            try { return WordContext.Application == null ? "غير متصل" : "جاهز"; }
            catch { return "غير متصل"; }
        }

        public void OnConnection(object application, ext_ConnectMode mode, object addInInst, ref Array custom)
        {
            try
            {
                WordContext.Application = application;
                Logger.Info("OnConnection " + mode);
            }
            catch (Exception ex)
            {
                Logger.Error("OnConnection failed", ex);
                // Do not rethrow into Word.
            }
        }

        public void OnDisconnection(ext_DisconnectMode mode, ref Array custom)
        {
            try
            {
                Logger.Info("OnDisconnection " + mode);
                WordContext.Application = null;
                ribbonUi = null;
            }
            catch (Exception ex)
            {
                Logger.Error("OnDisconnection failed", ex);
            }
        }

        public void OnAddInsUpdate(ref Array custom)
        {
            try { Logger.Info("OnAddInsUpdate"); } catch { }
        }

        public void OnStartupComplete(ref Array custom)
        {
            try { Logger.Info("OnStartupComplete"); } catch { }
        }

        public void OnBeginShutdown(ref Array custom)
        {
            try { Logger.Info("OnBeginShutdown"); } catch { }
        }
    }
}
