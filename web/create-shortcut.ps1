$ws = New-Object -ComObject WScript.Shell
$desktop = $ws.SpecialFolders("Desktop")

# Remove old shortcut if it exists
$oldPath = Join-Path $desktop "Jump Quest.lnk"
if (Test-Path $oldPath) { Remove-Item $oldPath -Force; Write-Host "Removed old shortcut" }

# Create new shortcut
$scPath = Join-Path $desktop "Jamison Gaming's Jump Quest!.lnk"
$sc = $ws.CreateShortcut($scPath)
$sc.TargetPath = "D:\ProjectsHome\JumpQuest\web\launch-jumpquest.bat"
$sc.WorkingDirectory = "D:\ProjectsHome\JumpQuest\web"
$sc.Description = "Jamison Gaming's Jump Quest! - 3D Obby Platformer"
$sc.WindowStyle = 7
$sc.Save()
Write-Host "Shortcut created at: $scPath"
