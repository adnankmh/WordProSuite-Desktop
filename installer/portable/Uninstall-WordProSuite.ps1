$prog="WordProSuite.Desktop.AddIn";$clsid="{79D9E91D-88D5-4C41-B805-82D64D1348B2}"
Get-Process WINWORD -ErrorAction SilentlyContinue|Stop-Process -Force -ErrorAction SilentlyContinue
Remove-Item "HKCU:\Software\Microsoft\Office\Word\Addins\$prog" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "HKCU:\Software\Classes\$prog" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "HKCU:\Software\Classes\CLSID\$clsid" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item (Join-Path $env:LOCALAPPDATA "WordProSuite") -Recurse -Force -ErrorAction SilentlyContinue
