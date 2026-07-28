namespace WordProSuite.Desktop.Ribbon
{
    internal static class RibbonXml
    {
        internal const string Value = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<customUI xmlns=""http://schemas.microsoft.com/office/2009/07/customui"" onLoad=""RibbonOnLoad"">
  <ribbon>
    <tabs>
      <tab id=""tabWordProCore"" label=""WordPro Suite Pro"">
        <group id=""grpCoreMain"" label=""الرئيسية"">
          <button id=""btnCommandCenter"" tag=""command-center"" label=""مركز 600 أداة"" size=""large"" imageMso=""AddInManager"" onAction=""RibbonOnAction""/>
          <button id=""btnHealth"" tag=""health-check"" label=""فحص الإضافة"" size=""large"" imageMso=""HappyFace"" onAction=""RibbonOnAction""/>
          <button id=""btnAbout"" tag=""about"" label=""حول"" imageMso=""Info"" onAction=""RibbonOnAction""/>
        </group>

        <group id=""grpCoreArabic"" label=""العربية واللغات"">
          <button id=""btnRtl"" tag=""rtl"" label=""RTL"" size=""large"" imageMso=""ParagraphRightToLeft"" onAction=""RibbonOnAction""/>
          <button id=""btnAutoDirection"" tag=""auto-direction"" label=""اتجاه تلقائي"" size=""large"" imageMso=""TextDirectionOptions"" onAction=""RibbonOnAction""/>
          <button id=""btnArabicClean"" tag=""arabic-clean-all"" label=""تنظيف عربي"" size=""large"" imageMso=""ReviewDeleteAllMarkupInDocument"" onAction=""RibbonOnAction""/>
          <menu id=""menuArabicPro"" label=""أدوات عربية"" imageMso=""TextDirectionOptions"">
            <button id=""btnLtr"" tag=""ltr"" label=""LTR"" onAction=""RibbonOnAction""/>
            <button id=""btnArabicNormalize"" tag=""normalize-arabic"" label=""توحيد الحروف"" onAction=""RibbonOnAction""/>
            <button id=""btnArabicPunctuation"" tag=""normalize-arabic-punctuation"" label=""ترقيم عربي ذكي"" onAction=""RibbonOnAction""/>
            <button id=""btnArabicQuotes"" tag=""arabic-quotes"" label=""اقتباس عربي"" onAction=""RibbonOnAction""/>
            <button id=""btnRemoveDiacritics"" tag=""remove-diacritics"" label=""حذف التشكيل"" onAction=""RibbonOnAction""/>
            <button id=""btnRemoveTatweel"" tag=""remove-tatweel"" label=""حذف التطويل"" onAction=""RibbonOnAction""/>
            <button id=""btnPersianLetters"" tag=""persian-to-arabic-letters"" label=""تصحيح الحروف الفارسية"" onAction=""RibbonOnAction""/>
            <button id=""btnEasternDigits"" tag=""digits-eastern"" label=""أرقام عربية"" onAction=""RibbonOnAction""/>
            <button id=""btnWesternDigits"" tag=""digits-western"" label=""أرقام غربية"" onAction=""RibbonOnAction""/>
            <button id=""btnArabicWords"" tag=""number-arabic-words"" label=""تفقيط الرقم"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpCoreText"" label=""النص والتحرير"">
          <button id=""btnSpaces"" tag=""collapse-spaces"" label=""تنظيف المسافات"" size=""large"" imageMso=""ClearFormatting"" onAction=""RibbonOnAction""/>
          <button id=""btnDuplicates"" tag=""remove-duplicate-paragraphs"" label=""حذف التكرار"" size=""large"" imageMso=""ParagraphSpacingAfter"" onAction=""RibbonOnAction""/>
          <menu id=""menuTextCleanup"" label=""تنظيف وتحويل"" imageMso=""TextBoxInsert"">
            <button id=""btnTrimLines"" tag=""trim-lines"" label=""قص حواف الأسطر"" onAction=""RibbonOnAction""/>
            <button id=""btnEmptyParagraphs"" tag=""remove-empty-paragraphs"" label=""حذف الفقرات الفارغة"" onAction=""RibbonOnAction""/>
            <button id=""btnJoinParagraphs"" tag=""join-paragraphs"" label=""دمج الفقرات"" onAction=""RibbonOnAction""/>
            <button id=""btnDuplicateWords"" tag=""remove-duplicate-words"" label=""حذف الكلمات المتكررة"" onAction=""RibbonOnAction""/>
            <button id=""btnSmartQuotes"" tag=""smart-quotes"" label=""اقتباس ذكي"" onAction=""RibbonOnAction""/>
            <button id=""btnDashes"" tag=""normalize-dashes"" label=""توحيد الشرطات"" onAction=""RibbonOnAction""/>
            <button id=""btnHtml"" tag=""strip-html-tags"" label=""إزالة HTML"" onAction=""RibbonOnAction""/>
            <button id=""btnUnicodeSpaces"" tag=""normalize-unicode-spaces"" label=""توحيد مسافات Unicode"" onAction=""RibbonOnAction""/>
            <button id=""btnPunctuation"" tag=""remove-repeated-punctuation"" label=""حذف الترقيم المكرر"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuTextExtract"" label=""استخراج وتحليل"" imageMso=""RefreshAll"">
            <button id=""btnEmails"" tag=""extract-emails"" label=""استخراج البريد"" onAction=""RibbonOnAction""/>
            <button id=""btnUrls"" tag=""extract-urls"" label=""استخراج الروابط"" onAction=""RibbonOnAction""/>
            <button id=""btnPhones"" tag=""extract-phones"" label=""استخراج الهواتف"" onAction=""RibbonOnAction""/>
            <button id=""btnNumbers"" tag=""extract-numbers-pro"" label=""استخراج الأرقام"" onAction=""RibbonOnAction""/>
            <button id=""btnFrequency"" tag=""word-frequency-report"" label=""تكرار الكلمات"" onAction=""RibbonOnAction""/>
            <button id=""btnLongParagraphs"" tag=""long-paragraph-report"" label=""الفقرات الطويلة"" onAction=""RibbonOnAction""/>
            <button id=""btnStats"" tag=""text-statistics"" label=""إحصاءات النص"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpCoreFormat"" label=""التنسيق الاحترافي"">
          <button id=""btnArabicReport"" tag=""format-arabic-report"" label=""تقرير عربي"" size=""large"" imageMso=""StylesPane"" onAction=""RibbonOnAction""/>
          <button id=""btnOfficialLetter"" tag=""format-official-letter"" label=""خطاب رسمي"" size=""large"" imageMso=""FontDialog"" onAction=""RibbonOnAction""/>
          <menu id=""menuFontsCore"" label=""الخطوط"" imageMso=""FontDialog"">
            <button id=""btnArial14"" tag=""font-arial-14"" label=""Arial 14"" onAction=""RibbonOnAction""/>
            <button id=""btnAptos11"" tag=""font-aptos-11"" label=""Aptos 11"" onAction=""RibbonOnAction""/>
            <button id=""btnTahoma14"" tag=""font-tahoma-14"" label=""Tahoma 14"" onAction=""RibbonOnAction""/>
            <button id=""btnTimes14"" tag=""font-times-14"" label=""Times New Roman 14"" onAction=""RibbonOnAction""/>
            <button id=""btnNoto14"" tag=""font-noto-naskh-14"" label=""Noto Naskh Arabic 14"" onAction=""RibbonOnAction""/>
            <button id=""btnTraditional16"" tag=""font-traditional-arabic-16"" label=""Traditional Arabic 16"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuParagraphCore"" label=""الفقرات"" imageMso=""ParagraphDialog"">
            <button id=""btnJustify"" tag=""align-justify"" label=""ضبط كامل"" onAction=""RibbonOnAction""/>
            <button id=""btnLine15"" tag=""line-spacing-15"" label=""تباعد 1.5"" onAction=""RibbonOnAction""/>
            <button id=""btnExact14"" tag=""line-exact-14"" label=""تباعد ثابت 14"" onAction=""RibbonOnAction""/>
            <button id=""btnAfter6"" tag=""paragraph-after-6-pro"" label=""مسافة بعد 6"" onAction=""RibbonOnAction""/>
            <button id=""btnFirstIndent"" tag=""indent-first-0-5cm"" label=""بادئة أول سطر 0.5 سم"" onAction=""RibbonOnAction""/>
            <button id=""btnHanging"" tag=""indent-hanging-0-5cm"" label=""بادئة معلقة 0.5 سم"" onAction=""RibbonOnAction""/>
            <button id=""btnKeepLines"" tag=""keep-lines-together"" label=""إبقاء الأسطر معًا"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpCoreTables"" label=""الجداول"">
          <button id=""btnTableProfessional"" tag=""table-professional"" label=""جدول احترافي"" size=""large"" imageMso=""TableInsertDialogWord"" onAction=""RibbonOnAction""/>
          <button id=""btnTableRtl"" tag=""table-rtl"" label=""جدول RTL"" size=""large"" imageMso=""TableAutoFitWindow"" onAction=""RibbonOnAction""/>
          <menu id=""menuTablesCore"" label=""أدوات الجداول"" imageMso=""TablePropertiesDialog"">
            <button id=""btnTableFit"" tag=""table-autofit-window"" label=""ملاءمة الصفحة"" onAction=""RibbonOnAction""/>
            <button id=""btnTableHeader"" tag=""table-header-style"" label=""تنسيق صف العنوان"" onAction=""RibbonOnAction""/>
            <button id=""btnTableBand"" tag=""table-banded-rows"" label=""صفوف متبادلة"" onAction=""RibbonOnAction""/>
            <button id=""btnTableRepeat"" tag=""table-repeat-header"" label=""تكرار صف العنوان"" onAction=""RibbonOnAction""/>
            <button id=""btnTableTrim"" tag=""table-trim-cells"" label=""تنظيف الخلايا"" onAction=""RibbonOnAction""/>
            <button id=""btnTableDuplicates"" tag=""table-remove-duplicate-rows"" label=""حذف الصفوف المكررة"" onAction=""RibbonOnAction""/>
            <button id=""btnTableNumber"" tag=""table-number-first-column"" label=""ترقيم العمود الأول"" onAction=""RibbonOnAction""/>
            <button id=""btnTableSum"" tag=""table-sum-column"" label=""مجموع العمود"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpCoreDocument"" label=""المستند والصفحات"">
          <button id=""btnDashboard"" tag=""document-dashboard"" label=""لوحة المستند"" size=""large"" imageMso=""RefreshAll"" onAction=""RibbonOnAction""/>
          <button id=""btnToc"" tag=""insert-toc"" label=""إضافة فهرس"" size=""large"" imageMso=""TableOfContentsAddTextGallery"" onAction=""RibbonOnAction""/>
          <menu id=""menuPagesCore"" label=""إعداد الصفحة"" imageMso=""PageSetupDialog"">
            <button id=""btnPaperA4"" tag=""paper-a4-pro"" label=""ورق A4"" onAction=""RibbonOnAction""/>
            <button id=""btnPortrait"" tag=""orientation-portrait-pro"" label=""عمودي"" onAction=""RibbonOnAction""/>
            <button id=""btnLandscape"" tag=""orientation-landscape-pro"" label=""أفقي"" onAction=""RibbonOnAction""/>
            <button id=""btnMarginsOfficial"" tag=""margins-official-pro"" label=""هوامش خطاب رسمي"" onAction=""RibbonOnAction""/>
            <button id=""btnMarginsAcademic"" tag=""margins-academic-pro"" label=""هوامش أكاديمية"" onAction=""RibbonOnAction""/>
            <button id=""btnColumns2"" tag=""columns-2-pro"" label=""عمودان"" onAction=""RibbonOnAction""/>
            <button id=""btnPageNumbers"" tag=""page-numbers-add"" label=""أرقام الصفحات"" onAction=""RibbonOnAction""/>
            <button id=""btnUpdateFields"" tag=""update-fields"" label=""تحديث الحقول"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpCoreFiles"" label=""الملفات وPDF"">
          <button id=""btnBackup"" tag=""save-backup"" label=""نسخة احتياطية"" size=""large"" imageMso=""FileSaveAs"" onAction=""RibbonOnAction""/>
          <button id=""btnPdf"" tag=""export-pdf"" label=""PDF"" size=""large"" imageMso=""FileSaveAsPdfOrXps"" onAction=""RibbonOnAction""/>
          <button id=""btnFinal"" tag=""workflow-final-delivery"" label=""نسخة نهائية"" size=""large"" imageMso=""FileCheckIn"" onAction=""RibbonOnAction""/>
        </group>
      </tab>

      <tab id=""tabWordProEnterprise"" label=""WordPro Enterprise"">
        <group id=""grpEnterpriseMain"" label=""مركز المؤسسة"">
          <button id=""btnEnterpriseCenter"" tag=""command-center"" label=""كل الأدوات"" size=""large"" imageMso=""AddInManager"" onAction=""RibbonOnAction""/>
          <button id=""btnExecutiveSummary"" tag=""insert-executive-summary"" label=""ملخص تنفيذي"" size=""large"" imageMso=""NewDocumentOrTemplate"" onAction=""RibbonOnAction""/>
          <button id=""btnDocumentControl"" tag=""insert-document-control"" label=""ضبط المستند"" size=""large"" imageMso=""FileCheckIn"" onAction=""RibbonOnAction""/>
        </group>

        <group id=""grpEnterpriseFrameworks"" label=""أطر العمل العالمية"">
          <menu id=""menuGovernance"" label=""الحوكمة والإدارة"" size=""large"" imageMso=""TableInsertDialogWord"">
            <button id=""btnRaci"" tag=""insert-raci-matrix-v3"" label=""مصفوفة RACI"" onAction=""RibbonOnAction""/>
            <button id=""btnSwot"" tag=""insert-swot-matrix-v3"" label=""تحليل SWOT"" onAction=""RibbonOnAction""/>
            <button id=""btnPestle"" tag=""insert-pestle-matrix"" label=""تحليل PESTLE"" onAction=""RibbonOnAction""/>
            <button id=""btnKpi"" tag=""insert-kpi-dashboard"" label=""لوحة KPI"" onAction=""RibbonOnAction""/>
            <button id=""btnRiskV3"" tag=""insert-risk-register-v3"" label=""سجل المخاطر"" onAction=""RibbonOnAction""/>
            <button id=""btnIssuesV3"" tag=""insert-issue-register-v3"" label=""سجل المشكلات"" onAction=""RibbonOnAction""/>
            <button id=""btnActionsV3"" tag=""insert-action-log-v3"" label=""سجل الإجراءات"" onAction=""RibbonOnAction""/>
            <button id=""btnDecisionsV3"" tag=""insert-decision-log-v3"" label=""سجل القرارات"" onAction=""RibbonOnAction""/>
            <button id=""btnChangeRequest"" tag=""insert-change-request"" label=""طلبات التغيير"" onAction=""RibbonOnAction""/>
            <button id=""btnLessons"" tag=""insert-lessons-learned"" label=""الدروس المستفادة"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuOperations"" label=""التشغيل واللوجستيات"" size=""large"" imageMso=""TablePropertiesDialog"">
            <button id=""btnStakeholders"" tag=""insert-stakeholder-register"" label=""أصحاب المصلحة"" onAction=""RibbonOnAction""/>
            <button id=""btnCommunication"" tag=""insert-communication-plan"" label=""خطة الاتصالات"" onAction=""RibbonOnAction""/>
            <button id=""btnProcurement"" tag=""insert-procurement-plan"" label=""خطة المشتريات"" onAction=""RibbonOnAction""/>
            <button id=""btnInventory"" tag=""insert-inventory-register-v3"" label=""سجل المخزون"" onAction=""RibbonOnAction""/>
            <button id=""btnAssets"" tag=""insert-asset-register"" label=""سجل الأصول"" onAction=""RibbonOnAction""/>
            <button id=""btnIncident"" tag=""insert-incident-register"" label=""سجل الحوادث"" onAction=""RibbonOnAction""/>
            <button id=""btnAudit"" tag=""insert-audit-findings"" label=""نتائج التدقيق"" onAction=""RibbonOnAction""/>
            <button id=""btnTraining"" tag=""insert-training-matrix"" label=""مصفوفة التدريب"" onAction=""RibbonOnAction""/>
            <button id=""btnBudget"" tag=""insert-budget-tracker"" label=""متابعة الميزانية"" onAction=""RibbonOnAction""/>
            <button id=""btnContract"" tag=""insert-contract-tracker"" label=""متابعة العقود"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpEnterpriseAnalytics"" label=""ذكاء المستند"">
          <button id=""btnReadability"" tag=""metric-readability-pro"" label=""قابلية القراءة"" size=""large"" imageMso=""RefreshAll"" onAction=""RibbonOnAction""/>
          <menu id=""menuMetrics"" label=""المؤشرات"" imageMso=""HappyFace"">
            <button id=""btnMetricWords"" tag=""metric-words-pro"" label=""الكلمات"" onAction=""RibbonOnAction""/>
            <button id=""btnMetricPages"" tag=""metric-pages-pro"" label=""الصفحات"" onAction=""RibbonOnAction""/>
            <button id=""btnMetricTables"" tag=""metric-tables-pro"" label=""الجداول"" onAction=""RibbonOnAction""/>
            <button id=""btnMetricImages"" tag=""metric-images-pro"" label=""الصور"" onAction=""RibbonOnAction""/>
            <button id=""btnMetricComments"" tag=""metric-comments-pro"" label=""التعليقات"" onAction=""RibbonOnAction""/>
            <button id=""btnMetricRevisions"" tag=""metric-revisions-pro"" label=""التعديلات"" onAction=""RibbonOnAction""/>
            <button id=""btnMetricFields"" tag=""metric-fields-pro"" label=""الحقول"" onAction=""RibbonOnAction""/>
            <button id=""btnMetricHeadings"" tag=""metric-headings-pro"" label=""العناوين"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpEnterprisePresets"" label=""إعدادات عالمية"">
          <menu id=""menuGlobalFonts"" label=""خطوط عالمية"" size=""large"" imageMso=""FontDialog"">
            <button id=""btnFontAptos12"" tag=""font-aptos-12"" label=""Aptos 12"" onAction=""RibbonOnAction""/>
            <button id=""btnFontCambria12"" tag=""font-cambria-12"" label=""Cambria 12"" onAction=""RibbonOnAction""/>
            <button id=""btnFontGeorgia12"" tag=""font-georgia-12"" label=""Georgia 12"" onAction=""RibbonOnAction""/>
            <button id=""btnFontSegoe12"" tag=""font-segoe-12"" label=""Segoe UI 12"" onAction=""RibbonOnAction""/>
            <button id=""btnFontDubai12"" tag=""font-dubai-12"" label=""Dubai 12"" onAction=""RibbonOnAction""/>
            <button id=""btnFontSimplified14"" tag=""font-simplified-arabic-14"" label=""Simplified Arabic 14"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuLayoutWorld"" label=""تخطيط عالمي"" size=""large"" imageMso=""PageSetupDialog"">
            <button id=""btnA3"" tag=""paper-a3-pro"" label=""A3"" onAction=""RibbonOnAction""/>
            <button id=""btnA5"" tag=""paper-a5-pro"" label=""A5"" onAction=""RibbonOnAction""/>
            <button id=""btnLetter"" tag=""paper-letter-pro"" label=""Letter"" onAction=""RibbonOnAction""/>
            <button id=""btnLegal"" tag=""paper-legal-pro"" label=""Legal"" onAction=""RibbonOnAction""/>
            <button id=""btnBindingMargins"" tag=""margins-binding-pro"" label=""هوامش تجليد"" onAction=""RibbonOnAction""/>
            <button id=""btnColumns3"" tag=""columns-3-pro"" label=""3 أعمدة"" onAction=""RibbonOnAction""/>
            <button id=""btnLineNumbers"" tag=""line-numbers-continuous-pro"" label=""ترقيم الأسطر"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpEnterpriseWorkspace"" label=""مساحة العمل"">
          <menu id=""menuZoom"" label=""التكبير"" imageMso=""PageSetupDialog"">
            <button id=""btnZoom75"" tag=""zoom-75-pro"" label=""75%"" onAction=""RibbonOnAction""/>
            <button id=""btnZoom100"" tag=""zoom-100-pro"" label=""100%"" onAction=""RibbonOnAction""/>
            <button id=""btnZoom125"" tag=""zoom-125-pro"" label=""125%"" onAction=""RibbonOnAction""/>
            <button id=""btnZoom150"" tag=""zoom-150-pro"" label=""150%"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuViews"" label=""طرق العرض"" imageMso=""PageSetupDialog"">
            <button id=""btnPrintView"" tag=""view-print-pro"" label=""تخطيط الطباعة"" onAction=""RibbonOnAction""/>
            <button id=""btnOutlineView"" tag=""view-outline-pro"" label=""المخطط"" onAction=""RibbonOnAction""/>
            <button id=""btnWebView"" tag=""view-web-pro"" label=""الويب"" onAction=""RibbonOnAction""/>
            <button id=""btnReadingView"" tag=""view-reading-pro"" label=""القراءة"" onAction=""RibbonOnAction""/>
            <button id=""btnMarksOn"" tag=""formatting-marks-on"" label=""إظهار العلامات"" onAction=""RibbonOnAction""/>
            <button id=""btnMarksOff"" tag=""formatting-marks-off"" label=""إخفاء العلامات"" onAction=""RibbonOnAction""/>
            <button id=""btnRulersOn"" tag=""rulers-on"" label=""إظهار المساطر"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpEnterpriseReview"" label=""المراجعة والأمان"">
          <button id=""btnCommentsReport"" tag=""comments-report-pro"" label=""تقرير التعليقات"" size=""large"" imageMso=""RefreshAll"" onAction=""RibbonOnAction""/>
          <button id=""btnRevisionsSummary"" tag=""revisions-summary-pro"" label=""ملخص التعديلات"" size=""large"" imageMso=""FileCheckIn"" onAction=""RibbonOnAction""/>
          <menu id=""menuSecurity"" label=""الأمان"" imageMso=""Info"">
            <button id=""btnProtect"" tag=""protect-readonly"" label=""حماية المستند"" onAction=""RibbonOnAction""/>
            <button id=""btnUnprotect"" tag=""unprotect"" label=""إلغاء الحماية"" onAction=""RibbonOnAction""/>
            <button id=""btnMetadata"" tag=""remove-metadata"" label=""إزالة البيانات الشخصية"" onAction=""RibbonOnAction""/>
            <button id=""btnLinks"" tag=""remove-external-links"" label=""إزالة الروابط الخارجية"" onAction=""RibbonOnAction""/>
            <button id=""btnConfidential"" tag=""insert-confidential-banner"" label=""شريط سري"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpEnterpriseLicense"" label=""الترخيص والدعم"">
          <button id=""btnActivate"" tag=""activate"" label=""تفعيل"" size=""large"" imageMso=""HappyFace"" onAction=""RibbonOnAction""/>
          <button id=""btnLicense"" tag=""license-info"" label=""حالة الترخيص"" imageMso=""Info"" onAction=""RibbonOnAction""/>
          <button id=""btnMachine"" tag=""machine-id"" label=""Machine ID"" imageMso=""AddInManager"" onAction=""RibbonOnAction""/>
        </group>
      </tab>

      <tab id=""tabUltra600AI"" label=""Ultra 600 AI"">
        <group id=""grpUltraMain"" label=""موسوعة الأدوات"">
          <button id=""btnUltraCatalog"" tag=""catalog-600-center"" label=""موسوعة 600 أداة"" size=""large"" imageMso=""AddInManager"" onAction=""RibbonOnAction""/>
          <button id=""btnUltraCenter"" tag=""command-center"" label=""بحث كل الأدوات"" size=""large"" imageMso=""RefreshAll"" onAction=""RibbonOnAction""/>
          <button id=""btnUltraHealth"" tag=""health-check"" label=""فحص النظام"" imageMso=""HappyFace"" onAction=""RibbonOnAction""/>
        </group>

        <group id=""grpUltraWorkspace"" label=""المستندات الذكية"">
          <button id=""btnWorkspaceSave"" tag=""workspace-save-session"" label=""حفظ مساحة العمل"" size=""large"" imageMso=""FileSaveAs"" onAction=""RibbonOnAction""/>
          <button id=""btnWorkspaceRestore"" tag=""workspace-restore-session"" label=""استعادة المساحة"" size=""large"" imageMso=""FileOpen"" onAction=""RibbonOnAction""/>
          <menu id=""menuWorkspaceUltra"" label=""إدارة المستندات"" imageMso=""PageSetupDialog"">
            <button id=""btnWorkspaceExport"" tag=""workspace-export-open-docs"" label=""تصدير المستندات المفتوحة"" onAction=""RibbonOnAction""/>
            <button id=""btnWorkspaceCloseClean"" tag=""workspace-close-unmodified"" label=""إغلاق غير المعدّل"" onAction=""RibbonOnAction""/>
            <button id=""btnWorkspaceCloseOthers"" tag=""workspace-close-others"" label=""إغلاق المستندات الأخرى"" onAction=""RibbonOnAction""/>
            <button id=""btnWorkspaceRename"" tag=""workspace-rename-document"" label=""إعادة تسمية المستند"" onAction=""RibbonOnAction""/>
            <button id=""btnWorkspaceDuplicate"" tag=""workspace-duplicate-document"" label=""إنشاء نسخة"" onAction=""RibbonOnAction""/>
            <button id=""btnWorkspaceCompare"" tag=""workspace-compare-file"" label=""مقارنة مع ملف"" onAction=""RibbonOnAction""/>
            <button id=""btnWorkspaceArrange"" tag=""workspace-arrange-windows"" label=""ترتيب النوافذ"" onAction=""RibbonOnAction""/>
            <button id=""btnWorkspaceSplit"" tag=""workspace-split-toggle"" label=""تقسيم النافذة"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpUltraSmart"" label=""الذكاء المحلي وAI"">
          <button id=""btnOfflineSummary"" tag=""smart-offline-summary"" label=""تلخيص دون إنترنت"" size=""large"" imageMso=""RefreshAll"" onAction=""RibbonOnAction""/>
          <button id=""btnExecutiveSmart"" tag=""smart-executive-summary"" label=""ملخص تنفيذي"" size=""large"" imageMso=""RefreshAll"" onAction=""RibbonOnAction""/>
          <menu id=""menuSmartOffline"" label=""تحليل ذكي محلي"" imageMso=""RefreshAll"">
            <button id=""btnSmartKeywords"" tag=""smart-keywords"" label=""الكلمات المفتاحية"" onAction=""RibbonOnAction""/>
            <button id=""btnSmartActions"" tag=""smart-action-items"" label=""إجراءات العمل"" onAction=""RibbonOnAction""/>
            <button id=""btnSmartDecisions"" tag=""smart-decisions"" label=""القرارات"" onAction=""RibbonOnAction""/>
            <button id=""btnSmartQuestions"" tag=""smart-reading-questions"" label=""أسئلة قراءة"" onAction=""RibbonOnAction""/>
            <button id=""btnSmartSentiment"" tag=""smart-sentiment"" label=""تحليل النبرة"" onAction=""RibbonOnAction""/>
            <button id=""btnSmartReadability"" tag=""smart-readability-advice"" label=""قابلية القراءة"" onAction=""RibbonOnAction""/>
            <button id=""btnSmartTitles"" tag=""smart-title-suggestions"" label=""اقتراح عناوين"" onAction=""RibbonOnAction""/>
            <button id=""btnSmartOutline"" tag=""smart-outline"" label=""مخطط هيكلي"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuAiProvider"" label=""موفر AI اختياري"" imageMso=""RefreshAll"">
            <button id=""btnAiSettings"" tag=""ai-provider-settings"" label=""إعداد الموفر"" onAction=""RibbonOnAction""/>
            <button id=""btnAiRewrite"" tag=""ai-rewrite-provider"" label=""إعادة صياغة"" onAction=""RibbonOnAction""/>
            <button id=""btnAiTranslate"" tag=""ai-translate-provider"" label=""ترجمة"" onAction=""RibbonOnAction""/>
            <button id=""btnAiChat"" tag=""ai-chat-provider"" label=""دردشة مع المستند"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpUltraBatchMedia"" label=""الميديا والدفعات"">
          <menu id=""menuUltraMedia"" label=""الصور والوسائط"" size=""large"" imageMso=""FileOpen"">
            <button id=""btnMediaExport"" tag=""media-export-images"" label=""تصدير الصور"" onAction=""RibbonOnAction""/>
            <button id=""btnMediaHalf"" tag=""media-resize-half"" label=""تصغير 50%"" onAction=""RibbonOnAction""/>
            <button id=""btnMediaFit"" tag=""media-fit-page"" label=""ملاءمة الصفحة"" onAction=""RibbonOnAction""/>
            <button id=""btnMediaCenter"" tag=""media-center-images"" label=""توسيط الصور"" onAction=""RibbonOnAction""/>
            <button id=""btnMediaCaptions"" tag=""media-caption-images"" label=""إضافة تسميات"" onAction=""RibbonOnAction""/>
            <button id=""btnMediaCompress"" tag=""media-compress-pictures"" label=""ضغط الصور"" onAction=""RibbonOnAction""/>
            <button id=""btnMediaAccess"" tag=""media-accessibility-report"" label=""تقرير الوصول"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuUltraBatch"" label=""عمليات دفعية"" size=""large"" imageMso=""FileSaveAsPdfOrXps"">
            <button id=""btnBatchDocx"" tag=""batch-save-docx"" label=""مجلد إلى DOCX"" onAction=""RibbonOnAction""/>
            <button id=""btnBatchPdf"" tag=""batch-export-pdf-v4"" label=""مجلد إلى PDF"" onAction=""RibbonOnAction""/>
            <button id=""btnBatchComments"" tag=""batch-remove-comments"" label=""حذف التعليقات"" onAction=""RibbonOnAction""/>
            <button id=""btnBatchRevisions"" tag=""batch-accept-revisions"" label=""قبول التعديلات"" onAction=""RibbonOnAction""/>
            <button id=""btnBatchFields"" tag=""batch-update-fields"" label=""تحديث الحقول"" onAction=""RibbonOnAction""/>
            <button id=""btnBatchMetadata"" tag=""batch-remove-properties"" label=""تنظيف الخصائص"" onAction=""RibbonOnAction""/>
            <button id=""btnBatchPrint"" tag=""batch-print-documents"" label=""طباعة المجلد"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpUltraSecurityAcademic"" label=""الأبحاث والأمان"">
          <menu id=""menuUltraAcademic"" label=""بحث ومراجع"" size=""large"" imageMso=""TableOfContentsAddTextGallery"">
            <button id=""btnApa"" tag=""academic-insert-apa"" label=""مرجع APA"" onAction=""RibbonOnAction""/>
            <button id=""btnMla"" tag=""academic-insert-mla"" label=""مرجع MLA"" onAction=""RibbonOnAction""/>
            <button id=""btnBibAudit"" tag=""academic-bibliography-audit"" label=""تدقيق المراجع"" onAction=""RibbonOnAction""/>
            <button id=""btnFootAudit"" tag=""academic-footnote-audit"" label=""تدقيق الحواشي"" onAction=""RibbonOnAction""/>
            <button id=""btnHeadingAudit"" tag=""academic-heading-audit"" label=""تدقيق العناوين"" onAction=""RibbonOnAction""/>
            <button id=""btnExtractCitations"" tag=""academic-extract-citations"" label=""استخراج الاستشهادات"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuUltraSecurity"" label=""الطمس والأمان"" size=""large"" imageMso=""Info"">
            <button id=""btnRedactEmail"" tag=""security-redact-emails"" label=""طمس البريد"" onAction=""RibbonOnAction""/>
            <button id=""btnRedactPhone"" tag=""security-redact-phones"" label=""طمس الهواتف"" onAction=""RibbonOnAction""/>
            <button id=""btnRedactId"" tag=""security-redact-identifiers"" label=""طمس المعرّفات"" onAction=""RibbonOnAction""/>
            <button id=""btnRedactSelection"" tag=""security-redact-selection"" label=""طمس التحديد"" onAction=""RibbonOnAction""/>
            <button id=""btnSecurityAudit"" tag=""security-document-audit"" label=""تدقيق الأمان"" onAction=""RibbonOnAction""/>
            <button id=""btnHiddenDelete"" tag=""security-remove-hidden-text"" label=""حذف النص المخفي"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpUltraProjectsForms"" label=""المشاريع والنماذج"">
          <menu id=""menuUltraProjects"" label=""إدارة المشاريع"" size=""large"" imageMso=""TableInsertDialogWord"">
            <button id=""btnGantt"" tag=""project-gantt-table"" label=""Gantt مبسط"" onAction=""RibbonOnAction""/>
            <button id=""btnTaskBoard"" tag=""project-task-board"" label=""لوحة المهام"" onAction=""RibbonOnAction""/>
            <button id=""btnWeeklyStatus"" tag=""project-weekly-status"" label=""حالة أسبوعية"" onAction=""RibbonOnAction""/>
            <button id=""btnDeliverables"" tag=""project-deliverables-register"" label=""سجل المخرجات"" onAction=""RibbonOnAction""/>
            <button id=""btnLegalChecklist"" tag=""legal-clause-checklist"" label=""فحص العقد"" onAction=""RibbonOnAction""/>
            <button id=""btnLegalParties"" tag=""legal-party-table"" label=""أطراف العقد"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuUltraForms"" label=""نماذج تفاعلية"" size=""large"" imageMso=""NewDocumentOrTemplate"">
            <button id=""btnFormText"" tag=""form-text-control"" label=""حقل نص"" onAction=""RibbonOnAction""/>
            <button id=""btnFormDate"" tag=""form-date-control"" label=""حقل تاريخ"" onAction=""RibbonOnAction""/>
            <button id=""btnFormCheck"" tag=""form-checkbox-control"" label=""مربع اختيار"" onAction=""RibbonOnAction""/>
            <button id=""btnFormExport"" tag=""form-export-controls-csv"" label=""تصدير CSV"" onAction=""RibbonOnAction""/>
          </menu>
        </group>

        <group id=""grpUltraProductivity"" label=""الإنتاجية والتصدير"">
          <button id=""btnFocusMode"" tag=""productivity-focus-mode"" label=""وضع التركيز"" size=""large"" imageMso=""PageSetupDialog"" onAction=""RibbonOnAction""/>
          <button id=""btnWordGoal"" tag=""productivity-word-goal"" label=""هدف الكلمات"" size=""large"" imageMso=""HappyFace"" onAction=""RibbonOnAction""/>
          <menu id=""menuProductivityUltra"" label=""الجلسة"" imageMso=""RefreshAll"">
            <button id=""btnSessionStart"" tag=""productivity-writing-session"" label=""بدء جلسة كتابة"" onAction=""RibbonOnAction""/>
            <button id=""btnSessionReport"" tag=""productivity-session-report"" label=""تقرير الجلسة"" onAction=""RibbonOnAction""/>
            <button id=""btnReadingTime"" tag=""productivity-reading-time"" label=""زمن القراءة"" onAction=""RibbonOnAction""/>
            <button id=""btnRestoreView"" tag=""productivity-restore-view"" label=""استعادة الواجهة"" onAction=""RibbonOnAction""/>
          </menu>
          <menu id=""menuExportUltra"" label=""تصدير متقدم"" imageMso=""FileSaveAs"">
            <button id=""btnHtmlV4"" tag=""export-filtered-html"" label=""HTML"" onAction=""RibbonOnAction""/>
            <button id=""btnRtfV4"" tag=""export-rtf"" label=""RTF"" onAction=""RibbonOnAction""/>
            <button id=""btnOdtV4"" tag=""export-odt"" label=""ODT"" onAction=""RibbonOnAction""/>
            <button id=""btnPdfRangeV4"" tag=""export-pdf-page-range"" label=""نطاق PDF"" onAction=""RibbonOnAction""/>
            <button id=""btnBookletV4"" tag=""print-booklet-layout"" label=""إعداد كتيب"" onAction=""RibbonOnAction""/>
          </menu>
        </group>
      </tab>
    </tabs>
  </ribbon>
</customUI>";
    }
}
