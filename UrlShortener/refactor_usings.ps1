$ErrorActionPreference = "Stop"
$projects = @(
    "src\Core\LinkForge.Domain",
    "src\Core\LinkForge.Application",
    "src\Infrastructure\LinkForge.Infrastructure",
    "src\Infrastructure\LinkForge.Persistence",
    "src\Presentation\LinkForge.API"
)

foreach ($proj in $projects) {
    $usings = @()
    $csFiles = Get-ChildItem -Path $proj -Filter "*.cs" -Recurse | Where-Object { 
        $_.FullName -notmatch "\\obj\\" -and 
        $_.FullName -notmatch "\\bin\\" -and 
        $_.Name -ne "GlobalUsings.cs"
    }

    foreach ($file in $csFiles) {
        $content = Get-Content $file.FullName
        $newContent = @()
        
        foreach ($line in $content) {
            if ($line.Trim() -match "^using\s+[^;(]+;$") {
                $usingLine = $line.Trim()
                if (-not $usingLine.StartsWith("global ")) {
                    $usings += "global " + $usingLine
                } else {
                    $usings += $usingLine
                }
            } else {
                $newContent += $line
            }
        }
        
        # Remove empty lines at the start of the file
        while ($newContent.Count -gt 0 -and [string]::IsNullOrWhiteSpace($newContent[0])) {
            $newContent = $newContent[1..($newContent.Count - 1)]
        }

        Set-Content -Path $file.FullName -Value $newContent
    }

    $uniqueUsings = $usings | Sort-Object -Unique
    if ($uniqueUsings.Count -gt 0) {
        $globalUsingsFile = Join-Path -Path $proj -ChildPath "GlobalUsings.cs"
        if (Test-Path $globalUsingsFile) {
            $existing = Get-Content $globalUsingsFile
            $uniqueUsings = ($uniqueUsings + $existing) | Sort-Object -Unique
        }
        
        # Ensure all existing usings have global prefix just in case
        $finalUsings = @()
        foreach ($u in $uniqueUsings) {
            $t = $u.Trim()
            if (-not $t.StartsWith("global ") -and $t.StartsWith("using ")) {
                $finalUsings += "global " + $t
            } else {
                $finalUsings += $t
            }
        }
        
        Set-Content -Path $globalUsingsFile -Value ($finalUsings | Sort-Object -Unique)
    }
}
Write-Host "Refactoring complete."
