# WordPro Suite Desktop Ultimate 4.0

إضافة COM مكتبية لـ Microsoft Word 2024 مبنية على .NET Framework 4.8، تعمل دون VBA أو Office.js أو Node.js أو localhost.

## الإصدار

- 600 أمر مسجّل وفريد.
- 15 محركًا تخصصيًا.
- 3 تبويبات Ribbon: `WordPro Suite Pro` و`WordPro Enterprise` و`Ultra 600 AI`.
- ملف تثبيت واحد: `WordProSuite_Setup.exe`.
- تثبيت لكل مستخدم داخل LocalAppData مع تسجيل COM تحت HKCU.
- تجربة وتفعيل وإصلاح وإزالة من واجهة Setup واحدة.
- نظام Serial Number بمفتاح عام داخل البرنامج ومفتاح خاص منفصل لدى المالك.

## البناء

شغّل GitHub Actions، ثم نزّل Artifact:

```text
WordProSuite-Desktop-Ultimate-4.0-Windows
```

يحتوي على:

```text
WordProSuite_Setup.exe
INSTALL_AR.txt
FEATURES_AR.md
ULTIMATE_600_CATALOG.json
ULTIMATE_600_CATALOG.csv
RELEASE_MANIFEST.json
SHA256_SETUP.txt
```

## الذكاء الاصطناعي

الأدوات الذكية المحلية مثل التلخيص والكلمات المفتاحية واستخراج القرارات والأسئلة تعمل دون إنترنت. أدوات التوليد والترجمة والدردشة الحقيقية تدعم موفرًا اختياريًا متوافقًا مع OpenAI مثل Ollama أو LM Studio أو خادم OpenAI-Compatible. لا يتم تضمين مفتاح API أو نموذج مدفوع داخل البرنامج.

## ملاحظة هندسية

الدليل المرجعي المرفق يذكر 600 موضع ميزة، لكنه يستخدم وصفًا عامًا متكررًا في عدد كبير منها. تم تحويل هذه المواضع العامة إلى أدوات فعلية محددة وقابلة للتشغيل بدل إنشاء أزرار شكلية أو أوامر غير منفذة.
