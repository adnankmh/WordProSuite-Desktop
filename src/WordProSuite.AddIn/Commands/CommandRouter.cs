using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WordProSuite.Desktop.Infrastructure;
using WordProSuite.Desktop.UI;

namespace WordProSuite.Desktop.Commands
{
    internal sealed class CommandDescriptor
    {
        internal CommandDescriptor(string id,string category,string title,string description,Action execute)
        { Id=id; Category=category; Title=title; Description=description; Execute=execute; }
        internal string Id {get;}
        internal string Category {get;}
        internal string Title {get;}
        internal string Description {get;}
        internal Action Execute {get;}
    }

    internal static class CommandRouter
    {
        private static readonly Dictionary<string,CommandDescriptor> Map =
            new Dictionary<string,CommandDescriptor>(StringComparer.OrdinalIgnoreCase);

        static CommandRouter()
        {
            A("rtl","العربية","RTL","اتجاه اليمين لليسار.",Commands.Rtl);
            A("ltr","العربية","LTR","اتجاه اليسار لليمين.",Commands.Ltr);
            A("remove-diacritics","العربية","حذف التشكيل","حذف التشكيل العربي.",Commands.RemoveDiacritics);
            A("remove-tatweel","العربية","حذف التطويل","حذف ـ.",Commands.RemoveTatweel);
            A("normalize-arabic","العربية","توحيد الحروف","توحيد الألف والياء والهمزات.",Commands.NormalizeArabic);
            A("digits-eastern","العربية","أرقام عربية","تحويل 0-9 إلى ٠-٩.",Commands.ToEastern);
            A("digits-western","العربية","أرقام غربية","تحويل الأرقام العربية إلى 0-9.",Commands.ToWestern);
            A("arabic-clean-all","العربية","تنظيف عربي شامل","تنظيف وضبط RTL.",Commands.ArabicCleanAll);

            A("collapse-spaces","النص","تنظيف المسافات","حذف المسافات المتكررة.",Commands.CollapseSpaces);
            A("remove-empty-paragraphs","النص","حذف الفقرات الفارغة","اختصار الفراغات المتكررة.",Commands.RemoveEmptyParagraphs);
            A("tabs-to-spaces","النص","Tabs إلى مسافات","استبدال Tabs.",Commands.TabsToSpaces);
            A("linebreaks-to-paragraphs","النص","الأسطر إلى فقرات","تحويل فواصل الأسطر.",Commands.LineBreaksToParagraphs);
            A("remove-duplicate-lines","النص","حذف الأسطر المكررة","الاحتفاظ بأول سطر.",Commands.RemoveDuplicateLines);
            A("sort-lines-asc","النص","فرز تصاعدي","فرز الأسطر.",()=>Commands.SortLines(false));
            A("sort-lines-desc","النص","فرز تنازلي","فرز الأسطر.",()=>Commands.SortLines(true));
            A("uppercase","النص","UPPERCASE","تحويل الأحرف.",Commands.Uppercase);
            A("lowercase","النص","lowercase","تحويل الأحرف.",Commands.Lowercase);
            A("titlecase","النص","Title Case","تحويل بدايات الكلمات.",Commands.TitleCase);
            A("remove-hyperlinks","النص","إزالة الروابط","حذف الروابط مع إبقاء النص.",Commands.RemoveHyperlinks);

            A("font-arial-14","التنسيق","Arial 14","تطبيق Arial 14.",Commands.Arial14);
            A("clear-formatting","التنسيق","إزالة التنسيق","مسح التنسيق المباشر.",Commands.ClearFormatting);
            A("align-right","التنسيق","يمين","محاذاة يمين.",()=>Commands.Align(2));
            A("align-left","التنسيق","يسار","محاذاة يسار.",()=>Commands.Align(0));
            A("align-center","التنسيق","توسيط","توسيط الفقرات.",()=>Commands.Align(1));
            A("align-justify","التنسيق","ضبط","ضبط الفقرات.",()=>Commands.Align(3));
            A("line-spacing-115","التنسيق","تباعد 1.15","تباعد أسطر.",()=>Commands.LineSpacing(13.8f));
            A("line-spacing-15","التنسيق","تباعد 1.5","تباعد أسطر.",()=>Commands.LineSpacing(18f));
            A("heading-1","الأنماط","عنوان 1","تطبيق Heading 1.",()=>Commands.Style("Heading 1"));
            A("heading-2","الأنماط","عنوان 2","تطبيق Heading 2.",()=>Commands.Style("Heading 2"));
            A("heading-3","الأنماط","عنوان 3","تطبيق Heading 3.",()=>Commands.Style("Heading 3"));
            A("style-normal","الأنماط","عادي","تطبيق Normal.",()=>Commands.Style("Normal"));
            A("format-arabic-report","التنسيق","تقرير عربي","تنسيق تقرير عربي احترافي.",Commands.ArabicReport);

            A("table-rtl","الجداول","جدول RTL","اتجاه الجدول RTL.",Commands.TableRtl);
            A("table-autofit-window","الجداول","ملاءمة الصفحة","ملاءمة عرض الصفحة.",()=>Commands.TableAutoFit(2));
            A("table-autofit-content","الجداول","ملاءمة المحتوى","ملاءمة المحتوى.",()=>Commands.TableAutoFit(1));
            A("table-distribute-columns","الجداول","توزيع الأعمدة","توزيع بالتساوي.",Commands.TableDistributeColumns);
            A("table-distribute-rows","الجداول","توزيع الصفوف","توزيع بالتساوي.",Commands.TableDistributeRows);
            A("table-repeat-header","الجداول","تكرار صف العنوان","تكرار الصف الأول.",Commands.TableRepeatHeader);
            A("table-remove-empty-rows","الجداول","حذف الصفوف الفارغة","حذف الصفوف الخالية.",Commands.TableRemoveEmptyRows);
            A("table-center-cells","الجداول","توسيط الخلايا","توسيط أفقي وعمودي.",Commands.TableCenterCells);
            A("table-to-text","الجداول","تحويل إلى نص","تحويل الجدول إلى نص.",Commands.TableToText);

            A("update-fields","المستند","تحديث الحقول","تحديث جميع الحقول.",Commands.UpdateFields);
            A("insert-toc","المستند","إضافة فهرس","إدراج جدول محتويات.",Commands.InsertToc);
            A("page-numbers-add","المستند","إضافة أرقام الصفحات","إضافة إلى التذييل.",Commands.AddPageNumbers);
            A("page-numbers-remove","المستند","حذف أرقام الصفحات","حذف حقول PAGE.",Commands.RemovePageNumbers);
            A("page-a4","المستند","A4","ضبط حجم الورق.",Commands.SetA4);
            A("page-portrait","المستند","عمودي","اتجاه عمودي.",()=>Commands.Orientation(0));
            A("page-landscape","المستند","أفقي","اتجاه أفقي.",()=>Commands.Orientation(1));
            A("margins-normal","المستند","هوامش عادية","2.54 سم.",()=>Commands.Margins(72f));
            A("margins-narrow","المستند","هوامش ضيقة","1.27 سم.",()=>Commands.Margins(36f));

            A("remove-comments","المراجعة","حذف التعليقات","حذف التعليقات.",Commands.RemoveComments);
            A("accept-revisions","المراجعة","قبول التعديلات","قبول التغييرات.",Commands.AcceptRevisions);
            A("reject-revisions","المراجعة","رفض التعديلات","رفض التغييرات.",Commands.RejectRevisions);
            A("remove-metadata","الخصوصية","إزالة البيانات الشخصية","إزالة معلومات المستند.",Commands.RemoveMetadata);

            A("save-backup","الملفات","نسخة احتياطية","حفظ نسخة مؤرخة.",Commands.SaveBackup);
            A("export-pdf","الملفات","تصدير PDF","تصدير PDF.",()=>Commands.ExportPdf(false));
            A("export-pdfa","الملفات","تصدير PDF/A","تصدير PDF/A.",()=>Commands.ExportPdf(true));

            A("template-agenda","القوالب","جدول أعمال","إدراج نموذج.",()=>Commands.InsertTemplate("agenda"));
            A("template-minutes","القوالب","محضر اجتماع","إدراج نموذج.",()=>Commands.InsertTemplate("minutes"));
            A("template-official-letter","القوالب","خطاب رسمي","إدراج نموذج.",()=>Commands.InsertTemplate("letter"));
            A("template-medical-report","القوالب","تقرير طبي","إدراج نموذج.",()=>Commands.InsertTemplate("medical"));
            A("template-project-report","القوالب","تقرير مشروع","إدراج نموذج.",()=>Commands.InsertTemplate("project"));
            A("template-handover","القوالب","تسليم واستلام","إدراج نموذج.",()=>Commands.InsertTemplate("handover"));
            A("workflow-final-delivery","الأتمتة","تجهيز نسخة نهائية","قبول التعديلات وحذف التعليقات وتحديث الحقول.",Commands.FinalDelivery);

            A("command-center","النظام","مركز الأدوات","نافذة كبيرة للبحث والتشغيل.",CommandCenterForm.ShowCenter);
            A("health-check","النظام","فحص الإضافة","عرض حالة التشغيل.",Commands.HealthCheck);
            A("about","النظام","حول","معلومات الإصدار.",Commands.About);
        }

        internal static IEnumerable<CommandDescriptor> All => Map.Values;
        private static void A(string id,string cat,string title,string desc,Action action) =>
            Map.Add(id,new CommandDescriptor(id,cat,title,desc,action));

        internal static void Execute(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) return;
            if (!Map.TryGetValue(id,out var c))
            { MessageBox.Show("الأداة غير مسجلة: "+id,"WordPro Suite"); return; }
            try { Logger.Info("Start "+id); c.Execute(); Logger.Info("Done "+id); }
            catch(Exception ex){ Logger.Error("Failed "+id,ex); MessageBox.Show(ex.Message,"WordPro Suite",MessageBoxButtons.OK,MessageBoxIcon.Error); }
        }
    }
}
