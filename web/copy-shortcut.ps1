$desktop = [Environment]::GetFolderPath("Desktop")
$src = Get-ChildItem $desktop -Filter "Jamison*Jump Quest*" | Select-Object -First 1
if ($src) {
    $dest = Join-Path $desktop "Jamison's Games"
    if (Test-Path $dest) {
        Copy-Item $src.FullName (Join-Path $dest $src.Name) -Force
        Write-Host "Copied shortcut to Jamison's Games folder"
    } else {
        Write-Host "Jamison's Games folder not found"
    }
} else {
    Write-Host "Jump Quest shortcut not found on desktop"
}
