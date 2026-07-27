# WordPro Suite Desktop Ultimate 3.0

إضافة COM مكتبية أصلية لبرنامج Microsoft Word، تعمل محليًا بالكامل دون Node.js أو localhost أو VBA. يحتوي الإصدار على **500 أداة فريدة** موزعة على **30 قسمًا**، مع تبويبتين دائمتين في Ribbon:

- **WordPro Suite Pro** للأدوات اليومية والتحرير والتنسيق والجداول والملفات.
- **WordPro Enterprise** لأطر العمل المؤسسية والتحليل والقوالب العالمية ومساحة العمل والترخيص.

## التثبيت

بعد نجاح GitHub Actions نزّل Artifact باسم:

```text
WordProSuite-Desktop-Ultimate-3.0-Windows
```

ثم شغّل الملف الوحيد:

```text
WordProSuite_Setup.exe
```

لا يحتاج المستخدم إلى MSI أو Payload أو تشغيل PowerShell يدويًا. يقوم Setup باستخراج DLL المضمّن، وتسجيل COM لكل مستخدم، وضبط Word، والتحقق من إنشاء كائن COM قبل إعلان نجاح التثبيت.

## التفعيل

يدعم البرنامج:

- تجربة محلية محدودة.
- Serial Number دائم.
- Serial Number بتاريخ انتهاء.
- ربط الترخيص بجهاز واحد بواسطة Machine ID.
- إصدارات PRO وENTERPRISE وULTIMATE.
- إصلاح وإزالة وإعادة تثبيت من شاشة Setup نفسها.

المفتاح الخاص بإنشاء الأرقام لا يوجد في المشروع العام ولا داخل Setup.

## أهم المجموعات

- العربية والاتجاه والكتابة ثنائية اللغة.
- النص والتحرير والتنظيف والاستخراج.
- الخطوط العالمية والتنسيق والألوان.
- الفقرات الدقيقة وتخطيط الصفحة المتقدم.
- الجداول والجداول المتقدمة وقوالب المؤسسات.
- المستند والصفحات والحقول والأتمتة.
- المراجعة والأمان والخصوصية.
- الملفات وPDF والنسخ الاحتياطية.
- ذكاء المستند والتحليل والإحصاء.
- 30 قالبًا مؤسسيًا عالميًا جديدًا.

## البناء

```powershell
.\scripts\validate-source.ps1
.\scripts\build-release.ps1 -Configuration Release
```

ينتج البناء:

```text
release\WordProSuite_Setup.exe
release\INSTALL_AR.txt
release\SHA256_SETUP.txt
release\RELEASE_MANIFEST.json
WordProSuite_Desktop_Ultimate_V3_Windows.zip
```

## بوابات الجودة

- 500 Command IDs فريدة.
- تبويبتان Ribbon.
- كل Ribbon tag مرتبط بأمر مسجل.
- فحص مشكلة CS1977 الخاصة بـ dynamic/lambda.
- فحص Prompt namespace لمنع CS0103.
- بناء مع `-warnaserror`.
- فحص تضمين DLL فعليًا داخل Setup.
- التحقق من SHA-256 للحزمة.
- فحص عدم تسرب المفتاح الخاص.
