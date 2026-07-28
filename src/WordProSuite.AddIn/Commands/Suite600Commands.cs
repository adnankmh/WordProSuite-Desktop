using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WordProSuite.Desktop.UI;

namespace WordProSuite.Desktop.Commands
{
    internal static class Suite600Commands
    {
        private static readonly Regex SentenceRegex = new Regex(@"(?<=[\.!؟!])\s+|(?:\r\n|\r|\n)+", RegexOptions.Compiled);
        private static readonly Regex WordRegex = new Regex(@"[\p{L}\p{N}]+", RegexOptions.Compiled);
        private static readonly string DataRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WordProSuite");
        private static DateTime SessionStarted;
        private static int SessionStartWords;

        internal static void Workspace(string action)
        {
            dynamic app = WordContext.Application;
            dynamic doc = WordContext.Document;
            string workspaceDir = Path.Combine(DataRoot, "Workspaces");
            Directory.CreateDirectory(workspaceDir);
            string workspaceFile = Path.Combine(workspaceDir, "default.workspace.txt");

            switch (action)
            {
                case "save-session":
                    {
                        var paths = new List<string>();
                        for (int i = 1; i <= app.Documents.Count; i++)
                        {
                            dynamic d = app.Documents.Item(i);
                            string full = Convert.ToString(d.FullName);
                            if (!String.IsNullOrWhiteSpace(full) && File.Exists(full)) paths.Add(full);
                        }
                        File.WriteAllLines(workspaceFile, paths.Distinct(StringComparer.OrdinalIgnoreCase), Encoding.UTF8);
                        MessageBox.Show("تم حفظ مساحة العمل: " + paths.Count + " مستند.", "WordPro Suite");
                        return;
                    }
                case "restore-session":
                    {
                        if (!File.Exists(workspaceFile)) throw new InvalidOperationException("لا توجد مساحة عمل محفوظة بعد.");
                        var open = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 1; i <= app.Documents.Count; i++)
                        {
                            string full = Convert.ToString(app.Documents.Item(i).FullName);
                            if (!String.IsNullOrWhiteSpace(full)) open.Add(full);
                        }
                        int count = 0;
                        foreach (string path in File.ReadAllLines(workspaceFile, Encoding.UTF8))
                        {
                            if (File.Exists(path) && !open.Contains(path)) { app.Documents.Open(path); count++; }
                        }
                        MessageBox.Show("تمت استعادة " + count + " مستند.", "WordPro Suite");
                        return;
                    }
                case "export-open":
                    {
                        string output = WordContext.SavePath("تصدير المستندات المفتوحة", "Text files (*.txt)|*.txt", "OpenDocuments.txt");
                        if (output == null) return;
                        var lines = new List<string>();
                        for (int i = 1; i <= app.Documents.Count; i++)
                        {
                            dynamic d = app.Documents.Item(i);
                            lines.Add(Convert.ToString(d.Name) + "\t" + Convert.ToString(d.FullName));
                        }
                        File.WriteAllLines(output, lines, Encoding.UTF8);
                        Process.Start("notepad.exe", output);
                        return;
                    }
                case "close-unmodified":
                    {
                        int closed = 0;
                        for (int i = app.Documents.Count; i >= 1; i--)
                        {
                            dynamic d = app.Documents.Item(i);
                            if ((bool)d.Saved) { d.Close(false); closed++; }
                        }
                        MessageBox.Show("تم إغلاق " + closed + " مستند غير معدّل.", "WordPro Suite");
                        return;
                    }
                case "close-others":
                    {
                        string active = Convert.ToString(doc.FullName);
                        for (int i = app.Documents.Count; i >= 1; i--)
                        {
                            dynamic d = app.Documents.Item(i);
                            if (!String.Equals(Convert.ToString(d.FullName), active, StringComparison.OrdinalIgnoreCase)) d.Close();
                        }
                        return;
                    }
                case "rename":
                    {
                        string full = Convert.ToString(doc.FullName);
                        if (!File.Exists(full)) throw new InvalidOperationException("احفظ المستند أولاً.");
                        string name = Prompt.Show("إعادة تسمية المستند", "الاسم الجديد دون الامتداد:", Path.GetFileNameWithoutExtension(full));
                        if (String.IsNullOrWhiteSpace(name)) return;
                        string newPath = Path.Combine(Path.GetDirectoryName(full), SanitizeFileName(name) + Path.GetExtension(full));
                        doc.SaveAs2(newPath);
                        MessageBox.Show("تم الحفظ باسم:\n" + newPath, "WordPro Suite");
                        return;
                    }
                case "duplicate":
                    {
                        string full = Convert.ToString(doc.FullName);
                        if (!File.Exists(full)) throw new InvalidOperationException("احفظ المستند أولاً.");
                        string copy = Path.Combine(Path.GetDirectoryName(full), Path.GetFileNameWithoutExtension(full) + "_Copy_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + Path.GetExtension(full));
                        doc.SaveCopyAs(copy);
                        app.Documents.Open(copy);
                        return;
                    }
                case "compare":
                    {
                        using (var dlg = new OpenFileDialog { Filter = "Word documents|*.docx;*.docm;*.doc|All files|*.*", Title = "اختر المستند للمقارنة" })
                        {
                            if (dlg.ShowDialog() != DialogResult.OK) return;
                            dynamic other = app.Documents.Open(dlg.FileName, ReadOnly: true, Visible: false);
                            try { app.CompareDocuments(doc, other); }
                            finally { other.Close(false); }
                        }
                        return;
                    }
                case "arrange":
                    app.Windows.Arrange(0);
                    return;
                case "split-toggle":
                    {
                        dynamic window = app.ActiveWindow;
                        double split = Convert.ToDouble(window.SplitVertical, CultureInfo.InvariantCulture);
                        window.SplitVertical = split > 0 ? 0 : 50;
                        return;
                    }
                default:
                    throw new InvalidOperationException("أمر مساحة العمل غير معروف.");
            }
        }

