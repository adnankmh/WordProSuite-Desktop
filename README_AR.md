# WordPro Suite Desktop V1 — حزمة البناء الاحترافية

هذه إعادة بناء مستقلة لبرنامج WordPro Suite كإضافة سطح مكتب حقيقية لـMicrosoft Word.

## ما الذي تغيّر؟

- لا تستخدم VBA أو DOTM.
- لا تستخدم Office.js.
- لا تستخدم Node.js.
- لا تستخدم localhost.
- لا توجد نافذة أوامر أثناء تشغيل الإضافة.
- Ribbon ثابت داخل Word.
- مركز أدوات WinForms كبير بأزرار وخطوط واضحة.
- تسجيل تلقائي مع بدء Word.
- MSI منفصل لـOffice 32-bit وOffice 64-bit.
- مشغّل EXE يكتشف معمارية Office.
- دعم Install / Repair / Uninstall.
- سجل تشخيص داخل `%LOCALAPPDATA%\WordProSuite\Logs`.

## الأدوات

عدد الأوامر المسجلة في المصدر: **67**.

تشمل العربية وRTL، تنظيف النص، التنسيق، الأنماط، الجداول، الصفحات، المراجعة، الخصوصية، PDF، النسخ الاحتياطي، القوالب، وتجهيز النسخة النهائية.

## البناء على Windows

```powershell
choco install wixtoolset --version=3.14.1 -y
.\scripts\build-release.ps1
```

الناتج المتوقع:

```text
release\WordProSuite_Setup.exe
release\Installers\WordProSuite.Desktop.x86.msi
release\Installers\WordProSuite.Desktop.x64.msi
```

## الاختبار الحقيقي

لا يوصف الناتج بأنه نهائي قبل تشغيل Workflow `Test on Office 2024` على جهاز Windows يحتوي Office 2024 فعليًا.
