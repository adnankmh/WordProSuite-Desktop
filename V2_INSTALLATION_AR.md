# طريقة التحديث والبناء

1. فك ضغط الحزمة.
2. انسخ المجلدات `.github` و`scripts` و`src` فوق المجلدات نفسها في مستودع `WordProSuite-Desktop`.
3. وافق على استبدال الملفات.
4. في GitHub Desktop نفّذ Commit بعنوان `WordPro Suite Desktop Pro v2` ثم Push origin.
5. بعد نجاح GitHub Actions نزّل Artifact باسم `WordProSuite-Desktop-Pro-v2-Windows`.
6. فك الضغط مع إبقاء مجلد `Payload` بجانب `WordProSuite_Setup.exe`.
7. أغلق Word، شغّل `WordProSuite_Setup.exe` واضغط «تثبيت».
8. افتح Word ثم اضغط «تفعيل البرنامج».

المثبت الجديد يسجل الإضافة للحساب الحالي في مساري Registry 32 و64 بت، لذلك يوجد Setup واحد لمختلف معماريات Office.
