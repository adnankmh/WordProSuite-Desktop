using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WordProSuite.Desktop.Commands;
using WordProSuite.Desktop.Licensing;

namespace WordProSuite.Desktop.UI
{
    internal sealed class CommandCenterForm : Form
    {
        private static CommandCenterForm Instance;
        private readonly TextBox SearchBox = new TextBox();
        private readonly ComboBox CategoryBox = new ComboBox();
        private readonly CheckBox FavoritesOnly = new CheckBox();
        private readonly CheckBox RecentOnly = new CheckBox();
        private readonly FlowLayoutPanel Cards = new FlowLayoutPanel();
        private readonly Label ResultCount = new Label();
        private readonly Label LicenseStatus = new Label();

        private CommandCenterForm()
        {
            Text = "WordPro Suite Desktop Pro — مركز الأدوات";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1240;
            Height = 820;
            MinimumSize = new Size(900, 600);
            Font = new Font("Segoe UI", 10.5f);
            KeyPreview = true;
            BackColor = Color.FromArgb(245, 247, 250);

            var hero = new Panel { Dock = DockStyle.Top, Height = 112, BackColor = Color.FromArgb(22, 53, 79), Padding = new Padding(24, 16, 24, 12) };
            var title = new Label
            {
                Text = "WordPro Suite Desktop Pro",
                Dock = DockStyle.Top,
                Height = 42,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };
            var activation = new Button
            {
                Text = "تفعيل / إدارة الترخيص",
                Dock = DockStyle.Left,
                Width = 190,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(34, 167, 132),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold)
            };
            activation.FlatAppearance.BorderSize = 0;
            activation.Click += (s, e) => { ActivationForm.ShowActivation(); RefreshLicense(); };
            LicenseStatus.Dock = DockStyle.Fill;
            LicenseStatus.ForeColor = Color.FromArgb(218, 231, 241);
            LicenseStatus.TextAlign = ContentAlignment.MiddleRight;
            LicenseStatus.Font = new Font("Segoe UI", 10.5f);
            var statusPanel = new Panel { Dock = DockStyle.Fill };
            statusPanel.Controls.Add(LicenseStatus);
            statusPanel.Controls.Add(activation);
            hero.Controls.Add(statusPanel);
            hero.Controls.Add(title);

            var filters = new Panel { Dock = DockStyle.Top, Height = 92, Padding = new Padding(24, 20, 24, 12), BackColor = Color.White };
            var searchLabel = new Label { Text = "بحث", Dock = DockStyle.Right, Width = 55, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
            SearchBox.Dock = DockStyle.Right;
            SearchBox.Width = 430;
            SearchBox.Font = new Font("Segoe UI", 13f);
            SearchBox.TextChanged += (s, e) => Render();
            SearchBox.KeyDown += SearchKeyDown;

            CategoryBox.DropDownStyle = ComboBoxStyle.DropDownList;
            CategoryBox.Dock = DockStyle.Right;
            CategoryBox.Width = 245;
            CategoryBox.Font = new Font("Segoe UI", 11.5f);
            CategoryBox.SelectedIndexChanged += (s, e) => Render();

            FavoritesOnly.Text = "المفضلة فقط";
            FavoritesOnly.Dock = DockStyle.Right;
            FavoritesOnly.Width = 125;
            FavoritesOnly.CheckedChanged += (s, e) => { if (FavoritesOnly.Checked) RecentOnly.Checked = false; Render(); };

            RecentOnly.Text = "آخر استخدام";
            RecentOnly.Dock = DockStyle.Right;
            RecentOnly.Width = 125;
            RecentOnly.CheckedChanged += (s, e) => { if (RecentOnly.Checked) FavoritesOnly.Checked = false; Render(); };

            ResultCount.Dock = DockStyle.Fill;
            ResultCount.TextAlign = ContentAlignment.MiddleLeft;
            ResultCount.ForeColor = Color.DimGray;

            filters.Controls.Add(ResultCount);
            filters.Controls.Add(RecentOnly);
            filters.Controls.Add(FavoritesOnly);
            filters.Controls.Add(CategoryBox);
            filters.Controls.Add(SearchBox);
            filters.Controls.Add(searchLabel);

            Cards.Dock = DockStyle.Fill;
            Cards.AutoScroll = true;
            Cards.FlowDirection = FlowDirection.RightToLeft;
            Cards.WrapContents = true;
            Cards.Padding = new Padding(18);
            Cards.BackColor = Color.FromArgb(245, 247, 250);

            Controls.Add(Cards);
            Controls.Add(filters);
            Controls.Add(hero);

            CategoryBox.Items.Add("كل الأقسام");
            foreach (string category in CommandRouter.All.Select(x => x.Category).Distinct().OrderBy(x => x)) CategoryBox.Items.Add(category);
            CategoryBox.SelectedIndex = 0;

            Shown += (s, e) => { RefreshLicense(); SearchBox.Focus(); Render(); };
            FormClosed += (s, e) => Instance = null;
            KeyDown += FormKeyDown;
        }

