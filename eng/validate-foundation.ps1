[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [ValidateRange(2, 1000000)]
    [int]$TickCount = 5000,

    [ValidateRange(1, 10)]
    [int]$RepeatCount = 3,

    [ValidateRange(1000, 300000)]
    [long]$MaxMilliseconds = 30000,

    [switch]$NoBuild
)

. (Join-Path $PSScriptRoot 'common.ps1')

$root = Split-Path -Parent $PSScriptRoot
$testProject = Join-Path $root 'tests\AI.Sandbox.Engine.Core.Tests\AI.Sandbox.Engine.Core.Tests.csproj'
$probeProject = Join-Path $root 'samples\AI.Sandbox.Engine.FoundationProbe\AI.Sandbox.Engine.FoundationProbe.csproj'

& (Join-Path $PSScriptRoot 'verify-repository.ps1')

if (-not $NoBuild) {
    & (Join-Path $PSScriptRoot 'build.ps1') -Configuration $Configuration
}

Push-Location $root
try {
    Invoke-CheckedNative -FilePath 'dotnet' -ArgumentList @(
        'test',
        $testProject,
        '--configuration',
        $Configuration,
        '--no-build',
        '--no-restore',
        '--filter',
        'FullyQualifiedName~FoundationValidationTests'
    )

    $expectedChecksum = $null

    for ($iteration = 1; $iteration -le $RepeatCount; $iteration++) {
        Write-Host "`nFoundation probe iteration $iteration/$RepeatCount" `
            -ForegroundColor Cyan

        $output = @(
            & dotnet run `
                --project $probeProject `
                --configuration $Configuration `
                --no-build `
                -- $TickCount 2>&1
        )
        $exitCode = $LASTEXITCODE
        $output | Out-Host

        if ($exitCode -ne 0) {
            throw "Foundation Probe failed with exit code $exitCode."
        }

        $checksumLines = @(
            $output |
                ForEach-Object { $_.ToString() } |
                Where-Object { $_ -match '^checksum=[0-9a-f]{64}$' }
        )
        $elapsedLines = @(
            $output |
                ForEach-Object { $_.ToString() } |
                Where-Object { $_ -match '^elapsed_ms=\d+$' }
        )

        if ($checksumLines.Count -ne 1) {
            throw 'Foundation Probe must emit exactly one canonical checksum.'
        }

        if ($elapsedLines.Count -ne 1) {
            throw 'Foundation Probe must emit exactly one elapsed_ms value.'
        }

        $checksumLine = $checksumLines[0]
        $elapsedLine = $elapsedLines[0]
        $checksum = $checksumLine.Substring(('checksum='.Length))
        $elapsedMilliseconds =
            [long]$elapsedLine.Substring(('elapsed_ms='.Length))

        if ($null -eq $expectedChecksum) {
            $expectedChecksum = $checksum
        }
        elseif ($checksum -ne $expectedChecksum) {
            throw "Foundation Probe checksum changed between repetitions: '$expectedChecksum' vs '$checksum'."
        }

        if ($elapsedMilliseconds -gt $MaxMilliseconds) {
            throw "Foundation Probe exceeded the baseline budget: ${elapsedMilliseconds} ms > ${MaxMilliseconds} ms."
        }
    }

    Write-Host "`nFoundation validation passed." -ForegroundColor Green
    Write-Host "Ticks per probe: $TickCount" -ForegroundColor Green
    Write-Host "Probe repetitions: $RepeatCount" -ForegroundColor Green
    Write-Host "Stable checksum: $expectedChecksum" -ForegroundColor Green
    Write-Host "Maximum allowed time: ${MaxMilliseconds} ms" -ForegroundColor Green
}
finally {
    Pop-Location
}
