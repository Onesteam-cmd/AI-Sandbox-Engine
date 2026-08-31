[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

. (Join-Path $PSScriptRoot 'common.ps1')

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'AI.Sandbox.Engine.slnx'

& (Join-Path $PSScriptRoot 'verify-repository.ps1')

Push-Location $root
try {
    Invoke-CheckedNative -FilePath 'dotnet' -ArgumentList @(
        'restore', $solution,
        '--locked-mode'
    )

    Invoke-CheckedNative -FilePath 'dotnet' -ArgumentList @(
        'build', $solution,
        '--configuration', $Configuration,
        '--no-restore'
    )
}
finally {
    Pop-Location
}
