using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WordProSuite.Desktop.Infrastructure;
using WordProSuite.Desktop.Licensing;
using WordProSuite.Desktop.UI;

namespace WordProSuite.Desktop.Commands
{
    internal sealed class CommandDescriptor
    {
        internal CommandDescriptor(string id, string category, string title, string description, string keywords, Action execute, bool requiresLicense)
        {
            Id = id; Category = category; Title = title; Description = description; Keywords = keywords ?? "";
            Execute = execute; RequiresLicense = requiresLicense;
        }
        internal string Id { get; }
        internal string Category { get; }
        internal string Title { get; }
        internal string Description { get; }
        internal string Keywords { get; }
        internal Action Execute { get; }
        internal bool RequiresLicense { get; }
    }

    internal static class CommandRouter
    {
        private static readonly Dictionary<string, CommandDescriptor> Map =
            new Dictionary<string, CommandDescriptor>(StringComparer.OrdinalIgnoreCase);

        static CommandRouter()
        {
            // العربية والاتجاه
            A("rtl", "العربية والاتجاه", "اتجاه RTL", "تطبيق اتجاه الكتابة من اليمين إلى اليسار.", "عربي يمين", Commands.Rtl);
            A("ltr", "العربية والاتجاه", "اتجاه LTR", "تطبيق اتجاه الكتابة من اليسار إلى اليمين.", "انجليزي يسار", Commands.Ltr);
            A("auto-direction", "العربية والاتجاه", "اكتشاف الاتجاه تلقائيًا", "اختيار RTL أو LTR حسب النص المحدد.", "تلقائي لغة", AdvancedCommands.AutoDirection);
            A("remove-diacritics", "العربية والاتجاه", "حذف التشكيل", "حذف الحركات والعلامات العربية.", "حركات", Commands.RemoveDiacritics);
            A("remove-tatweel", "العربية والاتجاه", "حذف التطويل", "حذف الكشيدة ـ من النص.", "كشيدة", Commands.RemoveTatweel);
            A("normalize-arabic", "العربية والاتجاه", "توحيد الحروف العربية", "توحيد أشكال الألف والياء والهمزات.", "الف همزة ياء", Commands.NormalizeArabic);
            A("normalize-arabic-punctuation", "العربية والاتجاه", "ترقيم عربي ذكي", "تحويل الفاصلة والفاصلة المنقوطة وعلامة السؤال إلى العربية وضبط المسافات.", "فاصلة ترقيم", AdvancedCommands.NormalizeArabicPunctuation);
            A("arabic-quotes", "العربية والاتجاه", "علامات اقتباس عربية", "تحويل علامات الاقتباس إلى « ».", "اقتباس", AdvancedCommands.ArabicQuotes);
            A("remove-zero-width", "العربية والاتجاه", "حذف المحارف المخفية", "حذف محارف zero-width وBOM من النص.", "مخفي", AdvancedCommands.RemoveZeroWidth);
            A("digits-eastern", "العربية والاتجاه", "أرقام عربية", "تحويل الأرقام الغربية إلى ٠١٢٣.", "ارقام", Commands.ToEastern);
            A("digits-western", "العربية والاتجاه", "أرقام غربية", "تحويل الأرقام العربية والفارسية إلى 0123.", "ارقام", Commands.ToWestern);
            A("keyboard-ar-to-en", "العربية والاتجاه", "تصحيح كتابة عربية إلى إنجليزية", "إصلاح نص كُتب بلوحة مفاتيح عربية بدل الإنجليزية.", "كيبورد لوحة", AdvancedCommands.KeyboardArabicToEnglish);
            A("keyboard-en-to-ar", "العربية والاتجاه", "تصحيح كتابة إنجليزية إلى عربية", "إصلاح نص كُتب بلوحة مفاتيح إنجليزية بدل العربية.", "كيبورد لوحة", AdvancedCommands.KeyboardEnglishToArabic);
            A("insert-arabic-date", "العربية والاتجاه", "إدراج التاريخ بالعربية", "إدراج التاريخ الميلادي بأسماء الأشهر العربية.", "تاريخ", AdvancedCommands.InsertArabicDate);
            A("insert-hijri-date", "العربية والاتجاه", "إدراج التاريخ الهجري", "إدراج تاريخ أم القرى الحالي.", "هجري تاريخ", AdvancedCommands.InsertHijriDate);
            A("reverse-text", "العربية والاتجاه", "عكس النص", "عكس ترتيب الأحرف في النص المحدد.", "عكس", AdvancedCommands.ReverseText);
            A("number-arabic-words", "العربية والاتجاه", "رقم إلى كلمات عربية", "تحويل الرقم المحدد إلى كلمات عربية.", "تفقيط أرقام", AdvancedCommands.NumberToArabicWords);
            A("arabic-clean-all", "العربية والاتجاه", "تنظيف عربي شامل", "حذف التشكيل والتطويل وضبط RTL وArial 14.", "تنظيف شامل", Commands.ArabicCleanAll);

            // النص والتحرير
            A("collapse-spaces", "النص والتحرير", "تنظيف المسافات", "حذف المسافات المتكررة.", "مسافة", Commands.CollapseSpaces);
            A("trim-lines", "النص والتحرير", "قص حواف الأسطر", "حذف المسافات من بداية ونهاية كل سطر.", "trim", AdvancedCommands.TrimLines);
            A("remove-empty-paragraphs", "النص والتحرير", "حذف الفقرات الفارغة", "اختصار الفقرات الفارغة المتكررة.", "فراغ", Commands.RemoveEmptyParagraphs);
            A("join-paragraphs", "النص والتحرير", "دمج الفقرات", "تحويل الفقرات المحددة إلى فقرة واحدة.", "دمج", AdvancedCommands.JoinParagraphs);
            A("tabs-to-spaces", "النص والتحرير", "Tabs إلى مسافات", "استبدال علامات الجدولة بمسافات.", "tab", Commands.TabsToSpaces);
            A("linebreaks-to-paragraphs", "النص والتحرير", "فواصل الأسطر إلى فقرات", "تحويل line breaks إلى paragraph breaks.", "سطر فقرة", Commands.LineBreaksToParagraphs);
            A("remove-duplicate-lines", "النص والتحرير", "حذف الأسطر المكررة", "الاحتفاظ بأول ظهور لكل سطر.", "مكرر", Commands.RemoveDuplicateLines);
            A("remove-duplicate-paragraphs", "النص والتحرير", "حذف الفقرات المكررة", "حذف الفقرات المتكررة مع تجاهل اختلاف المسافات.", "تكرار", AdvancedCommands.RemoveDuplicateParagraphs);
            A("remove-duplicate-words", "النص والتحرير", "حذف الكلمات المتتابعة المكررة", "تحويل كلمة كلمة إلى كلمة واحدة.", "مكرر كلمات", AdvancedCommands.RemoveDuplicateConsecutiveWords);
            A("sort-lines-asc", "النص والتحرير", "فرز تصاعدي", "فرز الأسطر تصاعديًا.", "ترتيب", () => Commands.SortLines(false));
            A("sort-lines-desc", "النص والتحرير", "فرز تنازلي", "فرز الأسطر تنازليًا.", "ترتيب", () => Commands.SortLines(true));
            A("prefix-lines", "النص والتحرير", "إضافة بادئة للأسطر", "إضافة نص قبل كل سطر.", "بادئة", AdvancedCommands.PrefixLines);
            A("suffix-lines", "النص والتحرير", "إضافة لاحقة للأسطر", "إضافة نص بعد كل سطر.", "لاحقة", AdvancedCommands.SuffixLines);
            A("split-by-delimiter", "النص والتحرير", "تقسيم حسب فاصل", "استبدال فاصل محدد بفواصل فقرات.", "تقسيم", AdvancedCommands.SplitByDelimiter);
            A("paragraphs-bullets", "النص والتحرير", "تحويل إلى نقاط", "تحويل الفقرات إلى قائمة نقطية.", "قائمة", AdvancedCommands.ParagraphsToBullets);
            A("paragraphs-numbers", "النص والتحرير", "تحويل إلى ترقيم", "تحويل الفقرات إلى قائمة مرقمة.", "قائمة أرقام", AdvancedCommands.ParagraphsToNumbers);
            A("clear-list", "النص والتحرير", "إزالة تنسيق القائمة", "إزالة الترقيم أو التعداد من الفقرات.", "قائمة", AdvancedCommands.ClearList);
            A("uppercase", "النص والتحرير", "UPPERCASE", "تحويل الأحرف الإنجليزية إلى كبيرة.", "case", Commands.Uppercase);
            A("lowercase", "النص والتحرير", "lowercase", "تحويل الأحرف الإنجليزية إلى صغيرة.", "case", Commands.Lowercase);
            A("titlecase", "النص والتحرير", "Title Case", "تكبير بدايات الكلمات الإنجليزية.", "case", Commands.TitleCase);
            A("sentence-case", "النص والتحرير", "Sentence case", "تكبير بداية كل جملة.", "case", AdvancedCommands.SentenceCase);
            A("invert-case", "النص والتحرير", "Invert Case", "عكس حالة الأحرف الكبيرة والصغيرة.", "case", AdvancedCommands.InvertCase);
            A("smart-quotes", "النص والتحرير", "اقتباس ذكي", "تحويل الاقتباسات المستقيمة إلى ذكية.", "quotes", AdvancedCommands.SmartQuotes);
            A("normalize-dashes", "النص والتحرير", "توحيد الشرطات", "توحيد hyphen وen dash وem dash.", "شرطة", AdvancedCommands.NormalizeDashes);
            A("remove-nonprinting", "النص والتحرير", "حذف المحارف غير المطبوعة", "حذف المحارف التحكمية غير المرئية.", "control", AdvancedCommands.RemoveNonPrinting);
            A("remove-hyperlinks", "النص والتحرير", "إزالة الروابط", "حذف الارتباطات مع إبقاء النص.", "روابط", Commands.RemoveHyperlinks);
            A("extract-emails", "النص والتحرير", "استخراج البريد الإلكتروني", "استخراج عناوين البريد من النص.", "email", AdvancedCommands.ExtractEmails);
            A("extract-urls", "النص والتحرير", "استخراج الروابط", "استخراج الروابط من النص.", "url", AdvancedCommands.ExtractUrls);
            A("extract-phones", "النص والتحرير", "استخراج أرقام الهواتف", "استخراج أرقام الهواتف المحتملة.", "phone", AdvancedCommands.ExtractPhones);
            A("text-statistics", "النص والتحرير", "إحصاءات النص", "عرض الكلمات والأحرف والفقرات والجمل.", "عدد احصاء", AdvancedCommands.TextStatistics);
            A("paste-plain", "النص والتحرير", "لصق كنص فقط", "لصق محتوى الحافظة دون تنسيق.", "لصق", AdvancedCommands.PastePlainText);

            // التنسيق والأنماط
            A("font-arial-14", "التنسيق والأنماط", "Arial 14", "تطبيق Arial 14 عربي وإنجليزي.", "خط", Commands.Arial14);
            A("font-tahoma-12", "التنسيق والأنماط", "Tahoma 12", "تطبيق Tahoma 12.", "خط", () => AdvancedCommands.FontPreset("Tahoma", 12));
            A("font-times-12", "التنسيق والأنماط", "Times New Roman 12", "تطبيق Times New Roman 12.", "خط", () => AdvancedCommands.FontPreset("Times New Roman", 12));
            A("font-calibri-11", "التنسيق والأنماط", "Calibri 11", "تطبيق Calibri 11.", "خط", () => AdvancedCommands.FontPreset("Calibri", 11));
            A("clear-formatting", "التنسيق والأنماط", "إزالة التنسيق المباشر", "إرجاع النص إلى تنسيق النمط.", "تنسيق", Commands.ClearFormatting);
            A("align-right", "التنسيق والأنماط", "محاذاة يمين", "محاذاة الفقرات إلى اليمين.", "محاذاة", () => Commands.Align(2));
            A("align-left", "التنسيق والأنماط", "محاذاة يسار", "محاذاة الفقرات إلى اليسار.", "محاذاة", () => Commands.Align(0));
            A("align-center", "التنسيق والأنماط", "توسيط", "توسيط الفقرات.", "محاذاة", () => Commands.Align(1));
            A("align-justify", "التنسيق والأنماط", "ضبط كامل", "ضبط الفقرات من الطرفين.", "محاذاة", () => Commands.Align(3));
            A("line-spacing-1", "التنسيق والأنماط", "تباعد مفرد", "تباعد أسطر مفرد.", "تباعد", () => AdvancedCommands.LineSpacingRule(0));
            A("line-spacing-115", "التنسيق والأنماط", "تباعد 1.15", "تباعد أسطر 1.15.", "تباعد", () => Commands.LineSpacing(13.8f));
            A("line-spacing-15", "التنسيق والأنماط", "تباعد 1.5", "تباعد أسطر 1.5.", "تباعد", () => Commands.LineSpacing(18f));
            A("line-spacing-2", "التنسيق والأنماط", "تباعد مزدوج", "تباعد أسطر مزدوج.", "تباعد", () => AdvancedCommands.LineSpacingRule(2));
            A("space-after-0", "التنسيق والأنماط", "مسافة بعد 0", "إلغاء المسافة بعد الفقرة.", "فقرة", () => AdvancedCommands.SpaceAfter(0));
            A("space-after-6", "التنسيق والأنماط", "مسافة بعد 6", "تعيين 6 نقاط بعد الفقرة.", "فقرة", () => AdvancedCommands.SpaceAfter(6));
            A("space-after-12", "التنسيق والأنماط", "مسافة بعد 12", "تعيين 12 نقطة بعد الفقرة.", "فقرة", () => AdvancedCommands.SpaceAfter(12));
            A("indent-first-line", "التنسيق والأنماط", "مسافة بادئة للسطر الأول", "تطبيق مسافة بادئة 0.75 سم.", "indent", () => AdvancedCommands.FirstLineIndent(21.26f));
            A("indent-remove", "التنسيق والأنماط", "إزالة المسافات البادئة", "إزالة إزاحات الفقرة.", "indent", AdvancedCommands.RemoveIndents);
            A("keep-with-next", "التنسيق والأنماط", "إبقاء مع التالي", "منع فصل العنوان عن الفقرة التالية.", "صفحة", AdvancedCommands.KeepWithNext);
            A("page-break-before", "التنسيق والأنماط", "بدء الفقرة في صفحة جديدة", "تطبيق Page break before.", "صفحة", AdvancedCommands.PageBreakBefore);
            A("highlight-yellow", "التنسيق والأنماط", "تمييز أصفر", "تمييز النص باللون الأصفر.", "لون", AdvancedCommands.HighlightYellow);
            A("highlight-clear", "التنسيق والأنماط", "إزالة التمييز", "إزالة لون التمييز.", "لون", AdvancedCommands.ClearHighlight);
            A("border-box", "التنسيق والأنماط", "إطار حول النص", "إضافة حدود حول الفقرات المحددة.", "حدود", AdvancedCommands.BorderBox);
            A("border-clear", "التنسيق والأنماط", "إزالة الحدود", "إزالة حدود الفقرات.", "حدود", AdvancedCommands.ClearBorders);
            A("heading-1", "التنسيق والأنماط", "عنوان 1", "تطبيق Heading 1.", "نمط", () => Commands.Style("Heading 1"));
            A("heading-2", "التنسيق والأنماط", "عنوان 2", "تطبيق Heading 2.", "نمط", () => Commands.Style("Heading 2"));
            A("heading-3", "التنسيق والأنماط", "عنوان 3", "تطبيق Heading 3.", "نمط", () => Commands.Style("Heading 3"));
            A("style-normal", "التنسيق والأنماط", "نمط عادي", "تطبيق Normal.", "نمط", () => Commands.Style("Normal"));
            A("format-arabic-report", "التنسيق والأنماط", "تقرير عربي", "تنسيق تقرير عربي احترافي.", "تقرير", Commands.ArabicReport);
            A("format-official-letter", "التنسيق والأنماط", "خطاب رسمي", "ضبط الصفحة والخط والفقرات لخطاب عربي رسمي.", "خطاب", AdvancedCommands.OfficialLetterFormat);
            A("format-academic", "التنسيق والأنماط", "تنسيق أكاديمي", "ضبط الصفحة والخطوط والتباعد لبحث أكاديمي.", "بحث", AdvancedCommands.AcademicFormat);
            A("format-copy", "التنسيق والأنماط", "حفظ لقطة تنسيق", "حفظ تنسيق النص الحالي داخل الجلسة.", "نسخ تنسيق", AdvancedCommands.CopyFormatSnapshot);
            A("format-apply", "التنسيق والأنماط", "تطبيق لقطة التنسيق", "تطبيق آخر تنسيق محفوظ.", "لصق تنسيق", AdvancedCommands.ApplyFormatSnapshot);

            // الجداول
            A("table-rtl", "الجداول", "جدول RTL", "اتجاه الجدول من اليمين إلى اليسار.", "جدول", Commands.TableRtl);
            A("table-ltr", "الجداول", "جدول LTR", "اتجاه الجدول من اليسار إلى اليمين.", "جدول", AdvancedCommands.TableLtr);
            A("table-professional", "الجداول", "تنسيق جدول احترافي", "حدود وعنوان وتوسيط وملاءمة الصفحة.", "فخم جدول", AdvancedCommands.TableProfessional);
            A("table-header-style", "الجداول", "تنسيق صف العنوان", "تظليل وتغليظ وتكرار الصف الأول.", "عنوان", AdvancedCommands.TableHeaderStyle);
            A("table-banded-rows", "الجداول", "صفوف متبادلة", "تظليل صفوف الجدول بالتناوب.", "تظليل", AdvancedCommands.TableBandedRows);
            A("table-autofit-window", "الجداول", "ملاءمة الصفحة", "ملاءمة عرض الجدول للصفحة.", "عرض", () => Commands.TableAutoFit(2));
            A("table-autofit-content", "الجداول", "ملاءمة المحتوى", "ملاءمة عرض الأعمدة للمحتوى.", "عرض", () => Commands.TableAutoFit(1));
            A("table-distribute-columns", "الجداول", "توزيع الأعمدة", "توزيع الأعمدة بالتساوي.", "أعمدة", Commands.TableDistributeColumns);
            A("table-distribute-rows", "الجداول", "توزيع الصفوف", "توزيع الصفوف بالتساوي.", "صفوف", Commands.TableDistributeRows);
            A("table-repeat-header", "الجداول", "تكرار صف العنوان", "تكرار الصف الأول في كل صفحة.", "عنوان", Commands.TableRepeatHeader);
            A("table-remove-empty-rows", "الجداول", "حذف الصفوف الفارغة", "حذف الصفوف التي لا تحتوي بيانات.", "صفوف", Commands.TableRemoveEmptyRows);
            A("table-remove-duplicate-rows", "الجداول", "حذف الصفوف المكررة", "حذف الصفوف المتطابقة.", "مكرر", AdvancedCommands.TableRemoveDuplicateRows);
            A("table-trim-cells", "الجداول", "تنظيف الخلايا", "حذف المسافات الزائدة داخل الخلايا.", "تنظيف", AdvancedCommands.TableTrimCells);
            A("table-center-cells", "الجداول", "توسيط الخلايا", "توسيط أفقي وعمودي.", "توسيط", Commands.TableCenterCells);
            A("table-number-first-column", "الجداول", "ترقيم العمود الأول", "إضافة تسلسل رقمي للصفوف.", "ترقيم", AdvancedCommands.TableNumberFirstColumn);
            A("table-sum-column", "الجداول", "مجموع العمود", "إضافة صف مجموع للعمود الحالي.", "حساب", () => AdvancedCommands.TableColumnAggregate(false));
            A("table-average-column", "الجداول", "متوسط العمود", "إضافة صف متوسط للعمود الحالي.", "حساب", () => AdvancedCommands.TableColumnAggregate(true));
            A("table-sort-asc", "الجداول", "فرز الجدول تصاعديًا", "فرز الصفوف حسب العمود الحالي.", "ترتيب", () => AdvancedCommands.TableSort(false));
            A("table-sort-desc", "الجداول", "فرز الجدول تنازليًا", "فرز الصفوف تنازليًا حسب العمود الحالي.", "ترتيب", () => AdvancedCommands.TableSort(true));
            A("table-row-above", "الجداول", "صف أعلى", "إضافة صف فوق الخلية الحالية.", "إضافة صف", AdvancedCommands.TableAddRowAbove);
            A("table-row-below", "الجداول", "صف أسفل", "إضافة صف في نهاية الجدول.", "إضافة صف", AdvancedCommands.TableAddRowBelow);
            A("table-col-left", "الجداول", "عمود يسار", "إضافة عمود قبل العمود الحالي.", "إضافة عمود", AdvancedCommands.TableAddColumnLeft);
            A("table-col-right", "الجداول", "عمود يمين", "إضافة عمود في نهاية الجدول.", "إضافة عمود", AdvancedCommands.TableAddColumnRight);
            A("table-delete-row", "الجداول", "حذف الصف", "حذف الصف الحالي.", "حذف", AdvancedCommands.TableDeleteRow);
            A("table-delete-column", "الجداول", "حذف العمود", "حذف العمود الحالي.", "حذف", AdvancedCommands.TableDeleteColumn);
            A("table-merge-cells", "الجداول", "دمج الخلايا", "دمج الخلايا المحددة.", "دمج", AdvancedCommands.TableMergeCells);
            A("table-split-cell", "الجداول", "تقسيم الخلية", "تقسيم الخلية إلى صفوف وأعمدة.", "تقسيم", AdvancedCommands.TableSplitCell);
            A("table-borders-all", "الجداول", "كل الحدود", "إظهار جميع حدود الجدول.", "حدود", () => AdvancedCommands.TableBorders(true));
            A("table-borders-none", "الجداول", "بدون حدود", "إخفاء حدود الجدول.", "حدود", () => AdvancedCommands.TableBorders(false));
            A("table-vertical-top", "الجداول", "محاذاة عمودية أعلى", "محاذاة محتوى الخلايا للأعلى.", "عمودي", () => AdvancedCommands.TableVertical(0));
            A("table-vertical-middle", "الجداول", "محاذاة عمودية وسط", "محاذاة محتوى الخلايا للوسط.", "عمودي", () => AdvancedCommands.TableVertical(1));
            A("table-vertical-bottom", "الجداول", "محاذاة عمودية أسفل", "محاذاة محتوى الخلايا للأسفل.", "عمودي", () => AdvancedCommands.TableVertical(3));
            A("text-to-table", "الجداول", "تحويل نص إلى جدول", "تحويل نص مفصول بعلامات إلى جدول.", "تحويل", AdvancedCommands.TextToTable);
            A("table-to-text", "الجداول", "تحويل جدول إلى نص", "تحويل الجدول إلى نص مفصول.", "تحويل", Commands.TableToText);
            A("table-transpose", "الجداول", "تبديل الصفوف والأعمدة", "Transpose للجدول الحالي.", "تبديل", AdvancedCommands.TableTranspose);

            // المستند والصفحات
            A("update-fields", "المستند والصفحات", "تحديث جميع الحقول", "تحديث الحقول والفهارس.", "حقول", Commands.UpdateFields);
            A("insert-toc", "المستند والصفحات", "إضافة فهرس محتويات", "إدراج فهرس من العناوين 1 إلى 3.", "فهرس", Commands.InsertToc);
            A("page-numbers-add", "المستند والصفحات", "إضافة أرقام الصفحات", "إضافة الترقيم إلى التذييل.", "ترقيم صفحة", Commands.AddPageNumbers);
            A("page-numbers-remove", "المستند والصفحات", "حذف أرقام الصفحات", "حذف حقول PAGE من التذييلات.", "ترقيم صفحة", Commands.RemovePageNumbers);
            A("insert-page-break", "المستند والصفحات", "فاصل صفحة", "إدراج فاصل صفحة يدوي.", "فاصل", AdvancedCommands.InsertPageBreak);
            A("insert-section-next", "المستند والصفحات", "مقطع صفحة جديدة", "إدراج Section Break Next Page.", "مقطع", AdvancedCommands.InsertSectionNext);
            A("insert-section-continuous", "المستند والصفحات", "مقطع مستمر", "إدراج Section Break Continuous.", "مقطع", AdvancedCommands.InsertSectionContinuous);
            A("remove-page-breaks", "المستند والصفحات", "حذف فواصل الصفحات", "حذف جميع فواصل الصفحات اليدوية.", "فاصل", AdvancedCommands.RemovePageBreaks);
            A("remove-section-breaks", "المستند والصفحات", "حذف فواصل المقاطع", "حذف جميع فواصل المقاطع.", "مقطع", AdvancedCommands.RemoveSectionBreaks);
            A("page-a4", "المستند والصفحات", "حجم A4", "ضبط الورق على A4.", "ورق", Commands.SetA4);
            A("page-a3", "المستند والصفحات", "حجم A3", "ضبط الورق على A3.", "ورق", () => AdvancedCommands.SetPaper(6));
            A("page-letter", "المستند والصفحات", "حجم Letter", "ضبط الورق على Letter.", "ورق", () => AdvancedCommands.SetPaper(2));
            A("page-portrait", "المستند والصفحات", "اتجاه عمودي", "جعل الصفحة عمودية.", "اتجاه", () => Commands.Orientation(0));
            A("page-landscape", "المستند والصفحات", "اتجاه أفقي", "جعل الصفحة أفقية.", "اتجاه", () => Commands.Orientation(1));
            A("margins-normal", "المستند والصفحات", "هوامش عادية", "ضبط الهوامش إلى 2.54 سم.", "هوامش", () => Commands.Margins(72f));
            A("margins-narrow", "المستند والصفحات", "هوامش ضيقة", "ضبط الهوامش إلى 1.27 سم.", "هوامش", () => Commands.Margins(36f));
            A("columns-1", "المستند والصفحات", "عمود واحد", "إرجاع النص إلى عمود واحد.", "أعمدة", () => AdvancedCommands.Columns(1));
            A("columns-2", "المستند والصفحات", "عمودان", "تقسيم الصفحة إلى عمودين.", "أعمدة", () => AdvancedCommands.Columns(2));
            A("columns-3", "المستند والصفحات", "ثلاثة أعمدة", "تقسيم الصفحة إلى ثلاثة أعمدة.", "أعمدة", () => AdvancedCommands.Columns(3));
            A("line-numbers-on", "المستند والصفحات", "تشغيل أرقام الأسطر", "إظهار ترقيم الأسطر.", "أسطر", () => AdvancedCommands.LineNumbers(true));
            A("line-numbers-off", "المستند والصفحات", "إيقاف أرقام الأسطر", "إخفاء ترقيم الأسطر.", "أسطر", () => AdvancedCommands.LineNumbers(false));
            A("header-text", "المستند والصفحات", "نص رأس الصفحة", "كتابة رأس موحد في جميع المقاطع.", "هيدر", AdvancedCommands.SetHeaderText);
            A("footer-text", "المستند والصفحات", "نص تذييل الصفحة", "كتابة تذييل موحد في جميع المقاطع.", "فوتر", AdvancedCommands.SetFooterText);
            A("clear-headers-footers", "المستند والصفحات", "مسح الرؤوس والتذييلات", "حذف نصوص الرؤوس والتذييلات.", "هيدر فوتر", AdvancedCommands.ClearHeadersFooters);
            A("insert-date-time", "المستند والصفحات", "إدراج تاريخ ووقت", "إدراج التاريخ والوقت الحاليين.", "تاريخ", AdvancedCommands.InsertDateTime);
            A("insert-file-name", "المستند والصفحات", "إدراج اسم ومسار الملف", "إدراج حقل FILENAME مع المسار.", "مسار", AdvancedCommands.InsertFileNameField);
            A("document-statistics", "المستند والصفحات", "إحصاءات المستند", "عرض الصفحات والكلمات والجداول والصور.", "احصاء", AdvancedCommands.DocumentStatistics);
            A("list-fonts", "المستند والصفحات", "قائمة خطوط الأنماط", "عرض الخطوط المستخدمة في أنماط المستند.", "خطوط", AdvancedCommands.ListStyleFonts);
            A("replace-font", "المستند والصفحات", "استبدال خط", "استبدال خط بآخر في المستند.", "خط", AdvancedCommands.ReplaceFont);
            A("remove-hidden-text", "المستند والصفحات", "حذف النص المخفي", "حذف كل النصوص ذات خاصية Hidden.", "مخفي", AdvancedCommands.RemoveHiddenText);
            A("unlink-fields", "المستند والصفحات", "تحويل الحقول إلى نص", "إلغاء ارتباط الحقول مع إبقاء النتائج.", "حقول", AdvancedCommands.UnlinkFields);
            A("remove-bookmarks", "المستند والصفحات", "حذف الإشارات المرجعية", "حذف جميع Bookmarks.", "bookmark", AdvancedCommands.RemoveBookmarks);
            A("insert-cover", "المستند والصفحات", "إنشاء صفحة غلاف", "إضافة غلاف منسق في بداية المستند.", "غلاف", AdvancedCommands.InsertCoverPage);
            A("insert-signature", "المستند والصفحات", "كتلة توقيع", "إدراج اسم وصفة وتوقيع وتاريخ.", "توقيع", AdvancedCommands.InsertSignatureBlock);

            // المراجعة والأمان
            A("track-on", "المراجعة والأمان", "تشغيل تعقب التغييرات", "تشغيل Track Changes.", "تعقب", () => AdvancedCommands.TrackChanges(true));
            A("track-off", "المراجعة والأمان", "إيقاف تعقب التغييرات", "إيقاف Track Changes.", "تعقب", () => AdvancedCommands.TrackChanges(false));
            A("remove-comments", "المراجعة والأمان", "حذف التعليقات", "حذف جميع التعليقات.", "تعليقات", Commands.RemoveComments);
            A("accept-revisions", "المراجعة والأمان", "قبول كل التعديلات", "قبول جميع التغييرات المتعقبة.", "تعديلات", Commands.AcceptRevisions);
            A("reject-revisions", "المراجعة والأمان", "رفض كل التعديلات", "رفض جميع التغييرات المتعقبة.", "تعديلات", Commands.RejectRevisions);
            A("protect-readonly", "المراجعة والأمان", "حماية للقراءة فقط", "حماية المستند بكلمة مرور ضد التحرير.", "حماية", () => AdvancedCommands.ProtectDocument(3));
            A("protect-forms", "المراجعة والأمان", "حماية تعبئة النماذج", "السماح بتعبئة النماذج فقط.", "حماية", () => AdvancedCommands.ProtectDocument(2));
            A("unprotect", "المراجعة والأمان", "إلغاء الحماية", "إلغاء حماية المستند بكلمة المرور.", "حماية", AdvancedCommands.UnprotectDocument);
            A("remove-metadata", "المراجعة والأمان", "إزالة البيانات الشخصية", "حذف معلومات المستند الشخصية.", "خصوصية", Commands.RemoveMetadata);
            A("clean-properties", "المراجعة والأمان", "تنظيف الخصائص المخصصة", "حذف البيانات الشخصية والخصائص المخصصة.", "خصوصية", AdvancedCommands.CleanProperties);
            A("remove-external-links", "المراجعة والأمان", "فصل الروابط الخارجية", "فصل روابط البيانات الخارجية.", "روابط", AdvancedCommands.RemoveExternalLinks);
            A("workflow-final-delivery", "المراجعة والأمان", "تجهيز نسخة نهائية", "نسخة احتياطية ثم قبول التعديلات وحذف التعليقات وتحديث الحقول.", "نهائي", Commands.FinalDelivery);
            A("workflow-final-share", "المراجعة والأمان", "تنظيف شامل قبل المشاركة", "نسخة احتياطية وتنظيف مراجعة وخصوصية ونص مخفي.", "مشاركة", AdvancedCommands.FinalShareWorkflow);

            // الملفات وPDF
            A("save-backup", "الملفات وPDF", "نسخة احتياطية مؤرخة", "حفظ نسخة احتياطية دون تغيير المستند المفتوح.", "حفظ", Commands.SaveBackup);
            A("save-version", "الملفات وPDF", "حفظ إصدار جديد", "حفظ نسخة باسم يحتوي التاريخ والوقت.", "نسخة", AdvancedCommands.SaveVersionedCopy);
            A("save-docx-copy", "الملفات وPDF", "نسخة DOCX", "إنشاء نسخة DOCX مستقلة.", "docx", AdvancedCommands.SaveDocxCopy);
            A("save-all", "الملفات وPDF", "حفظ كل المستندات", "حفظ جميع مستندات Word المفتوحة.", "حفظ", AdvancedCommands.SaveAllDocuments);
            A("export-pdf", "الملفات وPDF", "تصدير PDF", "تصدير المستند إلى PDF.", "pdf", () => Commands.ExportPdf(false));
            A("export-pdfa", "الملفات وPDF", "تصدير PDF/A", "تصدير أرشيفي PDF/A.", "pdfa", () => Commands.ExportPdf(true));
            A("export-selection-pdf", "الملفات وPDF", "تصدير التحديد PDF", "تصدير الجزء المحدد فقط إلى PDF.", "pdf تحديد", AdvancedCommands.ExportSelectionPdf);
            A("export-sections-pdf", "الملفات وPDF", "كل مقطع PDF مستقل", "تصدير كل Section إلى ملف PDF منفصل.", "pdf مقاطع", AdvancedCommands.ExportSectionsPdf);
            A("batch-pdf", "الملفات وPDF", "تحويل مجلد إلى PDF", "تحويل ملفات Word داخل مجلد إلى PDF دفعة واحدة.", "دفعي", AdvancedCommands.BatchPdf);
            A("merge-documents", "الملفات وPDF", "دمج مستندات", "إدراج عدة مستندات في المستند الحالي.", "دمج ملفات", AdvancedCommands.MergeDocuments);
            A("open-document-folder", "الملفات وPDF", "فتح موقع المستند", "فتح Explorer وتحديد الملف الحالي.", "مجلد", AdvancedCommands.OpenDocumentFolder);
            A("copy-document-path", "الملفات وPDF", "نسخ مسار المستند", "نسخ المسار الكامل إلى الحافظة.", "مسار", AdvancedCommands.CopyDocumentPath);
            A("archive-package", "الملفات وPDF", "حزمة أرشيف", "إنشاء مجلد يحتوي نسخة Word وPDF ومعلومات الحزمة.", "أرشيف", AdvancedCommands.ArchivePackage);

            // القوالب
            A("template-agenda", "القوالب", "جدول أعمال", "إدراج قالب جدول أعمال اجتماع.", "اجتماع", () => Commands.InsertTemplate("agenda"));
            A("template-minutes", "القوالب", "محضر اجتماع", "إدراج قالب محضر اجتماع.", "اجتماع", () => Commands.InsertTemplate("minutes"));
            A("template-official-letter", "القوالب", "خطاب رسمي", "إدراج قالب خطاب رسمي.", "خطاب", () => Commands.InsertTemplate("letter"));
            A("template-medical-report", "القوالب", "تقرير طبي", "إدراج قالب تقرير طبي.", "طبي", () => Commands.InsertTemplate("medical"));
            A("template-project-report", "القوالب", "تقرير مشروع", "إدراج قالب تقرير مشروع.", "مشروع", () => Commands.InsertTemplate("project"));
            A("template-handover", "القوالب", "تسليم واستلام", "إدراج محضر تسليم واستلام.", "تسليم", () => Commands.InsertTemplate("handover"));
            A("template-memo", "القوالب", "مذكرة داخلية", "إدراج قالب مذكرة.", "مذكرة", () => AdvancedCommands.InsertMoreTemplate("memo"));
            A("template-quotation", "القوالب", "عرض سعر", "إدراج قالب عرض سعر.", "سعر", () => AdvancedCommands.InsertMoreTemplate("quotation"));
            A("template-invoice", "القوالب", "فاتورة", "إدراج قالب فاتورة.", "فاتورة", () => AdvancedCommands.InsertMoreTemplate("invoice"));
            A("template-certificate", "القوالب", "شهادة تقدير", "إدراج قالب شهادة.", "شهادة", () => AdvancedCommands.InsertMoreTemplate("certificate"));
            A("template-attendance", "القوالب", "كشف حضور", "إدراج كشف حضور.", "حضور", () => AdvancedCommands.InsertMoreTemplate("attendance"));
            A("template-tor", "القوالب", "شروط مرجعية ToR", "إدراج هيكل شروط مرجعية.", "tor", () => AdvancedCommands.InsertMoreTemplate("tor"));
            A("template-concept-note", "القوالب", "مذكرة مفاهيمية", "إدراج هيكل Concept Note.", "مشروع", () => AdvancedCommands.InsertMoreTemplate("concept"));
            A("template-donor-report", "القوالب", "تقرير جهة مانحة", "إدراج هيكل تقرير مانحين.", "مانح", () => AdvancedCommands.InsertMoreTemplate("donor"));
            A("template-sop", "القوالب", "إجراء تشغيلي SOP", "إدراج هيكل SOP.", "اجراء", () => AdvancedCommands.InsertMoreTemplate("sop"));
            A("template-risk-register", "القوالب", "سجل مخاطر", "إدراج جدول سجل مخاطر.", "خطر", () => AdvancedCommands.InsertMoreTemplate("risk"));
            A("template-action-plan", "القوالب", "خطة عمل", "إدراج جدول خطة عمل.", "خطة", () => AdvancedCommands.InsertMoreTemplate("action"));
            A("template-cover-letter", "القوالب", "خطاب تغطية", "إدراج قالب Cover Letter عربي.", "خطاب", () => AdvancedCommands.InsertMoreTemplate("cover-letter"));

            // الأدوات الذكية
            A("calculator", "الأدوات الذكية", "حاسبة التحديد", "حساب تعبير رياضي محدد وإدراج النتيجة.", "حساب", AdvancedCommands.Calculator);
            A("insert-guid", "الأدوات الذكية", "إدراج UUID", "إدراج معرف فريد عالمي.", "guid uuid", AdvancedCommands.InsertGuid);
            A("insert-lorem-arabic", "الأدوات الذكية", "نص عربي تجريبي", "إدراج فقرات عربية لتجربة التصميم.", "lorem", AdvancedCommands.InsertArabicLorem);
            A("insert-checklist", "الأدوات الذكية", "قائمة مهام بمربعات", "تحويل بنود إلى قائمة ☐.", "checklist", AdvancedCommands.InsertChecklist);
            A("open-log", "الأدوات الذكية", "فتح سجل الإضافة", "فتح أحدث ملف Log.", "سجل", AdvancedCommands.OpenLog, false);

            // النظام والترخيص
            A("command-center", "النظام والترخيص", "مركز الأدوات الاحترافي", "بحث وفئات ومفضلة وآخر استخدام لكل الأدوات.", "مركز", CommandCenterForm.ShowCenter, false);
            A("activate", "النظام والترخيص", "تفعيل البرنامج", "إدخال Serial Number وتفعيل النسخة.", "ترخيص", AdvancedCommands.Activate, false);
            A("license-info", "النظام والترخيص", "حالة الترخيص", "عرض حالة الترخيص ومعرّف الجهاز.", "ترخيص", AdvancedCommands.ShowLicenseInfo, false);
            A("machine-id", "النظام والترخيص", "نسخ معرّف الجهاز", "نسخ Machine ID لإرساله إلى مالك البرنامج.", "ترخيص", AdvancedCommands.ShowMachineId, false);
            A("health-check", "النظام والترخيص", "فحص الإضافة", "عرض حالة الاتصال وإصدار Word والسجل.", "فحص", SystemCommands.HealthCheck, false);
            A("about", "النظام والترخيص", "حول WordPro Suite", "معلومات الإصدار والتقنية.", "حول", SystemCommands.About, false);
        }

        internal static IEnumerable<CommandDescriptor> All => Map.Values;

        private static void A(string id, string category, string title, string description, string keywords, Action action, bool requiresLicense = true)
        {
            Map.Add(id, new CommandDescriptor(id, category, title, description, keywords, action, requiresLicense));
        }

        internal static void Execute(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) return;
            CommandDescriptor command;
            if (!Map.TryGetValue(id, out command))
            {
                MessageBox.Show("الأداة غير مسجلة: " + id, "WordPro Suite");
                return;
            }

            if (command.RequiresLicense && !LicenseManager.EnsureLicensedWithUi())
            {
                MessageBox.Show("يلزم تفعيل البرنامج أو وجود فترة تجريبية فعالة.", "WordPro Suite", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Logger.Info("Start " + id);
                command.Execute();
                CommandUsageStore.RecordRun(id);
                Logger.Info("Done " + id);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed " + id, ex);
                MessageBox.Show(ex.Message, "WordPro Suite", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
