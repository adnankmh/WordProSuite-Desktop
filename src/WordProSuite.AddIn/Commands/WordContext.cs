using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WordProSuite.Desktop.Commands
{
    internal static class WordContext
    {
        internal static dynamic Application { get; set; }
        internal static dynamic Document
        {
            get
            {
                if (Application == null || Application.Documents.Count == 0)
                    throw new InvalidOperationException("افتح مستند Word أولاً.");
                return Application.ActiveDocument;
            }
        }
        internal static dynamic Selection => Application.Selection;
        internal static dynamic TargetRange
        {
            get
            {
                dynamic selection = Selection;
                if (selection != null && selection.Range != null &&
                    selection.Range.Start != selection.Range.End)
                    return selection.Range;
                return Document.Content;
            }
        }

        internal static void ReplaceAll(dynamic range, string find, string replace)
        {
            dynamic f = range.Find;
            f.ClearFormatting();
            f.Replacement.ClearFormatting();
            f.Text = find;
            f.Replacement.Text = replace;
            f.Forward = true;
            f.Wrap = 1;
            f.Format = false;
            f.MatchWildcards = false;
            f.Execute(Replace: 2);
        }

        internal static string SavePath(string title, string filter, string fileName)
        {
            using (var dlg = new SaveFileDialog { Title = title, Filter = filter, FileName = fileName })
                return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
        }

        internal static string BaseName()
        {
            try { return Path.GetFileNameWithoutExtension((string)Document.Name); }
            catch { return "Document"; }
        }

        internal static string CleanCell(string value) =>
            (value ?? "").Replace("\r\a", "").Replace("\a", "").Trim();
    }

    internal static class TextTransforms
    {
        private static readonly Regex Diacritics = new Regex(
            "[\\u0610-\\u061A\\u064B-\\u065F\\u0670\\u06D6-\\u06ED]",
            RegexOptions.Compiled);

        internal static string RemoveDiacritics(string s) => Diacritics.Replace(s ?? "", "");
        internal static string NormalizeArabic(string s) => (s ?? "")
            .Replace('أ','ا').Replace('إ','ا').Replace('آ','ا').Replace('ٱ','ا')
            .Replace('ى','ي').Replace('ؤ','و').Replace('ئ','ي');

        internal static string Eastern(string s)
        {
            const string a="0123456789", b="٠١٢٣٤٥٦٧٨٩";
            char[] c=(s??"").ToCharArray();
            for(int i=0;i<c.Length;i++){int x=a.IndexOf(c[i]);if(x>=0)c[i]=b[x];}
            return new string(c);
        }

        internal static string Western(string s)
        {
            const string a="0123456789", b="٠١٢٣٤٥٦٧٨٩", p="۰۱۲۳۴۵۶۷۸۹";
            char[] c=(s??"").ToCharArray();
            for(int i=0;i<c.Length;i++){int x=b.IndexOf(c[i]);if(x<0)x=p.IndexOf(c[i]);if(x>=0)c[i]=a[x];}
            return new string(c);
        }

        internal static string CollapseSpaces(string s)
        {
            string t=s??"";
            t=Regex.Replace(t,@"[ \t]{2,}"," ");
            t=Regex.Replace(t,@" +\r","\r");
            return t;
        }

        internal static string[] Lines(string s) => Regex.Split(s??"", @"\r\n|\r|\n");
    }
}
