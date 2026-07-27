using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WordProSuite.Desktop.UI;

namespace WordProSuite.Desktop.Commands
{
    internal static class ProfessionalCommands
    {
        private static readonly Regex NumberRegex = new Regex(@"(?<!\w)[+-]?(?:\d+[\d,]*(?:\.\d+)?|\.\d+)(?!\w)", RegexOptions.Compiled);
        private static readonly Regex HashtagRegex = new Regex(@"(?<!\w)#[\p{L}\p{N}_]+", RegexOptions.Compiled);
        private static readonly Regex MentionRegex = new Regex(@"(?<!\w)@[\p{L}\p{N}_.-]+", RegexOptions.Compiled);
        private static readonly Regex WordRegex = new Regex(@"[\p{L}\p{N}]+", RegexOptions.Compiled);

        // ---------- Arabic and text quality ----------
        internal static void NormalizeUnicodeSpaces()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            text = text.Replace('\u00A0', ' ').Replace('\u2007', ' ').Replace('\u202F', ' ');
            text = Regex.Replace(text, @"[ \t]{2,}", " ");
            text = Regex.Replace(text, @" +(?=\r|\n)", "");
            r.Text = text;
        }

        internal static void PersianToArabicLetters()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            r.Text = text.Replace('ی', 'ي').Replace('ک', 'ك').Replace('ۀ', 'ة').Replace('ە', 'ه');
        }

        internal static void ToPersianDigits()
        {
            dynamic r = WordContext.TargetRange;
            string text = TextTransforms.Western(Convert.ToString(r.Text) ?? "");
            const string western = "0123456789";
            const string persian = "۰۱۲۳۴۵۶۷۸۹";
            char[] chars = text.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                int index = western.IndexOf(chars[i]);
                if (index >= 0) chars[i] = persian[index];
            }
            r.Text = new string(chars);
        }

        internal static void RemoveRepeatedPunctuation()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            text = Regex.Replace(text, @"([!؟?،,؛;:.])\1{1,}", "$1");
            text = Regex.Replace(text, @"\.{4,}", "...");
            r.Text = text;
        }

        internal static void WrapArabicBrackets()
        {
            dynamic r = WordContext.TargetRange;
            string text = (Convert.ToString(r.Text) ?? "").TrimEnd('\r', '\n');
            r.Text = "﴿ " + text + " ﴾";
        }

        internal static void SplitSentencesToParagraphs()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            text = Regex.Replace(text, @"(?<=[.!؟?])\s+", "\r");
            r.Text = text;
        }

        internal static void ReverseParagraphOrder()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            string[] lines = TextTransforms.Lines(text);
            Array.Reverse(lines);
            r.Text = String.Join("\r", lines);
        }

        internal static void UniqueSortedLines()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            string[] lines = TextTransforms.Lines(text);
            List<string> result = lines
                .Select(delegate(string value) { return value.Trim(); })
                .Where(delegate(string value) { return value.Length > 0; })
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .OrderBy(delegate(string value) { return value; }, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
            r.Text = String.Join("\r", result);
        }

        internal static void ExtractNumbers() { ExtractRegexMatches(NumberRegex, "الأرقام المستخرجة"); }
        internal static void ExtractHashtags() { ExtractRegexMatches(HashtagRegex, "الوسوم المستخرجة"); }
        internal static void ExtractMentions() { ExtractRegexMatches(MentionRegex, "الإشارات المستخرجة"); }

        private static void ExtractRegexMatches(Regex regex, string title)
        {
            string text = Convert.ToString(WordContext.TargetRange.Text) ?? "";
            List<string> values = regex.Matches(text).Cast<Match>()
                .Select(delegate(Match match) { return match.Value; })
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToList();
            if (values.Count == 0) throw new InvalidOperationException("لم يتم العثور على نتائج.");
            ShowReport(title, values);
        }

        internal static void LongParagraphReport()
        {
            string raw = Prompt.Show("الفقرات الطويلة", "الحد الأدنى لعدد الكلمات:", "120");
            int threshold;
            if (raw == null) return;
            if (!Int32.TryParse(raw, out threshold) || threshold < 10) throw new InvalidOperationException("أدخل رقمًا صحيحًا لا يقل عن 10.");

            dynamic document = WordContext.Document;
            var lines = new List<string>();
            int index = 0;
            foreach (dynamic paragraph in document.Paragraphs)
            {
                index++;
                string text = (Convert.ToString(paragraph.Range.Text) ?? "").Trim();
                int words = WordRegex.Matches(text).Count;
                if (words >= threshold)
                {
                    string preview = text.Length > 140 ? text.Substring(0, 140) + "..." : text;
                    lines.Add("فقرة " + index + " — " + words + " كلمة — " + preview);
                }
            }
            if (lines.Count == 0) lines.Add("لا توجد فقرات تتجاوز الحد المحدد.");
            ShowReport("تقرير الفقرات الطويلة", lines);
        }

        internal static void WordFrequencyReport()
        {
            string text = Convert.ToString(WordContext.TargetRange.Text) ?? "";
            var counts = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            foreach (Match match in WordRegex.Matches(text))
            {
                string word = match.Value.Trim();
                if (word.Length < 3) continue;
                int count;
                counts.TryGetValue(word, out count);
                counts[word] = count + 1;
            }
            List<string> lines = counts.OrderByDescending(delegate(KeyValuePair<string, int> pair) { return pair.Value; })
                .ThenBy(delegate(KeyValuePair<string, int> pair) { return pair.Key; }, StringComparer.CurrentCultureIgnoreCase)
                .Take(50)
                .Select(delegate(KeyValuePair<string, int> pair) { return pair.Key + "\t" + pair.Value; })
                .ToList();
            if (lines.Count == 0) lines.Add("لا توجد كلمات كافية للتحليل.");
            lines.Insert(0, "الكلمة\tالتكرار");
            ShowReport("أكثر الكلمات تكرارًا", lines);
        }


        internal static void ParagraphsToCheckboxes() { PrefixParagraphs("☐ "); }
        internal static void ParagraphsToCheckedBoxes() { PrefixParagraphs("☑ "); }

        private static void PrefixParagraphs(string prefix)
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            string[] lines = TextTransforms.Lines(text);
            for (int i = 0; i < lines.Length; i++)
            {
                string clean = lines[i].Trim();
                lines[i] = clean.Length == 0 ? "" : prefix + clean;
            }
            r.Text = String.Join("\r", lines);
        }

        internal static void StripLeadingListMarkers()
        {
            dynamic r = WordContext.TargetRange;
            string text = Convert.ToString(r.Text) ?? "";
            string[] lines = TextTransforms.Lines(text);
            for (int i = 0; i < lines.Length; i++)
                lines[i] = Regex.Replace(lines[i], @"^\s*(?:[☐☑▪•\-–—]|\(?\d+[.)-]|[A-Za-zأ-ي][.)-])\s*", "");
            r.Text = String.Join("\r", lines);
        }

        internal static void InsertConfidentialBanner()
        {
            dynamic document = WordContext.Document;
            dynamic range = document.Range(0, 0);
            range.Text = "سري — للاستخدام المصرح به فقط\r";
            range.ParagraphFormat.Alignment = 1;
            range.ParagraphFormat.ReadingOrder = 0;
            range.Font.Name = "Arial";
            range.Font.NameBi = "Arial";
            range.Font.Size = 12;
            range.Font.SizeBi = 12;
            range.Font.Bold = -1;
            range.Font.Color = 255;
            range.HighlightColorIndex = 7;
        }

        internal static void InsertDocumentControlBlock()
        {
            InsertBusinessTable("بطاقة ضبط المستند", new[] { "اسم المستند", "الرقم المرجعي", "الإصدار", "تاريخ الإصدار", "المالك", "حالة المستند" }, 2);
        }

        internal static void InsertApprovalTable()
        {
            InsertBusinessTable("الاعتمادات", new[] { "الدور", "الاسم", "المنصب", "التوقيع", "التاريخ" }, 4);
            dynamic table = WordContext.Document.Tables[WordContext.Document.Tables.Count];
            table.Cell(2, 1).Range.Text = "إعداد";
            table.Cell(3, 1).Range.Text = "مراجعة";
            table.Cell(4, 1).Range.Text = "اعتماد";
        }

        internal static void InsertDistributionList()
        {
            InsertBusinessTable("قائمة التوزيع", new[] { "م", "الجهة/الشخص", "نسخة رقم", "وسيلة الإرسال", "تاريخ الإرسال", "الحالة" }, 8);
        }

        internal static void InsertExecutiveSummarySkeleton()
        {
            dynamic selection = WordContext.Selection;
            string text = "الملخص التنفيذي\r\r" +
                          "1. الخلفية والسياق\r\r" +
                          "2. الهدف والنطاق\r\r" +
                          "3. أبرز النتائج\r\r" +
                          "4. التحديات والمخاطر\r\r" +
                          "5. التوصيات ذات الأولوية\r\r" +
                          "6. الخطوات القادمة\r\r" +
                          "7. مؤشرات المتابعة\r";
            selection.TypeText(text);
            dynamic r = selection.Range;
            r.ParagraphFormat.ReadingOrder = 0;
            r.Font.Name = "Arial";
            r.Font.NameBi = "Arial";
            r.Font.Size = 14;
            r.Font.SizeBi = 14;
        }

        // ---------- Document productivity ----------
        internal static void InsertCurrentDateTime()
        {
            WordContext.Selection.TypeText(DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
        }

        internal static void InsertDocumentName()
        {
            WordContext.Selection.TypeText(Convert.ToString(WordContext.Document.Name));
        }

        internal static void InsertDocumentPath()
        {
            string path = GetDocumentPath();
            WordContext.Selection.TypeText(path);
        }

        internal static void CopyDocumentPath()
        {
            string path = GetDocumentPath();
            Clipboard.SetText(path);
            MessageBox.Show("تم نسخ مسار المستند.", "WordPro Suite");
        }

        internal static void OpenDocumentFolder()
        {
            string path = GetDocumentPath();
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = "/select,\"" + path + "\"",
                UseShellExecute = true
            });
        }

        private static string GetDocumentPath()
        {
            dynamic document = WordContext.Document;
            string path = Convert.ToString(document.FullName) ?? "";
            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new InvalidOperationException("احفظ المستند أولاً للحصول على مساره.");
            return path;
        }

        internal static void InsertPageBreak() { WordContext.Selection.InsertBreak(7); }
        internal static void InsertSectionBreakNextPage() { WordContext.Selection.InsertBreak(2); }

        internal static void LockAllFields()
        {
            SetAllFieldsLocked(true);
        }

        internal static void UnlockAllFields()
        {
            SetAllFieldsLocked(false);
        }

        private static void SetAllFieldsLocked(bool locked)
        {
            dynamic document = WordContext.Document;
            int count = 0;
            foreach (dynamic story in document.StoryRanges)
            {
                dynamic current = story;
                while (current != null)
                {
                    foreach (dynamic field in current.Fields)
                    {
                        field.Locked = locked ? -1 : 0;
                        count++;
                    }
                    current = current.NextStoryRange;
                }
            }
            MessageBox.Show((locked ? "تم قفل " : "تم فتح ") + count + " حقلًا.", "WordPro Suite");
        }

        internal static void ToggleFieldCodes()
        {
            dynamic options = WordContext.Application.Options;
            bool current = Convert.ToBoolean(options.ShowFieldCodes);
            options.ShowFieldCodes = !current;
        }

        internal static void RemoveAllBookmarks()
        {
            dynamic bookmarks = WordContext.Document.Bookmarks;
            int count = Convert.ToInt32(bookmarks.Count);
            for (int i = count; i >= 1; i--) bookmarks[i].Delete();
            MessageBox.Show("تم حذف " + count + " إشارة مرجعية.", "WordPro Suite");
        }

        // ---------- Formatting and language ----------
        internal static void SetArabicProofingLanguage() { WordContext.TargetRange.LanguageID = 1025; }
        internal static void SetEnglishProofingLanguage() { WordContext.TargetRange.LanguageID = 1033; }
        internal static void KeepLinesTogether() { WordContext.TargetRange.ParagraphFormat.KeepTogether = -1; }
        internal static void WidowControlOn() { WordContext.TargetRange.ParagraphFormat.WidowControl = -1; }
        internal static void HangingIndent() { WordContext.TargetRange.ParagraphFormat.FirstLineIndent = -21.26f; }
        internal static void HighlightGreen() { WordContext.TargetRange.HighlightColorIndex = 4; }
        internal static void HighlightRed() { WordContext.TargetRange.HighlightColorIndex = 6; }
        internal static void FontColorAutomatic() { WordContext.TargetRange.Font.Color = -16777216; }

        // ---------- Business frameworks ----------
        internal static void InsertRiskRegister()
        {
            InsertBusinessTable("سجل المخاطر", new[] { "المعرّف", "الخطر", "الاحتمالية", "الأثر", "درجة الخطر", "إجراء التخفيف", "المالك", "الحالة" }, 8);
        }

        internal static void InsertActionTracker()
        {
            InsertBusinessTable("متابعة الإجراءات", new[] { "م", "الإجراء", "المسؤول", "تاريخ البدء", "الموعد النهائي", "الأولوية", "الحالة", "ملاحظات" }, 8);
        }

        internal static void InsertIssueLog()
        {
            InsertBusinessTable("سجل القضايا والمشكلات", new[] { "المعرّف", "المشكلة", "تاريخ الفتح", "المالك", "الأولوية", "الإجراء", "الحالة", "تاريخ الإغلاق" }, 8);
        }

        internal static void InsertContactDirectory()
        {
            InsertBusinessTable("دليل جهات الاتصال", new[] { "الاسم", "المنصب", "الجهة", "الهاتف", "البريد الإلكتروني", "العنوان", "ملاحظات" }, 10);
        }

        internal static void InsertInventoryTable()
        {
            InsertBusinessTable("سجل المخزون", new[] { "رمز الصنف", "اسم الصنف", "الوحدة", "الكمية", "الموقع", "الحالة", "تاريخ الاستلام", "ملاحظات" }, 10);
        }

        private static void InsertBusinessTable(string title, string[] headers, int dataRows)
        {
            dynamic selection = WordContext.Selection;
            selection.TypeText(title);
            selection.TypeParagraph();
            dynamic range = selection.Range;
            dynamic table = WordContext.Document.Tables.Add(range, dataRows + 1, headers.Length);
            table.Direction = 0;
            table.AutoFitBehavior(2);
            table.Range.ParagraphFormat.ReadingOrder = 0;
            table.Range.Font.Name = "Arial";
            table.Range.Font.NameBi = "Arial";
            table.Range.Font.Size = 10;
            table.Range.Font.SizeBi = 10;
            for (int col = 1; col <= headers.Length; col++)
            {
                dynamic cell = table.Cell(1, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = -1;
                cell.Range.ParagraphFormat.Alignment = 1;
                cell.VerticalAlignment = 1;
                cell.Shading.BackgroundPatternColor = 14277081;
            }
            table.Rows[1].HeadingFormat = -1;
            table.Borders.Enable = 1;
        }

        // ---------- Review and reporting ----------
        internal static void CommentsReport()
        {
            dynamic document = WordContext.Document;
            var lines = new List<string>();
            int index = 0;
            foreach (dynamic comment in document.Comments)
            {
                index++;
                string author = Convert.ToString(comment.Author) ?? "";
                string text = (Convert.ToString(comment.Range.Text) ?? "").Trim();
                string scope = (Convert.ToString(comment.Scope.Text) ?? "").Trim();
                lines.Add(index + ". الكاتب: " + author);
                lines.Add("النص المرتبط: " + scope);
                lines.Add("التعليق: " + text);
                lines.Add("");
            }
            if (lines.Count == 0) lines.Add("لا توجد تعليقات في المستند.");
            ShowReport("تقرير التعليقات", lines);
        }

        internal static void RevisionsSummary()
        {
            dynamic revisions = WordContext.Document.Revisions;
            var groups = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (dynamic revision in revisions)
            {
                string type = Convert.ToString(revision.Type) ?? "غير معروف";
                int count;
                groups.TryGetValue(type, out count);
                groups[type] = count + 1;
            }
            var lines = new List<string>();
            lines.Add("إجمالي التعديلات: " + Convert.ToString(revisions.Count));
            foreach (KeyValuePair<string, int> pair in groups.OrderByDescending(delegate(KeyValuePair<string, int> item) { return item.Value; }))
                lines.Add("النوع " + pair.Key + ": " + pair.Value);
            if (groups.Count == 0) lines.Add("لا توجد تعديلات متعقبة.");
            ShowReport("ملخص التعديلات المتعقبة", lines);
        }

        private static void ShowReport(string title, IEnumerable<string> lines)
        {
            dynamic application = WordContext.Application;
            dynamic report = application.Documents.Add();
            report.Content.Text = title + "\r" + new string('=', Math.Min(60, title.Length + 10)) + "\r\r" + String.Join("\r", lines);
            report.Content.ParagraphFormat.ReadingOrder = 0;
            report.Content.Font.Name = "Arial";
            report.Content.Font.NameBi = "Arial";
            report.Content.Font.Size = 12;
            report.Content.Font.SizeBi = 12;
            report.Activate();
        }
    }
}
