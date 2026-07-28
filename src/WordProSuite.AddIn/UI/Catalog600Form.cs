using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WordProSuite.Desktop.Commands;

namespace WordProSuite.Desktop.UI
{
    internal sealed class Catalog600Form : Form
    {
        private static Catalog600Form Instance;
        private readonly TextBox SearchBox = new TextBox();
        private readonly ComboBox EngineBox = new ComboBox();
        private readonly ListView List = new ListView();
        private readonly Label Summary = new Label();
        private Dictionary<string, CommandDescriptor> Commands;

        private Catalog600Form()
        {
            Text = "Ultra Word Suite 600 — الموسوعة الاحترافية";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1260;
            Height = 820;
            MinimumSize = new Size(900, 600);
            Font = new Font("Segoe UI", 10.5f);

            Commands = CommandRouter.All.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

            var header = new Panel { Dock = DockStyle.Top, Height = 132, Padding = new Padding(20) };
            var title = new Label
            {
                Text = "موسوعة 600 أداة — 15 محركًا تخصصيًا",
                Dock = DockStyle.Top,
                Height = 46,
                Font = new Font("Segoe UI", 21f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            SearchBox.Width = 520;
            SearchBox.Height = 38;
            SearchBox.Font = new Font("Segoe UI", 13f);
            SearchBox.Location = new Point(20, 70);
            SearchBox.TextChanged += delegate { Render(); };

            EngineBox.Width = 420;
            EngineBox.Height = 38;
            EngineBox.Font = new Font("Segoe UI", 12f);
            EngineBox.DropDownStyle = ComboBoxStyle.DropDownList;
            EngineBox.Location = new Point(570, 70);
            EngineBox.Items.Add("كل المحركات");
            foreach (string engine in FeatureReferenceCatalog.All.Select(x => x.Engine).Distinct()) EngineBox.Items.Add(engine);
            EngineBox.SelectedIndex = 0;
            EngineBox.SelectedIndexChanged += delegate { Render(); };

            Summary.Dock = DockStyle.Bottom;
            Summary.Height = 28;
            Summary.TextAlign = ContentAlignment.MiddleRight;
            Summary.ForeColor = Color.DimGray;

            header.Controls.Add(title);
            header.Controls.Add(SearchBox);
            header.Controls.Add(EngineBox);
            header.Controls.Add(Summary);

            List.Dock = DockStyle.Fill;
            List.View = View.Details;
            List.FullRowSelect = true;
            List.GridLines = true;
            List.HideSelection = false;
            List.Columns.Add("الرقم", 70, HorizontalAlignment.Center);
            List.Columns.Add("المحرك", 300, HorizontalAlignment.Right);
            List.Columns.Add("الأداة", 310, HorizontalAlignment.Right);
            List.Columns.Add("الوصف", 490, HorizontalAlignment.Right);
            List.DoubleClick += delegate { RunSelected(); };

            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(14)
            };
            var run = new Button { Text = "تشغيل الأداة", Width = 180, Height = 42, Font = new Font("Segoe UI", 11.5f, FontStyle.Bold) };
            var commandCenter = new Button { Text = "مركز الأدوات", Width = 160, Height = 42 };
            var close = new Button { Text = "إغلاق", Width = 120, Height = 42 };
            run.Click += delegate { RunSelected(); };
            commandCenter.Click += delegate { CommandCenterForm.ShowCenter(); };
            close.Click += delegate { Close(); };
            footer.Controls.Add(run);
            footer.Controls.Add(commandCenter);
            footer.Controls.Add(close);

            Controls.Add(List);
            Controls.Add(footer);
            Controls.Add(header);
            FormClosed += delegate { Instance = null; };
            Render();
        }

        internal static void ShowCenter()
        {
            if (Instance == null || Instance.IsDisposed) Instance = new Catalog600Form();
            if (!Instance.Visible) Instance.Show();
            Instance.WindowState = FormWindowState.Normal;
            Instance.BringToFront();
            Instance.Activate();
        }

        private void Render()
        {
            if (EngineBox.SelectedItem == null) return;
            string engine = Convert.ToString(EngineBox.SelectedItem);
            string query = SearchBox.Text.Trim();
            IEnumerable<FeatureReference> source = FeatureReferenceCatalog.All;
            if (!String.Equals(engine, "كل المحركات", StringComparison.Ordinal)) source = source.Where(x => x.Engine == engine);

            if (query.Length > 0)
            {
                source = source.Where(reference =>
                {
                    CommandDescriptor command;
                    if (!Commands.TryGetValue(reference.CommandId, out command)) return false;
                    return command.Title.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                           command.Description.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                           command.Keywords.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                           reference.Number.ToString().Contains(query);
                });
            }

            List.BeginUpdate();
            List.Items.Clear();
            foreach (FeatureReference reference in source.OrderBy(x => x.Number))
            {
                CommandDescriptor command;
                if (!Commands.TryGetValue(reference.CommandId, out command)) continue;
                var item = new ListViewItem(reference.Number.ToString("000"));
                item.SubItems.Add(reference.Engine);
                item.SubItems.Add(command.Title);
                item.SubItems.Add(command.Description);
                item.Tag = reference.CommandId;
                List.Items.Add(item);
            }
            List.EndUpdate();
            Summary.Text = "المعروض: " + List.Items.Count + " من 600 أداة";
        }

        private void RunSelected()
        {
            if (List.SelectedItems.Count == 0) return;
            string commandId = Convert.ToString(List.SelectedItems[0].Tag);
            CommandRouter.Execute(commandId);
        }
    }
}
