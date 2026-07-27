using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WordProSuite.Desktop.Commands
{
    internal static class CommandUsageStore
    {
        private const string Path = @"Software\WordProSuite\Desktop\Usage";

        internal static HashSet<string> Favorites
        {
            get
            {
                string value = Read("Favorites");
                return new HashSet<string>((value ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries), StringComparer.OrdinalIgnoreCase);
            }
        }

        internal static IList<string> Recent
        {
            get
            {
                string value = Read("Recent");
                return (value ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Take(20).ToList();
            }
        }

        internal static bool IsFavorite(string id) => Favorites.Contains(id);

        internal static void ToggleFavorite(string id)
        {
            var favorites = Favorites;
            if (!favorites.Add(id)) favorites.Remove(id);
            Write("Favorites", String.Join(";", favorites.OrderBy(x => x)));
        }

        internal static void RecordRun(string id)
        {
            var list = Recent.Where(x => !String.Equals(x, id, StringComparison.OrdinalIgnoreCase)).ToList();
            list.Insert(0, id);
            Write("Recent", String.Join(";", list.Take(20)));
        }

        private static string Read(string name)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(Path)) return Convert.ToString(key == null ? null : key.GetValue(name));
        }

        private static void Write(string name, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(Path)) key.SetValue(name, value ?? "", RegistryValueKind.String);
        }
    }
}
