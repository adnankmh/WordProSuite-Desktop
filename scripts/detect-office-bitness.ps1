foreach($view in @([Microsoft.Win32.RegistryView]::Registry64,[Microsoft.Win32.RegistryView]::Registry32)){
  $base=[Microsoft.Win32.RegistryKey]::OpenBaseKey([Microsoft.Win32.RegistryHive]::LocalMachine,$view)
  $key=$base.OpenSubKey("SOFTWARE\Microsoft\Office\ClickToRun\Configuration")
  if($key){$p=$key.GetValue("Platform");if($p){Write-Output $p;exit 0}}
}
Write-Output "unknown";exit 1
