using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WordProSuite.SetupLauncher
{
    internal enum OfficeArchitecture { Unknown, X86, X64 }
    internal static class Program
    {
        [STAThread] private static void Main()
        {
            Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SetupForm());
        }
    }

    internal sealed class SetupForm : Form
    {
        private readonly Label Status=new Label();
        internal SetupForm()
        {
            Text="WordPro Suite Desktop Setup";RightToLeft=RightToLeft.Yes;RightToLeftLayout=true;
            StartPosition=FormStartPosition.CenterScreen;ClientSize=new Size(640,380);
            FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;Font=new Font("Segoe UI",11f);
            var title=new Label{Text="WordPro Suite Desktop",Font=new Font("Segoe UI",23f,FontStyle.Bold),TextAlign=ContentAlignment.MiddleCenter,Dock=DockStyle.Top,Height=78};
            var desc=new Label{Text="مثبت Microsoft Word لنسختَي Office ‏32-bit و64-bit.\nلا يستخدم Node.js أو localhost أو نوافذ أوامر.",TextAlign=ContentAlignment.MiddleCenter,Dock=DockStyle.Top,Height=72};
            Status.TextAlign=ContentAlignment.MiddleCenter;Status.Dock=DockStyle.Top;Status.Height=72;Status.Font=new Font("Segoe UI",12f,FontStyle.Bold);
            var buttons=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.RightToLeft,Padding=new Padding(50),WrapContents=false};
            buttons.Controls.Add(Button("تثبيت","/i"));buttons.Controls.Add(Button("إصلاح","/fa"));buttons.Controls.Add(Button("إزالة","/x"));
            Controls.Add(buttons);Controls.Add(Status);Controls.Add(desc);Controls.Add(title);Shown+=(s,e)=>RefreshStatus();
        }
        private Button Button(string text,string op)
        {
            var b=new Button{Text=text,Width=155,Height=58,Margin=new Padding(10),Font=new Font("Segoe UI",13f,FontStyle.Bold)};
            b.Click+=(s,e)=>RunMsi(op);return b;
        }
        private void RefreshStatus()
        {
            var a=Detect();
            Status.Text=a==OfficeArchitecture.Unknown?"لم يتم تحديد معمارية Office تلقائيًا؛ سيُطلب منك الاختيار.":"تم اكتشاف Office: "+(a==OfficeArchitecture.X64?"64-bit":"32-bit");
        }
        private void RunMsi(string op)
        {
            try
            {
                var a=Detect();if(a==OfficeArchitecture.Unknown)a=Ask();if(a==OfficeArchitecture.Unknown)return;
                string file=a==OfficeArchitecture.X64?"WordProSuite.Desktop.x64.msi":"WordProSuite.Desktop.x86.msi";
                string path=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Installers",file);
                if(!File.Exists(path))throw new FileNotFoundException("لم يتم العثور على ملف التثبيت.",path);
                var p=Process.Start(new ProcessStartInfo{FileName="msiexec.exe",Arguments=op+" \""+path+"\" /passive",UseShellExecute=true});
                p.WaitForExit();
                bool ok=p.ExitCode==0||p.ExitCode==3010;
                MessageBox.Show(ok?"اكتملت العملية بنجاح. أغلق Word وافتحه من جديد.":"انتهت العملية برمز: "+p.ExitCode,"WordPro Suite",MessageBoxButtons.OK,ok?MessageBoxIcon.Information:MessageBoxIcon.Warning);
            }
            catch(Exception ex){MessageBox.Show(ex.Message,"WordPro Suite",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        }
        private static OfficeArchitecture Ask()
        {
            var r=MessageBox.Show("هل نسخة Microsoft Office لديك 64-bit؟\n\nنعم لـ64-bit، ولا لـ32-bit.","معمارية Office",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
            return r==DialogResult.Yes?OfficeArchitecture.X64:r==DialogResult.No?OfficeArchitecture.X86:OfficeArchitecture.Unknown;
        }
        private static OfficeArchitecture Detect()
        {
            foreach(var view in new[]{RegistryView.Registry64,RegistryView.Registry32})
            using(var baseKey=RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,view))
            using(var key=baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
            {
                string p=key?.GetValue("Platform") as string;
                if(String.Equals(p,"x64",StringComparison.OrdinalIgnoreCase))return OfficeArchitecture.X64;
                if(String.Equals(p,"x86",StringComparison.OrdinalIgnoreCase))return OfficeArchitecture.X86;
            }
            return OfficeArchitecture.Unknown;
        }
    }
}
