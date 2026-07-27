$ErrorActionPreference="Stop"
$prog="WordProSuite.Desktop.AddIn"
$clsid="{79D9E91D-88D5-4C41-B805-82D64D1348B2}"
$src=Join-Path $PSScriptRoot "Payload\WordProSuite.AddIn.dll"
$dir=Join-Path $env:LOCALAPPDATA "WordProSuite\Desktop"
$dll=Join-Path $dir "WordProSuite.AddIn.dll"
if(!(Test-Path $src)){throw "Payload DLL missing"}
Get-Process WINWORD -ErrorAction SilentlyContinue|Stop-Process -Force -ErrorAction SilentlyContinue
New-Item $dir -ItemType Directory -Force|Out-Null
Copy-Item $src $dll -Force
$an=[Reflection.AssemblyName]::GetAssemblyName($dll)
$full=$an.FullName
$rv=[Reflection.Assembly]::ReflectionOnlyLoadFrom($dll).ImageRuntimeVersion
$cb=(New-Object Uri($dll)).AbsoluteUri
$cr="HKCU:\Software\Classes\CLSID\$clsid"
$ip="$cr\InprocServer32"
$vk="$ip\$($an.Version)"
$pr="HKCU:\Software\Classes\$prog"
New-Item $cr -Force|Out-Null
Set-ItemProperty $cr "(default)" "WordPro Suite Desktop Add-in"
New-Item $ip -Force|Out-Null
Set-ItemProperty $ip "(default)" "mscoree.dll"
New-ItemProperty $ip ThreadingModel "Both" -PropertyType String -Force|Out-Null
New-ItemProperty $ip Class "WordProSuite.Desktop.WordProAddIn" -PropertyType String -Force|Out-Null
New-ItemProperty $ip Assembly $full -PropertyType String -Force|Out-Null
New-ItemProperty $ip RuntimeVersion $rv -PropertyType String -Force|Out-Null
New-ItemProperty $ip CodeBase $cb -PropertyType String -Force|Out-Null
New-Item $vk -Force|Out-Null
foreach($n in "Class","Assembly","RuntimeVersion","CodeBase"){
  $v=@{"Class"="WordProSuite.Desktop.WordProAddIn";"Assembly"=$full;"RuntimeVersion"=$rv;"CodeBase"=$cb}[$n]
  New-ItemProperty $vk $n $v -PropertyType String -Force|Out-Null
}
New-Item "$cr\ProgId" -Force|Out-Null
Set-ItemProperty "$cr\ProgId" "(default)" $prog
New-Item $pr -Force|Out-Null
Set-ItemProperty $pr "(default)" "WordPro Suite Desktop Add-in"
New-Item "$pr\CLSID" -Force|Out-Null
Set-ItemProperty "$pr\CLSID" "(default)" $clsid
$ak="HKCU:\Software\Microsoft\Office\Word\Addins\$prog"
New-Item $ak -Force|Out-Null
New-ItemProperty $ak FriendlyName "WordPro Suite Desktop" -PropertyType String -Force|Out-Null
New-ItemProperty $ak Description "Professional productivity tools for Microsoft Word." -PropertyType String -Force|Out-Null
New-ItemProperty $ak LoadBehavior 3 -PropertyType DWord -Force|Out-Null
New-ItemProperty $ak CommandLineSafe 0 -PropertyType DWord -Force|Out-Null
$r="HKCU:\Software\Microsoft\Office\16.0\Word\Resiliency"
Remove-Item "$r\DisabledItems" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "$r\CrashingAddinList" -Recurse -Force -ErrorAction SilentlyContinue
$t=[Type]::GetTypeFromProgID($prog,$true)
$o=[Activator]::CreateInstance($t)
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($o)|Out-Null
Add-Type -AssemblyName PresentationFramework
[System.Windows.MessageBox]::Show("تم التثبيت بنجاح. افتح Word الآن.","WordPro Suite")|Out-Null
