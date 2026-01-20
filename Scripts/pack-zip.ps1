Remove-Item ..\QuickLook.Plugin.DuilibPreview.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path ..\Build\Release\ -Exclude *.pdb,*.xml
Compress-Archive $files ..\QuickLook.Plugin.DuilibPreview.zip
Move-Item ..\QuickLook.Plugin.DuilibPreview.zip ..\QuickLook.Plugin.DuilibPreview.qlplugin