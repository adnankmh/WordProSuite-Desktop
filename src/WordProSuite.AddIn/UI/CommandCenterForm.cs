using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WordProSuite.Desktop.Commands;

namespace WordProSuite.Desktop.UI
{
    internal sealed class CommandCenterForm : Form
    {
        private static CommandCenterForm Instance;
        private readonly TextBox Search = new TextBox();
        private readonly ComboBox Category = new ComboBox();
        private readonly FlowLayoutPanel Cards = new FlowLayoutPanel();

        private CommandCenterForm()
        {
            Text="WordPro Suite Desktop — مركز الأدوات";
            RightToLeft=RightToLeft.Yes;RightToLeftLayout=true;
            StartPosition=FormStartPosition.CenterScreen;
            Width=1100;Height=760;MinimumSize=new Size(820,540);
            Font=new Font("Segoe UI",11f);

            var header=new Panel{Dock=DockStyle.Top,Height=112,Padding=new Padding(20)};
            var title=new Label{Text="WordPro Suite Desktop",Dock=DockStyle.Top,Height=42,
                Font=new Font("Segoe UI",22f,FontStyle.Bold),TextAlign=ContentAlignment.MiddleRight};
            Search.Font=new Font("Segoe UI",14f);Search.Width=520;Search.Height=40;Search.Location=new Point(20,62);
            Search.TextChanged+=(s,e)=>Render();
            Category.DropDownStyle=ComboBoxStyle.DropDownList;Category.Font=new Font("Segoe UI",13f);
            Category.Width=260;Category.Height=40;Category.Location=new Point(560,62);Category.SelectedIndexChanged+=(s,e)=>Render();
            header.Controls.Add(title);header.Controls.Add(Search);header.Controls.Add(Category);

            Cards.Dock=DockStyle.Fill;Cards.AutoScroll=true;Cards.FlowDirection=FlowDirection.RightToLeft;
            Cards.WrapContents=true;Cards.Padding=new Padding(20);
            Controls.Add(Cards);Controls.Add(header);

            Category.Items.Add("كل الأقسام");
            foreach(string c in CommandRouter.All.Select(x=>x.Category).Distinct().OrderBy(x=>x))Category.Items.Add(c);
            Category.SelectedIndex=0;FormClosed+=(s,e)=>Instance=null;Render();
        }

        internal static void ShowCenter()
        {
            if(Instance==null||Instance.IsDisposed)Instance=new CommandCenterForm();
            if(!Instance.Visible)Instance.Show();
            Instance.WindowState=FormWindowState.Normal;Instance.BringToFront();Instance.Activate();
        }

        private void Render()
        {
            if(Category.SelectedItem==null)return;
            string q=Search.Text.Trim(),cat=Category.SelectedItem.ToString();
            IEnumerable<CommandDescriptor> data=CommandRouter.All.Where(x=>x.Id!="command-center").OrderBy(x=>x.Category).ThenBy(x=>x.Title);
            if(cat!="كل الأقسام")data=data.Where(x=>x.Category==cat);
            if(q.Length>0)data=data.Where(x=>x.Title.IndexOf(q,StringComparison.CurrentCultureIgnoreCase)>=0||
                x.Description.IndexOf(q,StringComparison.CurrentCultureIgnoreCase)>=0||x.Id.IndexOf(q,StringComparison.OrdinalIgnoreCase)>=0);
            Cards.SuspendLayout();Cards.Controls.Clear();
            foreach(var c in data)Cards.Controls.Add(Card(c));
            Cards.ResumeLayout();
        }

        private static Control Card(CommandDescriptor c)
        {
            var p=new Panel{Width=320,Height=168,Margin=new Padding(11),Padding=new Padding(15),BorderStyle=BorderStyle.FixedSingle};
            var t=new Label{Text=c.Title,Dock=DockStyle.Top,Height=38,Font=new Font("Segoe UI",14f,FontStyle.Bold),TextAlign=ContentAlignment.MiddleRight};
            var d=new Label{Text=c.Description,Dock=DockStyle.Fill,Font=new Font("Segoe UI",11f),TextAlign=ContentAlignment.TopRight};
            var b=new Button{Text="تشغيل",Dock=DockStyle.Bottom,Height=48,Font=new Font("Segoe UI",13f,FontStyle.Bold),Tag=c.Id};
            b.Click+=(s,e)=>CommandRouter.Execute((string)b.Tag);
            p.Controls.Add(d);p.Controls.Add(b);p.Controls.Add(t);return p;
        }
    }
}