        internal static void ShowCenter()
        {
            if (Instance == null || Instance.IsDisposed) Instance = new CommandCenterForm();
            if (!Instance.Visible) Instance.Show();
            Instance.WindowState = FormWindowState.Normal;
            Instance.BringToFront();
            Instance.Activate();
        }

        private void RefreshLicense()
        {
            LicenseStatus.Text = LicenseManager.StatusText + "    |    معرّف الجهاز: " + LicenseManager.MachineId;
        }

        private void Render()
        {
            if (CategoryBox.SelectedItem == null) return;
            string query = SearchBox.Text.Trim();
            string category = Convert.ToString(CategoryBox.SelectedItem);
            IEnumerable<CommandDescriptor> data = CommandRouter.All.Where(x => x.Id != "command-center");

            if (category != "كل الأقسام") data = data.Where(x => x.Category == category);
            if (FavoritesOnly.Checked) data = data.Where(x => CommandUsageStore.IsFavorite(x.Id));
            if (RecentOnly.Checked)
            {
                var order = CommandUsageStore.Recent.Select((id, index) => new { id, index })
                    .GroupBy(x => x.id, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(x => x.Key, x => x.Min(v => v.index), StringComparer.OrdinalIgnoreCase);
                data = data.Where(x => order.ContainsKey(x.Id)).OrderBy(x => order[x.Id]);
            }
            else data = data.OrderBy(x => x.Category).ThenBy(x => x.Title);

            if (query.Length > 0)
            {
                data = data.Where(x =>
                    Contains(x.Title, query) || Contains(x.Description, query) || Contains(x.Category, query) ||
                    Contains(x.Keywords, query) || Contains(x.Id, query));
            }

            var list = data.ToList();
            ResultCount.Text = list.Count + " أداة";
            Cards.SuspendLayout();
            Cards.Controls.Clear();
            foreach (var command in list) Cards.Controls.Add(CreateCard(command));
            if (list.Count == 0)
            {
                Cards.Controls.Add(new Label
                {
                    Text = "لا توجد أدوات مطابقة. جرّب كلمة بحث أخرى.",
                    Width = 700,
                    Height = 90,
                    Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                    ForeColor = Color.DimGray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(40)
                });
            }
            Cards.ResumeLayout();
        }

        private Control CreateCard(CommandDescriptor command)
        {
            var panel = new Panel
            {
                Width = 350,
                Height = 190,
                Margin = new Padding(10),
                Padding = new Padding(14),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var category = new Label
            {
                Text = command.Category,
                Dock = DockStyle.Top,
                Height = 24,
                ForeColor = Color.FromArgb(34, 126, 155),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };
            var title = new Label
            {
                Text = command.Title,
                Dock = DockStyle.Top,
                Height = 38,
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };
            var description = new Label
            {
                Text = command.Description,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(70, 70, 70),
                TextAlign = ContentAlignment.TopRight,
                Padding = new Padding(0, 4, 0, 4)
            };
            var actions = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            var run = new Button
            {
                Text = "تشغيل",
                Dock = DockStyle.Right,
                Width = 175,
                Height = 42,
                Tag = command.Id,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(22, 101, 144),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold)
            };
            run.FlatAppearance.BorderSize = 0;
            run.Click += RunClick;
            var favorite = new Button
            {
                Text = CommandUsageStore.IsFavorite(command.Id) ? "★ مفضلة" : "☆ إضافة للمفضلة",
                Dock = DockStyle.Left,
                Width = 145,
                Height = 42,
                Tag = command.Id,
                FlatStyle = FlatStyle.Flat
            };
            favorite.Click += (s, e) => { CommandUsageStore.ToggleFavorite(Convert.ToString(((Control)s).Tag)); Render(); };
            actions.Controls.Add(run);
            actions.Controls.Add(favorite);
            panel.Controls.Add(description);
            panel.Controls.Add(actions);
            panel.Controls.Add(title);
            panel.Controls.Add(category);
            return panel;
        }

        private void RunClick(object sender, EventArgs e)
        {
            string id = Convert.ToString(((Control)sender).Tag);
            CommandRouter.Execute(id);
            RefreshLicense();
            if (RecentOnly.Checked || FavoritesOnly.Checked) Render();
        }

        private static bool Contains(string source, string query) =>
            (source ?? "").IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0;

        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F) { SearchBox.Focus(); SearchBox.SelectAll(); e.Handled = true; }
            if (e.KeyCode == Keys.Escape) Close();
        }

        private void SearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var first = CommandRouter.All.FirstOrDefault(x =>
                    Contains(x.Title, SearchBox.Text.Trim()) || Contains(x.Keywords, SearchBox.Text.Trim()));
                if (first != null) CommandRouter.Execute(first.Id);
                e.Handled = true;
            }
        }
    }
}
