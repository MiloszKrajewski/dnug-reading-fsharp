Add-Type -AssemblyName System.IO.Compression.FileSystem

function Unzip([string] $zipfile, [string] $outpath) {
    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

function Download([string] $url, [string] $path) {
    $temp = [System.IO.Path]::GetTempFileName()
    Invoke-WebRequest "$url" -OutFile "$temp"
    Unzip "$temp" "$path"
    Remove-Item "$temp"
}

Push-Location $PSScriptRoot

try {
    if (-not (Test-Path "jupyter")) {
        & py -3 -m virtualenv jupyter
        & ./jupyter/scripts/pip install jupyter
    }

    if (-not (Test-Path "ifsharp")) {
        $url = "https://github.com/fsprojects/IfSharp/releases/download/v3.0.0-beta2/IfSharp.v3.0.0-beta2.zip"
        Download "$url" "./ifsharp"
        & "./ifsharp/ifsharp" "--install"
    }

    & ./jupyter/scripts/jupyter-notebook
}
finally {
    Pop-Location
}
