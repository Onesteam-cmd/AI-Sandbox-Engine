[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

. (Join-Path $PSScriptRoot 'common.ps1')

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'AI.Sandbox.Engine.slnx'

& (Join-Path $PSScriptRoot 'build.ps1') -Configuration $Configuration

Push-Location $root
try {
    Invoke-CheckedNative -FilePath 'dotnet' -ArgumentList @(
        'test', $solution,
        '--configuration', $Configuration,
        '--no-build',
        '--no-restore'
    )
}
finally {
    Pop-Location
}