        internal static void Text(string action)
        {
            dynamic range = WordContext.TargetRange;
            string text = Convert.ToString(range.Text) ?? "";
            switch (action)
            {
                case "extra-breaks":
                    text = Regex.Replace(text, @"(?:\r\n|\r|\n){3,}", "\r\r");
                    text = Regex.Replace(text, @"\v{2,}", "\v");
                    break;
                case "tabs": text = Regex.Replace(text, @"\t{2,}", "\t"); break;
                case "trim-document": text = text.Trim(' ', '\t', '\r', '\n', '\v'); break;
                case "punctuation-spacing":
                    text = Regex.Replace(text, @"\s+([,.;:!?،؛؟])", "$1");
                    text = Regex.Replace(text, @"([,.;:!?،؛؟])(?=[^\s\r\n])", "$1 ");
                    break;
                case "email-quotes": text = Regex.Replace(text, @"(?m)^\s*>+\s?", ""); break;
                case "list-plain":
                    try { range.ListFormat.ConvertNumbersToText(); return; }
                    catch { text = Regex.Replace(text, @"(?m)^\s*(?:[-•▪◦]|\d+[\.)])\s+", ""); }
                    break;
                case "semicolon-paragraph": text = Regex.Replace(text, @"\s*[;؛]\s*", "\r"); break;
                case "comma-lines": text = Regex.Replace(text, @"\s*[,،]\s*", "\r"); break;
                default: throw new InvalidOperationException("أمر تنظيف النص غير معروف.");
            }
            range.Text = text;
        }

        internal static void Arabic(string action)
        {
            dynamic range = WordContext.TargetRange;
            string text = Convert.ToString(range.Text) ?? "";
            switch (action)
            {
                case "proof-ar": range.LanguageID = 1025; return;
                case "proof-en": range.LanguageID = 1033; return;
                case "decimal": text = Regex.Replace(text, @"(?<=\d)\.(?=\d)", "٫"); break;
                case "thousands": text = Regex.Replace(text, @"(?<=\d),(?=\d{3}(?:\D|$))", "٬"); break;
                case "ligatures": text = text.Normalize(NormalizationForm.FormKC); break;
                case "quranic": text = Regex.Replace(text, "[\\u0610-\\u061A\\u06D6-\\u06ED]", ""); break;
                default: throw new InvalidOperationException("أمر العربية غير معروف.");
            }
            range.Text = text;
        }

        internal static void Smart(string action)
        {
            if (action == "settings") { ConfigureAiProvider(); return; }

            dynamic range = WordContext.TargetRange;
            string text = Convert.ToString(range.Text) ?? "";
            if (String.IsNullOrWhiteSpace(text)) throw new InvalidOperationException("حدد نصًا أو افتح مستندًا يحتوي نصًا.");

            switch (action)
            {
                case "offline-summary": InsertSmartResult("ملخص ذكي", OfflineSummary(text, 5)); return;
                case "executive-summary":
                    InsertSmartResult("ملخص تنفيذي", OfflineSummary(text, 4) + "\r\rعدد الكلمات: " + WordRegex.Matches(text).Count + "\rعدد الفقرات: " + TextTransforms.Lines(text).Length);
                    return;
                case "keywords": InsertSmartResult("الكلمات المفتاحية", String.Join("، ", TopWords(text, 15))); return;
                case "actions": InsertSmartResult("إجراءات العمل", ExtractSentences(text, new[] { "يجب", "يرجى", "يتعين", "مسؤول", "موعد", "إجراء", "تكليف", "will", "must", "action" })); return;
                case "decisions": InsertSmartResult("القرارات", ExtractSentences(text, new[] { "قرر", "قرار", "اعتمد", "تمت الموافقة", "وافق", "approved", "decided" })); return;
                case "questions": InsertSmartResult("أسئلة مراجعة", GenerateQuestions(text)); return;
                case "sentiment": InsertSmartResult("تحليل النبرة", Sentiment(text)); return;
                case "readability": InsertSmartResult("اقتراحات قابلية القراءة", ReadabilityAdvice(text)); return;
                case "titles":
                    {
                        string[] words = TopWords(text, 6);
                        var titles = new List<string>();
                        if (words.Length >= 2) titles.Add(words[0] + " و" + words[1] + ": تحليل وتوصيات");
                        if (words.Length >= 3) titles.Add("دراسة " + words[0] + " في ضوء " + words[2]);
                        titles.Add("ملخص احترافي للمستند");
                        InsertSmartResult("عناوين مقترحة", String.Join("\r", titles.Select((x, i) => (i + 1) + ". " + x)));
                        return;
                    }
                case "outline": InsertSmartResult("مخطط هيكلي", GenerateOutline(text)); return;
                case "provider-rewrite": ReplaceWithProvider(text, "أعد صياغة النص بأسلوب احترافي واضح مع الحفاظ على المعنى. أعد النص فقط."); return;
                case "provider-translate":
                    {
                        string target = Prompt.Show("الترجمة بالذكاء الاصطناعي", "اللغة المستهدفة:", "العربية");
                        if (target == null) return;
                        ReplaceWithProvider(text, "ترجم النص إلى " + target + " ترجمة دقيقة وطبيعية. أعد الترجمة فقط.");
                        return;
                    }
                case "provider-chat":
                    {
                        string question = Prompt.Show("دردشة مع المستند", "اكتب سؤالك عن النص:");
                        if (question == null) return;
                        InsertSmartResult("إجابة الذكاء الاصطناعي", AskProvider(question + "\n\nالنص المرجعي:\n" + text));
                        return;
                    }
                default: throw new InvalidOperationException("أمر الذكاء غير معروف.");
            }
        }

