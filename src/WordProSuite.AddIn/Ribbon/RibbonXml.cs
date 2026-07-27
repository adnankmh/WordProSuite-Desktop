namespace WordProSuite.Desktop.Ribbon
{
    internal static class RibbonXml
    {
        internal const string Value = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<customUI xmlns=""http://schemas.microsoft.com/office/2009/07/customui"" onLoad=""RibbonOnLoad"">
  <ribbon>
    <tabs>
      <tab id=""tabWordProDesktop"" label=""WordPro Suite Desktop Pro"">
        <group id=""grpHome"" label=""الرئيسية"">
          <button id=""btnCommandCenter"" tag=""command-center"" label=""مركز الأدوات"" size=""large"" imageMso=""AddInManager"" onAction=""RibbonOnAction"" screentip=""جميع الأدوات مع البحث والمفضلة""/>
          <button id=""btnFinalShare"" tag=""workflow-final-share"" label=""تجهيز للمشاركة"" size=""large"" imageMso=""FileCheckIn"" onAction=""RibbonOnAction""/>
          <menu id=""menuSystem"" label=""النظام والترخيص"" imageMso=""HappyFace"">
            <button id=""btnActivate"" tag=""activate"" label=""تفعيل البرنامج"" onAction=""RibbonOnAction""/>
            <button id=""btnLicenseInfo"" tag=""license-info"" label=""حالة الترخيص"" onAction=""RibbonOnAction""/>
            <button id=""btnMachineId"" tag=""machine-id"" label=""نسخ معرّف الجهاز"" onAction=""RibbonOnAction""/>
            <button id=""btnHealth"" tag=""health-check"" label=""فحص الإضافة"" onAction=""RibbonOnAction""/>
            <button id=""btnOpenLog"" tag=""open-log"" label=""فتح السجل"" onAction=""RibbonOnAction""/>
            <button id=""btnAbout"" tag=""about"" label=""حول"" onAction=""RibbonOnAction""/>
          </menu>
          <labelControl id=""lblLicenseStatus"" getLabel=""RibbonGetStatus""/>
        </group>

        <group id=""grpArabic"" label=""العربية والاتجاه"">
          <button id=""btnRtl"" tag=""rtl"" label=""RTL"" size=""large"" imageMso=""ParagraphRightToLeft"" onAction=""RibbonOnAction""/>
          <button id=""btnLtr"" tag=""ltr"" label=""LTR"" size=""large"" imageMso=""ParagraphLeftToRight"" onAction=""RibbonOnAction""/>
          <button id=""btnAutoDir"" tag=""auto-direction"" label=""اتجاه تلقائي"" imageMso=""TextDirectionOptions"" onAction=""RibbonOnAction""/>
          <button id=""btnArabicClean"" tag=""arabic-clean-all"" label=""تنظيف شامل"" size=""large"" imageMso=""ReviewDeleteAllMarkupInDocument"" onAction=""RibbonOnAction""/>
          <menu id=""menuArabicClean"" label=""تنظيف وتحويل"" imageMso=""TextDirectionOptions"">
            <button id=""btnDiac"" tag=""remove-diacritics"" label=""حذف التشكيل"" onAction=""RibbonOnAction""/>
            <button id=""btnTatweel"" tag=""remove-tatweel"" label=""حذف التطويل"" onAction=""RibbonOnAction""/>
            <button id=""btnNormalizeAr"" tag=""normalize-arabic"" label=""توحيد الحروف"" onAction=""RibbonOnAction""/>
            <button id=""btnPunctAr"" tag=""normalize-arabic-punctuation"" label=""ترقيم عربي ذكي"" onAction=""RibbonOnAction""/>
            <button id=""btnQuotesAr"" tag=""arabic-quotes"" label=""اقتباس عربي « »"" onAction=""RibbonOnAction""/>
            <button id=""btnZeroWidth"" tag=""remove-zero-width"" label=""حذف المحارف المخفية"" onAction=""RibbonOnAction""/>
            <button id=""btnEastern"" tag=""digits-eastern"" label=""أرقام عربية"" onAction=""RibbonOnAction""/>
            <button id=""btnWestern"" tag=""digits-western"" label=""أرقام غربية"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuArabicSmart"" label=""أدوات لغوية"" imageMso=""TextBoxInsert"">
            <button id=""btnKbArEn"" tag=""keyboard-ar-to-en"" label=""تصحيح عربي ← إنجليزي"" onAction=""RibbonOnAction""/>
            <button id=""btnKbEnAr"" tag=""keyboard-en-to-ar"" label=""تصحيح إنجليزي ← عربي"" onAction=""RibbonOnAction""/>
            <button id=""btnArabicDate"" tag=""insert-arabic-date"" label=""تاريخ ميلادي عربي"" onAction=""RibbonOnAction""/>
            <button id=""btnHijriDate"" tag=""insert-hijri-date"" label=""تاريخ هجري"" onAction=""RibbonOnAction""/>
            <button id=""btnNumberWords"" tag=""number-arabic-words"" label=""رقم إلى كلمات عربية"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpText"" label=""النص والتحرير"">
          <button id=""btnSpaces"" tag=""collapse-spaces"" label=""تنظيف المسافات"" size=""large"" imageMso=""ClearFormatting"" onAction=""RibbonOnAction""/>
          <button id=""btnDupParagraphs"" tag=""remove-duplicate-paragraphs"" label=""حذف المكرر"" size=""large"" imageMso=""ReviewDeleteAllMarkupInDocument"" onAction=""RibbonOnAction""/>
          <menu id=""menuTextClean"" label=""تنظيف النص"" imageMso=""TextBoxInsert"">
            <button id=""btnTrimLines"" tag=""trim-lines"" label=""قص حواف الأسطر"" onAction=""RibbonOnAction""/>
            <button id=""btnEmptyParagraphs"" tag=""remove-empty-paragraphs"" label=""حذف الفقرات الفارغة"" onAction=""RibbonOnAction""/>
            <button id=""btnJoinParagraphs"" tag=""join-paragraphs"" label=""دمج الفقرات"" onAction=""RibbonOnAction""/>
            <button id=""btnTabsSpaces"" tag=""tabs-to-spaces"" label=""Tabs إلى مسافات"" onAction=""RibbonOnAction""/>
            <button id=""btnLineParagraphs"" tag=""linebreaks-to-paragraphs"" label=""الأسطر إلى فقرات"" onAction=""RibbonOnAction""/>
            <button id=""btnDupWords"" tag=""remove-duplicate-words"" label=""حذف الكلمات المتتابعة المكررة"" onAction=""RibbonOnAction""/>
            <button id=""btnNonPrinting"" tag=""remove-nonprinting"" label=""حذف المحارف غير المطبوعة"" onAction=""RibbonOnAction""/>
            <button id=""btnRemoveLinks"" tag=""remove-hyperlinks"" label=""إزالة الروابط"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuTextTransform"" label=""تحويل وفرز"" imageMso=""TextBoxInsert"">
            <button id=""btnSortAsc"" tag=""sort-lines-asc"" label=""فرز تصاعدي"" onAction=""RibbonOnAction""/>
            <button id=""btnSortDesc"" tag=""sort-lines-desc"" label=""فرز تنازلي"" onAction=""RibbonOnAction""/>
            <button id=""btnPrefix"" tag=""prefix-lines"" label=""إضافة بادئة"" onAction=""RibbonOnAction""/>
            <button id=""btnSuffix"" tag=""suffix-lines"" label=""إضافة لاحقة"" onAction=""RibbonOnAction""/>
            <button id=""btnSplit"" tag=""split-by-delimiter"" label=""تقسيم حسب فاصل"" onAction=""RibbonOnAction""/>
            <button id=""btnBullets"" tag=""paragraphs-bullets"" label=""تحويل إلى نقاط"" onAction=""RibbonOnAction""/>
            <button id=""btnNumbers"" tag=""paragraphs-numbers"" label=""تحويل إلى ترقيم"" onAction=""RibbonOnAction""/>
            <button id=""btnClearList"" tag=""clear-list"" label=""إزالة القائمة"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuExtract"" label=""استخراج وإحصاء"" imageMso=""FindDialog"">
            <button id=""btnExtractEmail"" tag=""extract-emails"" label=""استخراج البريد"" onAction=""RibbonOnAction""/>
            <button id=""btnExtractUrls"" tag=""extract-urls"" label=""استخراج الروابط"" onAction=""RibbonOnAction""/>
            <button id=""btnExtractPhones"" tag=""extract-phones"" label=""استخراج الهواتف"" onAction=""RibbonOnAction""/>
            <button id=""btnTextStats"" tag=""text-statistics"" label=""إحصاءات النص"" onAction=""RibbonOnAction""/>
            <button id=""btnPastePlain"" tag=""paste-plain"" label=""لصق كنص فقط"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpFormat"" label=""التنسيق والأنماط"">
          <button id=""btnArial14"" tag=""font-arial-14"" label=""Arial 14"" size=""large"" imageMso=""FontDialog"" onAction=""RibbonOnAction""/>
          <button id=""btnArabicReport"" tag=""format-arabic-report"" label=""تقرير عربي"" size=""large"" imageMso=""StylesPane"" onAction=""RibbonOnAction""/>
          <menu id=""menuFontPresets"" label=""الخطوط"" imageMso=""FontDialog"">
            <button id=""btnTahoma12"" tag=""font-tahoma-12"" label=""Tahoma 12"" onAction=""RibbonOnAction""/>
            <button id=""btnTimes12"" tag=""font-times-12"" label=""Times New Roman 12"" onAction=""RibbonOnAction""/>
            <button id=""btnCalibri11"" tag=""font-calibri-11"" label=""Calibri 11"" onAction=""RibbonOnAction""/>
            <button id=""btnClearFormatting"" tag=""clear-formatting"" label=""إزالة التنسيق المباشر"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuParagraph"" label=""الفقرات"" imageMso=""ParagraphDialog"">
            <button id=""btnAlignRight"" tag=""align-right"" label=""يمين"" onAction=""RibbonOnAction""/>
            <button id=""btnAlignLeft"" tag=""align-left"" label=""يسار"" onAction=""RibbonOnAction""/>
            <button id=""btnAlignCenter"" tag=""align-center"" label=""توسيط"" onAction=""RibbonOnAction""/>
            <button id=""btnAlignJustify"" tag=""align-justify"" label=""ضبط كامل"" onAction=""RibbonOnAction""/>
            <button id=""btnLine1"" tag=""line-spacing-1"" label=""تباعد مفرد"" onAction=""RibbonOnAction""/>
            <button id=""btnLine115"" tag=""line-spacing-115"" label=""تباعد 1.15"" onAction=""RibbonOnAction""/>
            <button id=""btnLine15"" tag=""line-spacing-15"" label=""تباعد 1.5"" onAction=""RibbonOnAction""/>
            <button id=""btnLine2"" tag=""line-spacing-2"" label=""تباعد مزدوج"" onAction=""RibbonOnAction""/>
            <button id=""btnIndentFirst"" tag=""indent-first-line"" label=""بادئة السطر الأول"" onAction=""RibbonOnAction""/>
            <button id=""btnIndentRemove"" tag=""indent-remove"" label=""إزالة المسافات البادئة"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuStyles"" label=""الأنماط واللمسات"" imageMso=""StylesPane"">
            <button id=""btnHeading1"" tag=""heading-1"" label=""عنوان 1"" onAction=""RibbonOnAction""/>
            <button id=""btnHeading2"" tag=""heading-2"" label=""عنوان 2"" onAction=""RibbonOnAction""/>
            <button id=""btnHeading3"" tag=""heading-3"" label=""عنوان 3"" onAction=""RibbonOnAction""/>
            <button id=""btnNormalStyle"" tag=""style-normal"" label=""عادي"" onAction=""RibbonOnAction""/>
            <button id=""btnOfficialFormat"" tag=""format-official-letter"" label=""تنسيق خطاب رسمي"" onAction=""RibbonOnAction""/>
            <button id=""btnAcademicFormat"" tag=""format-academic"" label=""تنسيق أكاديمي"" onAction=""RibbonOnAction""/>
            <button id=""btnCopyFormat"" tag=""format-copy"" label=""حفظ لقطة تنسيق"" onAction=""RibbonOnAction""/>
            <button id=""btnApplyFormat"" tag=""format-apply"" label=""تطبيق لقطة التنسيق"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpTables"" label=""الجداول"">
          <button id=""btnTableProfessional"" tag=""table-professional"" label=""جدول احترافي"" size=""large"" imageMso=""TableInsertDialogWord"" onAction=""RibbonOnAction""/>
          <button id=""btnTableFit"" tag=""table-autofit-window"" label=""ملاءمة الصفحة"" size=""large"" imageMso=""TableAutoFitWindow"" onAction=""RibbonOnAction""/>
          <menu id=""menuTableFormat"" label=""تنسيق الجدول"" imageMso=""TablePropertiesDialog"">
            <button id=""btnTableRTL"" tag=""table-rtl"" label=""اتجاه RTL"" onAction=""RibbonOnAction""/>
            <button id=""btnTableLTR"" tag=""table-ltr"" label=""اتجاه LTR"" onAction=""RibbonOnAction""/>
            <button id=""btnTableHeader"" tag=""table-header-style"" label=""تنسيق صف العنوان"" onAction=""RibbonOnAction""/>
            <button id=""btnTableBands"" tag=""table-banded-rows"" label=""صفوف متبادلة"" onAction=""RibbonOnAction""/>
            <button id=""btnTableFitContent"" tag=""table-autofit-content"" label=""ملاءمة المحتوى"" onAction=""RibbonOnAction""/>
            <button id=""btnDistCols"" tag=""table-distribute-columns"" label=""توزيع الأعمدة"" onAction=""RibbonOnAction""/>
            <button id=""btnDistRows"" tag=""table-distribute-rows"" label=""توزيع الصفوف"" onAction=""RibbonOnAction""/>
            <button id=""btnRepeatHeader"" tag=""table-repeat-header"" label=""تكرار صف العنوان"" onAction=""RibbonOnAction""/>
            <button id=""btnBordersAll"" tag=""table-borders-all"" label=""كل الحدود"" onAction=""RibbonOnAction""/>
            <button id=""btnBordersNone"" tag=""table-borders-none"" label=""بدون حدود"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuTableData"" label=""البيانات والحساب"" imageMso=""TablePropertiesDialog"">
            <button id=""btnTrimCells"" tag=""table-trim-cells"" label=""تنظيف الخلايا"" onAction=""RibbonOnAction""/>
            <button id=""btnRemoveEmptyRows"" tag=""table-remove-empty-rows"" label=""حذف الصفوف الفارغة"" onAction=""RibbonOnAction""/>
            <button id=""btnRemoveDupRows"" tag=""table-remove-duplicate-rows"" label=""حذف الصفوف المكررة"" onAction=""RibbonOnAction""/>
            <button id=""btnNumberFirst"" tag=""table-number-first-column"" label=""ترقيم العمود الأول"" onAction=""RibbonOnAction""/>
            <button id=""btnSumCol"" tag=""table-sum-column"" label=""مجموع العمود"" onAction=""RibbonOnAction""/>
            <button id=""btnAvgCol"" tag=""table-average-column"" label=""متوسط العمود"" onAction=""RibbonOnAction""/>
            <button id=""btnSortTableAsc"" tag=""table-sort-asc"" label=""فرز تصاعدي"" onAction=""RibbonOnAction""/>
            <button id=""btnSortTableDesc"" tag=""table-sort-desc"" label=""فرز تنازلي"" onAction=""RibbonOnAction""/>
            <button id=""btnTranspose"" tag=""table-transpose"" label=""تبديل الصفوف والأعمدة"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuTableStructure"" label=""الصفوف والأعمدة"" imageMso=""TableInsertDialogWord"">
            <button id=""btnRowAbove"" tag=""table-row-above"" label=""صف أعلى"" onAction=""RibbonOnAction""/>
            <button id=""btnRowBelow"" tag=""table-row-below"" label=""صف أسفل"" onAction=""RibbonOnAction""/>
            <button id=""btnColLeft"" tag=""table-col-left"" label=""عمود يسار"" onAction=""RibbonOnAction""/>
            <button id=""btnColRight"" tag=""table-col-right"" label=""عمود يمين"" onAction=""RibbonOnAction""/>
            <button id=""btnDeleteRow"" tag=""table-delete-row"" label=""حذف الصف"" onAction=""RibbonOnAction""/>
            <button id=""btnDeleteCol"" tag=""table-delete-column"" label=""حذف العمود"" onAction=""RibbonOnAction""/>
            <button id=""btnMergeCells"" tag=""table-merge-cells"" label=""دمج الخلايا"" onAction=""RibbonOnAction""/>
            <button id=""btnSplitCell"" tag=""table-split-cell"" label=""تقسيم الخلية"" onAction=""RibbonOnAction""/>
            <button id=""btnTextTable"" tag=""text-to-table"" label=""نص إلى جدول"" onAction=""RibbonOnAction""/>
            <button id=""btnTableText"" tag=""table-to-text"" label=""جدول إلى نص"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpDocument"" label=""المستند والصفحات"">
          <button id=""btnFields"" tag=""update-fields"" label=""تحديث الحقول"" size=""large"" imageMso=""RefreshAll"" onAction=""RibbonOnAction""/>
          <button id=""btnToc"" tag=""insert-toc"" label=""إضافة فهرس"" size=""large"" imageMso=""TableOfContentsAddTextGallery"" onAction=""RibbonOnAction""/>
          <menu id=""menuPageSetup"" label=""إعداد الصفحة"" imageMso=""PageSetupDialog"">
            <button id=""btnA4"" tag=""page-a4"" label=""A4"" onAction=""RibbonOnAction""/>
            <button id=""btnA3"" tag=""page-a3"" label=""A3"" onAction=""RibbonOnAction""/>
            <button id=""btnLetter"" tag=""page-letter"" label=""Letter"" onAction=""RibbonOnAction""/>
            <button id=""btnPortrait"" tag=""page-portrait"" label=""عمودي"" onAction=""RibbonOnAction""/>
            <button id=""btnLandscape"" tag=""page-landscape"" label=""أفقي"" onAction=""RibbonOnAction""/>
            <button id=""btnMarginsNormal"" tag=""margins-normal"" label=""هوامش عادية"" onAction=""RibbonOnAction""/>
            <button id=""btnMarginsNarrow"" tag=""margins-narrow"" label=""هوامش ضيقة"" onAction=""RibbonOnAction""/>
            <button id=""btnCols1"" tag=""columns-1"" label=""عمود واحد"" onAction=""RibbonOnAction""/>
            <button id=""btnCols2"" tag=""columns-2"" label=""عمودان"" onAction=""RibbonOnAction""/>
            <button id=""btnCols3"" tag=""columns-3"" label=""ثلاثة أعمدة"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuBreaks"" label=""الفواصل والترقيم"" imageMso=""PageSetupDialog"">
            <button id=""btnPageBreak"" tag=""insert-page-break"" label=""فاصل صفحة"" onAction=""RibbonOnAction""/>
            <button id=""btnSectionNext"" tag=""insert-section-next"" label=""مقطع صفحة جديدة"" onAction=""RibbonOnAction""/>
            <button id=""btnSectionContinuous"" tag=""insert-section-continuous"" label=""مقطع مستمر"" onAction=""RibbonOnAction""/>
            <button id=""btnRemovePageBreaks"" tag=""remove-page-breaks"" label=""حذف فواصل الصفحات"" onAction=""RibbonOnAction""/>
            <button id=""btnRemoveSectionBreaks"" tag=""remove-section-breaks"" label=""حذف فواصل المقاطع"" onAction=""RibbonOnAction""/>
            <button id=""btnPageNumbers"" tag=""page-numbers-add"" label=""إضافة أرقام الصفحات"" onAction=""RibbonOnAction""/>
            <button id=""btnRemovePageNumbers"" tag=""page-numbers-remove"" label=""حذف أرقام الصفحات"" onAction=""RibbonOnAction""/>
            <button id=""btnLineNumbersOn"" tag=""line-numbers-on"" label=""تشغيل أرقام الأسطر"" onAction=""RibbonOnAction""/>
            <button id=""btnLineNumbersOff"" tag=""line-numbers-off"" label=""إيقاف أرقام الأسطر"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuDocumentTools"" label=""أدوات المستند"" imageMso=""PageSetupDialog"">
            <button id=""btnHeaderText"" tag=""header-text"" label=""نص رأس الصفحة"" onAction=""RibbonOnAction""/>
            <button id=""btnFooterText"" tag=""footer-text"" label=""نص التذييل"" onAction=""RibbonOnAction""/>
            <button id=""btnClearHeaderFooter"" tag=""clear-headers-footers"" label=""مسح الرؤوس والتذييلات"" onAction=""RibbonOnAction""/>
            <button id=""btnInsertDateTime"" tag=""insert-date-time"" label=""إدراج تاريخ ووقت"" onAction=""RibbonOnAction""/>
            <button id=""btnInsertFilename"" tag=""insert-file-name"" label=""اسم ومسار الملف"" onAction=""RibbonOnAction""/>
            <button id=""btnDocStats"" tag=""document-statistics"" label=""إحصاءات المستند"" onAction=""RibbonOnAction""/>
            <button id=""btnReplaceFont"" tag=""replace-font"" label=""استبدال خط"" onAction=""RibbonOnAction""/>
            <button id=""btnCoverPage"" tag=""insert-cover"" label=""إنشاء صفحة غلاف"" onAction=""RibbonOnAction""/>
            <button id=""btnSignature"" tag=""insert-signature"" label=""كتلة توقيع"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpReview"" label=""المراجعة والأمان"">
          <menu id=""menuReview"" label=""التعديلات والتعليقات"" size=""large"" imageMso=""ReviewDeleteAllMarkupInDocument"">
            <button id=""btnTrackOn"" tag=""track-on"" label=""تشغيل تعقب التغييرات"" onAction=""RibbonOnAction""/>
            <button id=""btnTrackOff"" tag=""track-off"" label=""إيقاف تعقب التغييرات"" onAction=""RibbonOnAction""/>
            <button id=""btnAccept"" tag=""accept-revisions"" label=""قبول كل التعديلات"" onAction=""RibbonOnAction""/>
            <button id=""btnReject"" tag=""reject-revisions"" label=""رفض كل التعديلات"" onAction=""RibbonOnAction""/>
            <button id=""btnComments"" tag=""remove-comments"" label=""حذف التعليقات"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuSecurity"" label=""الحماية والخصوصية"" imageMso=""ProtectDocument"">
            <button id=""btnProtectRead"" tag=""protect-readonly"" label=""حماية للقراءة فقط"" onAction=""RibbonOnAction""/>
            <button id=""btnProtectForms"" tag=""protect-forms"" label=""حماية تعبئة النماذج"" onAction=""RibbonOnAction""/>
            <button id=""btnUnprotect"" tag=""unprotect"" label=""إلغاء الحماية"" onAction=""RibbonOnAction""/>
            <button id=""btnMetadata"" tag=""remove-metadata"" label=""إزالة البيانات الشخصية"" onAction=""RibbonOnAction""/>
            <button id=""btnCleanProps"" tag=""clean-properties"" label=""تنظيف الخصائص"" onAction=""RibbonOnAction""/>
            <button id=""btnExternalLinks"" tag=""remove-external-links"" label=""فصل الروابط الخارجية"" onAction=""RibbonOnAction""/>
            <button id=""btnHiddenText"" tag=""remove-hidden-text"" label=""حذف النص المخفي"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpFiles"" label=""الملفات وPDF"">
          <button id=""btnBackup"" tag=""save-backup"" label=""نسخة احتياطية"" size=""large"" imageMso=""FileSaveAs"" onAction=""RibbonOnAction""/>
          <button id=""btnPdf"" tag=""export-pdf"" label=""تصدير PDF"" size=""large"" imageMso=""FileSaveAsPdfOrXps"" onAction=""RibbonOnAction""/>
          <menu id=""menuFiles"" label=""الحفظ والإصدارات"" imageMso=""FileSaveAs"">
            <button id=""btnSaveVersion"" tag=""save-version"" label=""حفظ إصدار جديد"" onAction=""RibbonOnAction""/>
            <button id=""btnSaveDocx"" tag=""save-docx-copy"" label=""نسخة DOCX"" onAction=""RibbonOnAction""/>
            <button id=""btnSaveAll"" tag=""save-all"" label=""حفظ كل المستندات"" onAction=""RibbonOnAction""/>
            <button id=""btnCopyPath"" tag=""copy-document-path"" label=""نسخ مسار المستند"" onAction=""RibbonOnAction""/>
            <button id=""btnOpenFolder"" tag=""open-document-folder"" label=""فتح موقع المستند"" onAction=""RibbonOnAction""/>
            <button id=""btnArchive"" tag=""archive-package"" label=""إنشاء حزمة أرشيف"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuPdf"" label=""PDF والدمج"" imageMso=""FileSaveAsPdfOrXps"">
            <button id=""btnPdfA"" tag=""export-pdfa"" label=""تصدير PDF/A"" onAction=""RibbonOnAction""/>
            <button id=""btnSelectionPdf"" tag=""export-selection-pdf"" label=""التحديد إلى PDF"" onAction=""RibbonOnAction""/>
            <button id=""btnSectionsPdf"" tag=""export-sections-pdf"" label=""كل مقطع PDF"" onAction=""RibbonOnAction""/>
            <button id=""btnBatchPdf"" tag=""batch-pdf"" label=""تحويل مجلد إلى PDF"" onAction=""RibbonOnAction""/>
            <button id=""btnMergeDocs"" tag=""merge-documents"" label=""دمج مستندات"" onAction=""RibbonOnAction""/>
          </menu>
        </group>


        <group id=""grpEnterprise"" label=""الحزمة الاحترافية"">
          <button id=""btnDocumentDashboard"" tag=""document-dashboard"" label=""لوحة المستند"" size=""large"" imageMso=""ReviewDisplayForReview"" onAction=""RibbonOnAction""/>
          <menu id=""menuProText"" label=""نص احترافي"" size=""large"" imageMso=""FindDialog"">
            <button id=""btnUnicodeSpaces"" tag=""normalize-unicode-spaces"" label=""توحيد المسافات Unicode"" onAction=""RibbonOnAction""/>
            <button id=""btnLeadingSpaces"" tag=""remove-leading-spaces"" label=""حذف مسافات البداية"" onAction=""RibbonOnAction""/>
            <button id=""btnTrailingSpaces"" tag=""remove-trailing-spaces"" label=""حذف مسافات النهاية"" onAction=""RibbonOnAction""/>
            <button id=""btnSentencesParagraphs"" tag=""sentences-to-paragraphs"" label=""الجمل إلى فقرات"" onAction=""RibbonOnAction""/>
            <button id=""btnStripHtml"" tag=""strip-html-tags"" label=""حذف HTML"" onAction=""RibbonOnAction""/>
            <button id=""btnWordFrequency"" tag=""word-frequency-top"" label=""أكثر الكلمات تكرارًا"" onAction=""RibbonOnAction""/>
            <button id=""btnFastReplace"" tag=""find-replace-prompt"" label=""بحث واستبدال سريع"" onAction=""RibbonOnAction""/>
            <button id=""btnHighlightTerm"" tag=""highlight-term"" label=""تمييز كلمة"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuEnterpriseFrames"" label=""أطر مؤسسية"" size=""large"" imageMso=""TableInsertDialogWord"">
            <button id=""btnDecisionLog"" tag=""insert-decision-log"" label=""سجل القرارات"" onAction=""RibbonOnAction""/>
            <button id=""btnRaci"" tag=""insert-raci-matrix"" label=""مصفوفة RACI"" onAction=""RibbonOnAction""/>
            <button id=""btnSwot"" tag=""insert-swot-matrix"" label=""تحليل SWOT"" onAction=""RibbonOnAction""/>
            <button id=""btnKpi"" tag=""insert-kpi-table"" label=""جدول KPI"" onAction=""RibbonOnAction""/>
            <button id=""btnBudget"" tag=""insert-budget-table"" label=""جدول ميزانية"" onAction=""RibbonOnAction""/>
            <button id=""btnTimeline"" tag=""insert-timeline-table"" label=""جدول زمني"" onAction=""RibbonOnAction""/>
            <button id=""btnYesNo"" tag=""insert-yes-no-table"" label=""قائمة تحقق"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuProExport"" label=""تحليل وتصدير"" imageMso=""FileSaveAs"">
            <button id=""btnListHeadings"" tag=""list-document-headings"" label=""قائمة العناوين"" onAction=""RibbonOnAction""/>
            <button id=""btnListBookmarks"" tag=""list-bookmarks"" label=""قائمة الإشارات"" onAction=""RibbonOnAction""/>
            <button id=""btnExportTxt"" tag=""export-plain-text"" label=""تصدير TXT"" onAction=""RibbonOnAction""/>
            <button id=""btnExportSelectionTxt"" tag=""export-selection-text"" label=""التحديد إلى TXT"" onAction=""RibbonOnAction""/>
            <button id=""btnCopyMarkdown"" tag=""copy-as-markdown"" label=""نسخ Markdown"" onAction=""RibbonOnAction""/>
            <button id=""btnPageXofY"" tag=""insert-page-x-of-y"" label=""صفحة X من Y"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpTemplates"" label=""القوالب والأدوات"">
          <menu id=""menuTemplates"" label=""قوالب جاهزة"" size=""large"" imageMso=""NewDocumentOrTemplate"">
            <button id=""btnAgenda"" tag=""template-agenda"" label=""جدول أعمال"" onAction=""RibbonOnAction""/>
            <button id=""btnMinutes"" tag=""template-minutes"" label=""محضر اجتماع"" onAction=""RibbonOnAction""/>
            <button id=""btnOfficialLetter"" tag=""template-official-letter"" label=""خطاب رسمي"" onAction=""RibbonOnAction""/>
            <button id=""btnMedical"" tag=""template-medical-report"" label=""تقرير طبي"" onAction=""RibbonOnAction""/>
            <button id=""btnProject"" tag=""template-project-report"" label=""تقرير مشروع"" onAction=""RibbonOnAction""/>
            <button id=""btnHandover"" tag=""template-handover"" label=""تسليم واستلام"" onAction=""RibbonOnAction""/>
            <button id=""btnMemo"" tag=""template-memo"" label=""مذكرة داخلية"" onAction=""RibbonOnAction""/>
            <button id=""btnQuotation"" tag=""template-quotation"" label=""عرض سعر"" onAction=""RibbonOnAction""/>
            <button id=""btnInvoice"" tag=""template-invoice"" label=""فاتورة"" onAction=""RibbonOnAction""/>
            <button id=""btnCertificate"" tag=""template-certificate"" label=""شهادة تقدير"" onAction=""RibbonOnAction""/>
            <button id=""btnAttendance"" tag=""template-attendance"" label=""كشف حضور"" onAction=""RibbonOnAction""/>
            <button id=""btnTor"" tag=""template-tor"" label=""شروط مرجعية ToR"" onAction=""RibbonOnAction""/>
            <button id=""btnConcept"" tag=""template-concept-note"" label=""مذكرة مفاهيمية"" onAction=""RibbonOnAction""/>
            <button id=""btnDonor"" tag=""template-donor-report"" label=""تقرير جهة مانحة"" onAction=""RibbonOnAction""/>
            <button id=""btnSop"" tag=""template-sop"" label=""إجراء تشغيلي SOP"" onAction=""RibbonOnAction""/>
            <button id=""btnRisk"" tag=""template-risk-register"" label=""سجل مخاطر"" onAction=""RibbonOnAction""/>
            <button id=""btnAction"" tag=""template-action-plan"" label=""خطة عمل"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuSmartTools"" label=""أدوات سريعة"" imageMso=""HappyFace"">
            <button id=""btnCalculator"" tag=""calculator"" label=""حاسبة التحديد"" onAction=""RibbonOnAction""/>
            <button id=""btnGuid"" tag=""insert-guid"" label=""إدراج UUID"" onAction=""RibbonOnAction""/>
            <button id=""btnLorem"" tag=""insert-lorem-arabic"" label=""نص عربي تجريبي"" onAction=""RibbonOnAction""/>
            <button id=""btnChecklist"" tag=""insert-checklist"" label=""قائمة مهام بمربعات"" onAction=""RibbonOnAction""/>
          </menu>
        </group>
      </tab>
    </tabs>
  </ribbon>
</customUI>";
    }
}