        internal static void Table(string action)
        {
            if (action == "calculation") { InsertTable("جدول حسابات", new[] { "م", "البند", "الكمية", "سعر الوحدة", "الإجمالي" }, 10); return; }
            if (action == "summary") { InsertTable("ملخص البيانات", new[] { "الفئة", "العدد", "القيمة", "النسبة", "الملاحظات" }, 10); return; }
            if (action == "tabs-to-table")
            {
                dynamic r = WordContext.TargetRange;
                r.ConvertToTable(Separator: 0, AutoFitBehavior: 1);
                return;
            }

            dynamic table = CurrentTable();
            if (action == "alternate-columns")
            {
                for (int c = 1; c <= table.Columns.Count; c++)
                {
                    if (c % 2 == 0) table.Columns.Item(c).Shading.BackgroundPatternColor = 14277081;
                }
                return;
            }

            for (int i = 1; i <= table.Range.Cells.Count; i++)
            {
                dynamic cell = table.Range.Cells.Item(i);
                string raw = CleanCell(Convert.ToString(cell.Range.Text));
                double number;
                DateTime date;
                if (action == "percent" && Double.TryParse(TextTransforms.Western(raw).Replace("%", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                    cell.Range.Text = (number > 1 ? number / 100d : number).ToString("P1", CultureInfo.InvariantCulture);
                else if (action == "currency" && Double.TryParse(TextTransforms.Western(raw).Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                    cell.Range.Text = number.ToString("N2", CultureInfo.InvariantCulture);
                else if (action == "date" && DateTime.TryParse(raw, out date))
                    cell.Range.Text = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }

        internal static void Media(string action)
        {
            dynamic doc = WordContext.Document;
            if (action == "export-images")
            {
                string full = Convert.ToString(doc.FullName);
                if (!File.Exists(full)) throw new InvalidOperationException("احفظ المستند أولاً.");
                string folder = Path.Combine(Path.GetDirectoryName(full), WordContext.BaseName() + "_Images_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                Directory.CreateDirectory(folder);
                string copyPath = Path.Combine(folder, Path.GetFileName(full));
                string html = Path.Combine(folder, "document.html");
                doc.SaveCopyAs(copyPath);
                dynamic copyDocument = WordContext.Application.Documents.Open(copyPath, ReadOnly: false, Visible: false, AddToRecentFiles: false);
                try { copyDocument.SaveAs2(html, 10); }
                finally { copyDocument.Close(false); }
                Process.Start("explorer.exe", folder);
                return;
            }

            if (action == "compress")
            {
                try { WordContext.Application.CommandBars.ExecuteMso("PicturesCompress"); }
                catch { throw new InvalidOperationException("حدد صورة أولاً ثم أعد تشغيل أداة ضغط الصور."); }
                return;
            }

            if (action == "accessibility")
            {
                var missing = new List<string>();
                for (int i = 1; i <= doc.InlineShapes.Count; i++)
                {
                    dynamic shape = doc.InlineShapes.Item(i);
                    string alt = Convert.ToString(shape.AlternativeText);
                    if (String.IsNullOrWhiteSpace(alt)) missing.Add("InlineShape " + i);
                }
                for (int i = 1; i <= doc.Shapes.Count; i++)
                {
                    dynamic shape = doc.Shapes.Item(i);
                    string alt = Convert.ToString(shape.AlternativeText);
                    if (String.IsNullOrWhiteSpace(alt)) missing.Add("Shape " + i);
                }
                MessageBox.Show(missing.Count == 0 ? "كل الصور تحتوي نصًا بديلاً." : "صور دون نص بديل:\n" + String.Join("\n", missing), "تقرير وصول الصور");
                return;
            }

            double maxWidth = 0;
            try
            {
                dynamic section = doc.Sections.Item(1);
                maxWidth = Convert.ToDouble(section.PageSetup.PageWidth) - Convert.ToDouble(section.PageSetup.LeftMargin) - Convert.ToDouble(section.PageSetup.RightMargin);
            }
            catch { maxWidth = 450; }

            for (int i = 1; i <= doc.InlineShapes.Count; i++)
            {
                dynamic shape = doc.InlineShapes.Item(i);
                if (action == "resize-half") { shape.LockAspectRatio = -1; shape.Width = Convert.ToSingle(shape.Width) * 0.5f; }
                else if (action == "fit-page") { shape.LockAspectRatio = -1; if (Convert.ToDouble(shape.Width) > maxWidth) shape.Width = maxWidth; }
                else if (action == "center") { shape.Range.ParagraphFormat.Alignment = 1; }
                else if (action == "captions")
                {
                    try { shape.Range.InsertCaption("شكل", " " + i, 0, 0); } catch { }
                }
            }
        }

        internal static void Batch(string action)
        {
            using (var dlg = new FolderBrowserDialog { Description = "اختر مجلد ملفات Word" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                string[] files = Directory.GetFiles(dlg.SelectedPath, "*.doc*", SearchOption.TopDirectoryOnly)
                    .Where(x => !Path.GetFileName(x).StartsWith("~$", StringComparison.OrdinalIgnoreCase)).ToArray();
                dynamic app = WordContext.Application;
                bool oldVisible = app.Visible;
                int ok = 0;
                var errors = new List<string>();
                foreach (string file in files)
                {
                    dynamic doc = null;
                    try
                    {
                        doc = app.Documents.Open(file, ReadOnly: false, Visible: false, AddToRecentFiles: false);
                        switch (action)
                        {
                            case "docx":
                                {
                                    string output = Path.Combine(dlg.SelectedPath, Path.GetFileNameWithoutExtension(file) + ".docx");
                                    doc.SaveAs2(output, 16); break;
                                }
                            case "pdf": doc.ExportAsFixedFormat(Path.ChangeExtension(file, ".pdf"), 17); break;
                            case "remove-comments": while (doc.Comments.Count > 0) doc.Comments.Item(1).Delete(); doc.Save(); break;
                            case "accept-revisions": if (doc.Revisions.Count > 0) doc.AcceptAllRevisions(); doc.Save(); break;
                            case "update-fields": doc.Fields.Update(); doc.Save(); break;
                            case "protect": doc.Protect(3, true); doc.Save(); break;
                            case "metadata": doc.RemoveDocumentInformation(99); doc.Save(); break;
                            case "print": doc.PrintOut(Background: false); break;
                            default: throw new InvalidOperationException("عملية دفعية غير معروفة.");
                        }
                        ok++;
                    }
                    catch (Exception ex) { errors.Add(Path.GetFileName(file) + ": " + ex.Message); }
                    finally { try { if (doc != null) doc.Close(false); } catch { } }
                }
                app.Visible = oldVisible;
                MessageBox.Show("تمت معالجة " + ok + " من " + files.Length + " ملف." + (errors.Count > 0 ? "\n\nأخطاء:\n" + String.Join("\n", errors.Take(10)) : ""), "العمليات الدفعية");
            }
        }

        internal static void Academic(string action)
        {
            dynamic doc = WordContext.Document;
            string text = Convert.ToString(WordContext.TargetRange.Text) ?? "";
            switch (action)
            {
                case "apa": WordContext.Selection.TypeText("اسم العائلة، الاسم الأول. (السنة). عنوان العمل. الناشر. DOI/URL"); return;
                case "mla": WordContext.Selection.TypeText("اسم العائلة، الاسم الأول. \"عنوان العمل.\" اسم المصدر، الناشر، السنة، الصفحات."); return;
                case "bibliography-audit":
                    {
                        string[] lines = TextTransforms.Lines(text).Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
                        string[] dup = lines.GroupBy(x => x, StringComparer.CurrentCultureIgnoreCase).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
                        MessageBox.Show("الأسطر المرجعية: " + lines.Length + "\nالمكررة: " + dup.Length + (dup.Length > 0 ? "\n\n" + String.Join("\n", dup.Take(20)) : ""), "تدقيق المراجع");
                        return;
                    }
                case "footnotes":
                    {
                        int empty = 0;
                        for (int i = 1; i <= doc.Footnotes.Count; i++) if (CleanCell(Convert.ToString(doc.Footnotes.Item(i).Range.Text)).Length < 3) empty++;
                        MessageBox.Show("عدد الحواشي: " + doc.Footnotes.Count + "\nالحواشي الفارغة/القصيرة: " + empty, "تدقيق الحواشي");
                        return;
                    }
                case "headings":
                    {
                        int h1 = 0, h2 = 0, h3 = 0;
                        for (int i = 1; i <= doc.Paragraphs.Count; i++)
                        {
                            string style = Convert.ToString(doc.Paragraphs.Item(i).Range.get_Style());
                            if (style.IndexOf("1", StringComparison.OrdinalIgnoreCase) >= 0 && style.IndexOf("Heading", StringComparison.OrdinalIgnoreCase) >= 0) h1++;
                            else if (style.IndexOf("2", StringComparison.OrdinalIgnoreCase) >= 0 && style.IndexOf("Heading", StringComparison.OrdinalIgnoreCase) >= 0) h2++;
                            else if (style.IndexOf("3", StringComparison.OrdinalIgnoreCase) >= 0 && style.IndexOf("Heading", StringComparison.OrdinalIgnoreCase) >= 0) h3++;
                        }
                        MessageBox.Show("Heading 1: " + h1 + "\nHeading 2: " + h2 + "\nHeading 3: " + h3, "هيكل العناوين");
                        return;
                    }
                case "extract":
                    {
                        MatchCollection matches = Regex.Matches(text, @"\([^\r\n()]{3,120}\)");
                        InsertSmartResult("الاستشهادات المستخرجة", String.Join("\r", matches.Cast<Match>().Select(m => m.Value).Distinct()));
                        return;
                    }
                default: throw new InvalidOperationException("أمر أكاديمي غير معروف.");
            }
        }

        internal static void Security(string action)
        {
            dynamic range = WordContext.TargetRange;
            string text = Convert.ToString(range.Text) ?? "";
            switch (action)
            {
                case "emails": range.Text = Regex.Replace(text, @"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}", "████████", RegexOptions.IgnoreCase); return;
                case "phones": range.Text = Regex.Replace(text, @"(?<!\d)(?:\+?\d[\d\s\-()]{6,}\d)(?!\d)", "████████"); return;
                case "identifiers": range.Text = Regex.Replace(text, @"(?<!\d)\d{8,20}(?!\d)", "████████"); return;
                case "selection":
                    {
                        if (range.Start == range.End) throw new InvalidOperationException("حدد النص المطلوب طمسه.");
                        int length = Math.Max(4, Math.Min(80, text.TrimEnd('\r', '\n', '\a').Length));
                        range.Text = new string('█', length);
                        return;
                    }
                case "audit":
                    {
                        dynamic doc = WordContext.Document;
                        int emails = Regex.Matches(text, @"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase).Count;
                        int phones = Regex.Matches(text, @"(?<!\d)(?:\+?\d[\d\s\-()]{6,}\d)(?!\d)").Count;
                        MessageBox.Show("روابط: " + doc.Hyperlinks.Count + "\nتعليقات: " + doc.Comments.Count + "\nتعديلات: " + doc.Revisions.Count + "\nبريد محتمل: " + emails + "\nهواتف محتملة: " + phones, "تدقيق أمان المستند");
                        return;
                    }
                case "hidden":
                    {
                        dynamic doc = WordContext.Document;
                        dynamic findRange = doc.Content.Duplicate;
                        dynamic find = findRange.Find;
                        find.ClearFormatting();
                        find.Font.Hidden = 1;
                        find.Replacement.ClearFormatting();
                        find.Replacement.Text = "";
                        find.Execute(FindText: "", ReplaceWith: "", Replace: 2, Format: true);
                        return;
                    }
                default: throw new InvalidOperationException("أمر الأمان غير معروف.");
            }
        }

        internal static void Export(string action)
        {
            dynamic doc = WordContext.Document;
            string baseName = WordContext.BaseName();
            if (action == "booklet") { doc.PageSetup.MultiplePages = 2; return; }
            if (action == "pdf-range")
            {
                string rawFrom = Prompt.Show("تصدير نطاق PDF", "أول صفحة:", "1"); if (rawFrom == null) return;
                string rawTo = Prompt.Show("تصدير نطاق PDF", "آخر صفحة:", rawFrom); if (rawTo == null) return;
                int from, to;
                if (!Int32.TryParse(rawFrom, out from) || !Int32.TryParse(rawTo, out to) || from < 1 || to < from) throw new InvalidOperationException("نطاق الصفحات غير صحيح.");
                string output = WordContext.SavePath("تصدير PDF", "PDF (*.pdf)|*.pdf", baseName + "_Pages_" + from + "-" + to + ".pdf");
                if (output != null) doc.ExportAsFixedFormat(output, 17, false, 0, 3, from, to);
                return;
            }

            int format;
            string ext;
            string filter;
            if (action == "html") { format = 10; ext = ".html"; filter = "HTML (*.html)|*.html"; }
            else if (action == "rtf") { format = 6; ext = ".rtf"; filter = "RTF (*.rtf)|*.rtf"; }
            else if (action == "odt") { format = 23; ext = ".odt"; filter = "OpenDocument (*.odt)|*.odt"; }
            else throw new InvalidOperationException("صيغة التصدير غير معروفة.");
            string path = WordContext.SavePath("تصدير المستند", filter, baseName + ext);
            if (path != null) doc.SaveAs2(path, format);
        }

        internal static void ClipboardTools(string action)
        {
            string dir = Path.Combine(DataRoot, "Snippets");
            Directory.CreateDirectory(dir);
            if (action == "save")
            {
                string text = Convert.ToString(WordContext.TargetRange.Text) ?? "";
                if (String.IsNullOrWhiteSpace(text)) throw new InvalidOperationException("حدد نصًا لحفظه.");
                string name = Prompt.Show("حفظ مقتطف", "اسم المقتطف:", "Snippet " + DateTime.Now.ToString("yyyy-MM-dd HH-mm"));
                if (String.IsNullOrWhiteSpace(name)) return;
                File.WriteAllText(Path.Combine(dir, SanitizeFileName(name) + ".txt"), text, Encoding.UTF8);
                return;
            }
            if (action == "insert")
            {
                string[] files = Directory.GetFiles(dir, "*.txt").OrderBy(x => x).ToArray();
                if (files.Length == 0) throw new InvalidOperationException("لا توجد مقتطفات محفوظة.");
                string list = String.Join("\n", files.Select((x, i) => (i + 1) + ". " + Path.GetFileNameWithoutExtension(x)));
                string raw = Prompt.Show("إدراج مقتطف", "اكتب الرقم:\n" + list, "1");
                int index;
                if (raw == null || !Int32.TryParse(raw, out index) || index < 1 || index > files.Length) return;
                WordContext.Selection.TypeText(File.ReadAllText(files[index - 1], Encoding.UTF8));
                return;
            }
            if (action == "clear")
            {
                foreach (string file in Directory.GetFiles(dir, "*.txt")) File.Delete(file);
                MessageBox.Show("تم مسح مكتبة المقتطفات.", "WordPro Suite");
                return;
            }
            if (action == "append")
            {
                string text = Convert.ToString(WordContext.TargetRange.Text) ?? "";
                if (String.IsNullOrWhiteSpace(text)) throw new InvalidOperationException("حدد نصًا أولاً.");
                string file = Path.Combine(dir, "Collected.txt");
                File.AppendAllText(file, text.Trim() + Environment.NewLine + Environment.NewLine, Encoding.UTF8);
                MessageBox.Show("تمت الإضافة إلى Collected.txt", "WordPro Suite");
                return;
            }
            throw new InvalidOperationException("أمر الحافظة غير معروف.");
        }

        internal static void Legal(string action)
        {
            if (action == "checklist")
            {
                InsertTable("قائمة فحص العقد", new[] { "البند", "موجود", "مخاطر/ملاحظات", "الإجراء" }, 16,
                    new[] { "بيانات الأطراف", "التعريفات", "النطاق", "المقابل المالي", "مدة العقد", "الإنهاء", "السرية", "الملكية الفكرية", "القوة القاهرة", "القانون الحاكم", "حل النزاعات", "الإشعارات", "التوقيعات", "الملاحق", "حماية البيانات" });
                return;
            }
            if (action == "parties")
            {
                InsertTable("أطراف العقد", new[] { "الطرف", "الاسم القانوني", "رقم التسجيل", "العنوان", "الممثل", "وسائل الاتصال" }, 5);
                return;
            }
            if (action == "obligations")
            {
                HighlightTerms(new[] { "يجب", "يلتزم", "يتعين", "مسؤول", "يحظر", "shall", "must", "obliged" }); return;
            }
            if (action == "dates")
            {
                dynamic range = WordContext.Document.Content.Duplicate;
                string text = Convert.ToString(range.Text) ?? "";
                var matches = Regex.Matches(text, @"\b(?:\d{1,2}[/\-.]\d{1,2}[/\-.]\d{2,4}|\d+\s+(?:يوم|أيام|شهر|أشهر|سنة|سنوات))\b");
                foreach (Match match in matches.Cast<Match>().Reverse())
                {
                    dynamic r = WordContext.Document.Range(range.Start + match.Index, range.Start + match.Index + match.Length);
                    r.HighlightColorIndex = 7;
                }
                MessageBox.Show("تم تمييز " + matches.Count + " تاريخًا أو مدة محتملة.", "WordPro Suite");
                return;
            }
            throw new InvalidOperationException("أمر قانوني غير معروف.");
        }

        internal static void Project(string action)
        {
            if (action == "gantt") { InsertTable("خطة Gantt مبسطة", new[] { "المهمة", "المسؤول", "البداية", "النهاية", "المدة", "التقدم %", "الحالة" }, 15); return; }
            if (action == "board") { InsertTable("لوحة المهام", new[] { "Backlog", "To Do", "Doing", "Review", "Done" }, 12); return; }
            if (action == "weekly")
            {
                WordContext.Selection.TypeText("تقرير الحالة الأسبوعي\r\r1. الملخص التنفيذي\r\r2. الإنجازات\r\r3. الأنشطة الجارية\r\r4. المخاطر والمشكلات\r\r5. القرارات المطلوبة\r\r6. خطة الأسبوع القادم\r\r7. مؤشرات الأداء\r");
                return;
            }
            if (action == "deliverables") { InsertTable("سجل المخرجات", new[] { "المخرج", "الوصف", "المسؤول", "الاستحقاق", "معيار القبول", "المراجع", "الحالة" }, 15); return; }
            throw new InvalidOperationException("أمر مشروع غير معروف.");
        }

        internal static void Forms(string action)
        {
            dynamic doc = WordContext.Document;
            dynamic selection = WordContext.Selection;
            if (action == "text")
            {
                dynamic c = doc.ContentControls.Add(1, selection.Range); c.Title = "حقل نص"; c.Tag = "TextField"; return;
            }
            if (action == "date")
            {
                dynamic c = doc.ContentControls.Add(6, selection.Range); c.Title = "التاريخ"; c.Tag = "DateField"; return;
            }
            if (action == "checkbox")
            {
                dynamic c = doc.ContentControls.Add(8, selection.Range); c.Title = "اختيار"; c.Tag = "CheckField"; return;
            }
            if (action == "export")
            {
                string output = WordContext.SavePath("تصدير بيانات النموذج", "CSV (*.csv)|*.csv", WordContext.BaseName() + "_FormData.csv");
                if (output == null) return;
                var lines = new List<string> { "Index,Title,Tag,Value" };
                for (int i = 1; i <= doc.ContentControls.Count; i++)
                {
                    dynamic c = doc.ContentControls.Item(i);
                    lines.Add(i + ",\"" + Csv(Convert.ToString(c.Title)) + "\",\"" + Csv(Convert.ToString(c.Tag)) + "\",\"" + Csv(CleanCell(Convert.ToString(c.Range.Text))) + "\"");
                }
                File.WriteAllLines(output, lines, new UTF8Encoding(true));
                Process.Start("explorer.exe", "/select,\"" + output + "\"");
                return;
            }
            throw new InvalidOperationException("أمر نموذج غير معروف.");
        }

        internal static void Productivity(string action)
        {
            dynamic app = WordContext.Application;
            dynamic doc = WordContext.Document;
            if (action == "focus")
            {
                try { app.CommandBars.ExecuteMso("MinimizeRibbon"); } catch { }
                app.DisplayStatusBar = false;
                try { app.ActiveWindow.DisplayRulers = false; } catch { }
                try { app.ActiveWindow.View.ShowAll = false; } catch { }
                return;
            }
            if (action == "restore")
            {
                app.DisplayStatusBar = true;
                try { app.ActiveWindow.DisplayRulers = true; } catch { }
                try { app.ActiveWindow.View.ShowAll = false; } catch { }
                return;
            }
            if (action == "session")
            {
                SessionStarted = DateTime.Now;
                SessionStartWords = Convert.ToInt32(doc.ComputeStatistics(0));
                MessageBox.Show("بدأت جلسة الكتابة عند " + SessionStarted.ToString("HH:mm") + ".", "WordPro Suite");
                return;
            }
            if (action == "goal")
            {
                string raw = Prompt.Show("هدف الكلمات", "عدد الكلمات المستهدف:", "1000");
                int goal;
                if (raw == null || !Int32.TryParse(raw, out goal) || goal <= 0) return;
                int current = Convert.ToInt32(doc.ComputeStatistics(0));
                double pct = Math.Min(100d, current * 100d / goal);
                MessageBox.Show("الحالي: " + current + "\nالهدف: " + goal + "\nالإنجاز: " + pct.ToString("0.0") + "%", "هدف الكلمات");
                return;
            }
            if (action == "reading-time")
            {
                int words = Convert.ToInt32(doc.ComputeStatistics(0));
                double minutes = words / 220d;
                MessageBox.Show("عدد الكلمات: " + words + "\nزمن القراءة المتوقع: " + Math.Ceiling(minutes) + " دقيقة.", "زمن القراءة");
                return;
            }
            if (action == "report")
            {
                int words = Convert.ToInt32(doc.ComputeStatistics(0));
                string duration = SessionStarted == default(DateTime) ? "لم تبدأ جلسة" : (DateTime.Now - SessionStarted).ToString(@"hh\:mm\:ss");
                int added = SessionStarted == default(DateTime) ? 0 : words - SessionStartWords;
                MessageBox.Show("المستند: " + Convert.ToString(doc.Name) + "\nالكلمات: " + words + "\nمدة الجلسة: " + duration + "\nالكلمات المضافة: " + added, "تقرير جلسة العمل");
                return;
            }
            throw new InvalidOperationException("أمر إنتاجية غير معروف.");
        }

        private static void ConfigureAiProvider()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\WordProSuite\AI"))
            {
                string endpoint = Prompt.Show("إعداد موفر AI", "Endpoint المتوافق مع OpenAI:", Convert.ToString(key.GetValue("Endpoint", "http://localhost:11434/v1/chat/completions")));
                if (endpoint == null) return;
                string model = Prompt.Show("إعداد موفر AI", "اسم النموذج:", Convert.ToString(key.GetValue("Model", "llama3.2")));
                if (model == null) return;
                string apiKey = Prompt.Show("إعداد موفر AI", "API Key (اتركه فارغًا للمحلي):", Convert.ToString(key.GetValue("ApiKey", "")), true);
                if (apiKey == null) return;
                key.SetValue("Endpoint", endpoint.Trim());
                key.SetValue("Model", model.Trim());
                key.SetValue("ApiKey", apiKey.Trim());
            }
            MessageBox.Show("تم حفظ إعدادات موفر الذكاء الاصطناعي.", "WordPro Suite");
        }

        private static string AskProvider(string prompt)
        {
            string endpoint, model, apiKey;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\WordProSuite\AI"))
            {
                endpoint = Convert.ToString(key == null ? null : key.GetValue("Endpoint"));
                model = Convert.ToString(key == null ? null : key.GetValue("Model"));
                apiKey = Convert.ToString(key == null ? null : key.GetValue("ApiKey"));
            }
            if (String.IsNullOrWhiteSpace(endpoint) || String.IsNullOrWhiteSpace(model))
                throw new InvalidOperationException("اضبط موفر AI من أداة «إعداد موفر AI» أولاً. يمكن استخدام Ollama أو LM Studio أو OpenAI-Compatible.");

            var payload = new Dictionary<string, object>
            {
                { "model", model },
                { "temperature", 0.2 },
                { "messages", new object[] { new Dictionary<string, object> { { "role", "user" }, { "content", prompt } } } }
            };
            string json = new JavaScriptSerializer().Serialize(payload);
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(3);
                if (!String.IsNullOrWhiteSpace(apiKey)) client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage response = client.PostAsync(endpoint, content).GetAwaiter().GetResult();
                    string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode) throw new InvalidOperationException("فشل موفر AI: " + (int)response.StatusCode + "\n" + body);
                    object root = new JavaScriptSerializer().DeserializeObject(body);
                    var dict = root as Dictionary<string, object>;
                    if (dict == null || !dict.ContainsKey("choices")) throw new InvalidOperationException("استجابة موفر AI غير متوقعة.");
                    object[] choices = dict["choices"] as object[];
                    if (choices == null || choices.Length == 0) throw new InvalidOperationException("لم يُرجع موفر AI نتيجة.");
                    var choice = choices[0] as Dictionary<string, object>;
                    var message = choice == null ? null : choice["message"] as Dictionary<string, object>;
                    string result = message == null ? "" : Convert.ToString(message["content"]);
                    if (String.IsNullOrWhiteSpace(result)) throw new InvalidOperationException("نتيجة موفر AI فارغة.");
                    return result.Trim();
                }
            }
        }

        private static void ReplaceWithProvider(string text, string instruction)
        {
            dynamic range = WordContext.TargetRange;
            range.Text = AskProvider(instruction + "\n\n" + text);
        }

        private static string OfflineSummary(string text, int maxSentences)
        {
            string[] sentences = SentenceRegex.Split(text).Select(x => x.Trim()).Where(x => x.Length >= 25).ToArray();
            if (sentences.Length == 0) return text.Length <= 800 ? text.Trim() : text.Substring(0, 800).Trim() + "…";
            string[] keywords = TopWords(text, 12);
            var ranked = sentences.Select((s, i) => new
            {
                Text = s,
                Index = i,
                Score = keywords.Count(k => s.IndexOf(k, StringComparison.CurrentCultureIgnoreCase) >= 0) + (i < 3 ? 1 : 0)
            }).OrderByDescending(x => x.Score).ThenBy(x => x.Index).Take(maxSentences).OrderBy(x => x.Index).Select(x => x.Text);
            return String.Join("\r", ranked.Select(x => "• " + x));
        }

        private static string[] TopWords(string text, int count)
        {
            var stop = new HashSet<string>(new[] { "من", "في", "على", "إلى", "عن", "أن", "إن", "هو", "هي", "هذا", "هذه", "ذلك", "التي", "الذي", "مع", "كما", "تم", "the", "and", "of", "to", "in", "for", "is", "a", "on", "with", "that" }, StringComparer.CurrentCultureIgnoreCase);
            return WordRegex.Matches(text).Cast<Match>().Select(m => m.Value.ToLower(CultureInfo.CurrentCulture)).Where(w => w.Length > 2 && !stop.Contains(w))
                .GroupBy(w => w, StringComparer.CurrentCultureIgnoreCase).OrderByDescending(g => g.Count()).ThenBy(g => g.Key).Take(count).Select(g => g.Key).ToArray();
        }

        private static string ExtractSentences(string text, string[] markers)
        {
            string[] items = SentenceRegex.Split(text).Select(x => x.Trim()).Where(x => x.Length > 10 && markers.Any(m => x.IndexOf(m, StringComparison.CurrentCultureIgnoreCase) >= 0)).Distinct().Take(30).ToArray();
            return items.Length == 0 ? "لم يتم العثور على عناصر واضحة." : String.Join("\r", items.Select(x => "• " + x));
        }

        private static string GenerateQuestions(string text)
        {
            string[] sentences = SentenceRegex.Split(text).Select(x => x.Trim()).Where(x => x.Length > 35).Take(10).ToArray();
            var result = new List<string>();
            for (int i = 0; i < sentences.Length; i++)
            {
                string phrase = sentences[i].Length > 100 ? sentences[i].Substring(0, 100) + "…" : sentences[i];
                result.Add((i + 1) + ". ما الفكرة الأساسية في العبارة التالية: «" + phrase + "»؟");
            }
            return result.Count == 0 ? "لا يوجد نص كافٍ لإنشاء أسئلة." : String.Join("\r", result);
        }

        private static string Sentiment(string text)
        {
            string[] positive = { "ممتاز", "جيد", "نجاح", "تحسن", "إيجابي", "فرصة", "قوي", "effective", "success", "good", "excellent" };
            string[] negative = { "ضعيف", "مشكلة", "خطر", "فشل", "سلبي", "تأخير", "نقص", "risk", "failure", "problem", "bad" };
            int pos = positive.Sum(x => Regex.Matches(text, Regex.Escape(x), RegexOptions.IgnoreCase).Count);
            int neg = negative.Sum(x => Regex.Matches(text, Regex.Escape(x), RegexOptions.IgnoreCase).Count);
            string label = pos > neg ? "إيجابية غالبًا" : neg > pos ? "سلبية/تحذيرية غالبًا" : "محايدة أو متوازنة";
            return "التقييم التقريبي: " + label + "\rالكلمات الإيجابية: " + pos + "\rالكلمات السلبية: " + neg;
        }

        private static string ReadabilityAdvice(string text)
        {
            string[] sentences = SentenceRegex.Split(text).Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            int words = WordRegex.Matches(text).Count;
            double average = sentences.Length == 0 ? 0 : (double)words / sentences.Length;
            var advice = new List<string> { "متوسط طول الجملة: " + average.ToString("0.0") + " كلمة." };
            if (average > 28) advice.Add("• قصّر الجمل الطويلة وقسّم الأفكار المركبة.");
            if (Regex.Matches(text, @"\([^\)]{80,}\)").Count > 0) advice.Add("• توجد عبارات اعتراضية طويلة؛ فكّر في تحويلها إلى جمل مستقلة.");
            if (TextTransforms.Lines(text).Count(x => x.Length > 500) > 0) advice.Add("• توجد فقرات طويلة؛ قسّمها بعناوين فرعية أو قوائم.");
            if (advice.Count == 1) advice.Add("• مستوى طول الجمل مناسب إجمالاً.");
            return String.Join("\r", advice);
        }

        private static string GenerateOutline(string text)
        {
            string[] lines = TextTransforms.Lines(text).Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            var candidates = lines.Where(x => x.Length <= 120).Take(20).ToArray();
            if (candidates.Length == 0) candidates = SentenceRegex.Split(text).Select(x => x.Trim()).Where(x => x.Length > 10).Take(12).ToArray();
            return String.Join("\r", candidates.Select((x, i) => (i + 1) + ". " + x));
        }

        private static void InsertSmartResult(string title, string content)
        {
            dynamic doc = WordContext.Application.Documents.Add();
            dynamic range = doc.Range(0, 0);
            range.Text = title + "\r\r" + content;
            range.ParagraphFormat.ReadingOrder = 0;
            range.Font.Name = "Arial";
            range.Font.NameBi = "Arial";
            range.Font.Size = 14;
            range.Font.SizeBi = 14;
        }

        private static dynamic CurrentTable()
        {
            dynamic selection = WordContext.Selection;
            if (!Convert.ToBoolean(selection.Information(12), CultureInfo.InvariantCulture)) throw new InvalidOperationException("ضع المؤشر داخل جدول أولاً.");
            return selection.Tables.Item(1);
        }

        private static void InsertTable(string title, string[] headers, int rows, string[] firstColumn = null)
        {
            dynamic selection = WordContext.Selection;
            selection.TypeText(title + "\r");
            dynamic range = selection.Range;
            dynamic table = WordContext.Document.Tables.Add(range, rows, headers.Length);
            table.Borders.Enable = 1;
            table.AutoFitBehavior(2);
            table.Rows.Item(1).Range.Bold = 1;
            table.Rows.Item(1).HeadingFormat = -1;
            table.Rows.Item(1).Range.ParagraphFormat.Alignment = 1;
            for (int c = 1; c <= headers.Length; c++) table.Cell(1, c).Range.Text = headers[c - 1];
            if (firstColumn != null)
            {
                int max = Math.Min(firstColumn.Length, rows - 1);
                for (int r = 0; r < max; r++) table.Cell(r + 2, 1).Range.Text = firstColumn[r];
            }
        }

        private static void HighlightTerms(string[] terms)
        {
            dynamic doc = WordContext.Document;
            int count = 0;
            foreach (string term in terms)
            {
                dynamic r = doc.Content.Duplicate;
                dynamic f = r.Find;
                f.ClearFormatting();
                f.Text = term;
                f.Forward = true;
                f.Wrap = 0;
                f.MatchCase = false;
                while (f.Execute())
                {
                    r.HighlightColorIndex = 7;
                    count++;
                    r.Collapse(0);
                }
            }
            MessageBox.Show("تم تمييز " + count + " موضعًا.", "WordPro Suite");
        }

        private static string CleanCell(string value)
        {
            return (value ?? "").Replace("\r\a", "").Replace("\a", "").Trim();
        }

        private static string Csv(string value)
        {
            return (value ?? "").Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ");
        }

        private static string SanitizeFileName(string value)
        {
            string result = value ?? "";
            foreach (char c in Path.GetInvalidFileNameChars()) result = result.Replace(c, '_');
            return result.Trim();
        }
    }
}
