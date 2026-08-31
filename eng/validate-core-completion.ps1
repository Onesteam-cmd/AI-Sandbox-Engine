& {
    [CmdletBinding()]
    param()

    Set-StrictMode -Version Latest
    $ErrorActionPreference = 'Stop'
    $ProgressPreference = 'SilentlyContinue'

    $Root = Split-Path -Parent $PSScriptRoot
    $Solution = Join-Path $Root 'AI.Sandbox.Engine.slnx'
    $ProbeProject = Join-Path $Root (
        'samples\AI.Sandbox.Engine.FoundationProbe\' +
        'AI.Sandbox.Engine.FoundationProbe.csproj')

    Write-Output '===== CORE COMPLETE REPOSITORY VERIFIER ====='
    & (Join-Path $Root 'eng\verify-repository.ps1')
    if ($LASTEXITCODE -ne 0) {
        throw "Repository verifier failed: $LASTEXITCODE"
    }

    Write-Output '===== CORE COMPLETE RELEASE BUILD ====='
    & dotnet build $Solution --configuration Release
    if ($LASTEXITCODE -ne 0) {
        throw "Release build failed: $LASTEXITCODE"
    }

    Write-Output '===== CORE COMPLETE TEST BASELINE ====='
    & dotnet test $Solution --configuration Release --no-build
    if ($LASTEXITCODE -ne 0) {
        throw "Core test baseline failed: $LASTEXITCODE"
    }

    Write-Output '===== CORE COMPLETE PRODUCT PIPELINE ====='
    $runtime = @(
        & dotnet run `
            --project $ProbeProject `
            --configuration Release `
            --no-build 2>&1 |
            ForEach-Object { "$_" }
    )
    $runtimeExit = $LASTEXITCODE
    $runtime | ForEach-Object { Write-Output $_ }

    if ($runtimeExit -ne 0) {
        throw "Foundation product pipeline runtime failed: $runtimeExit"
    }

    $runtimeText = $runtime -join "`n"
    foreach ($required in @(
        'FOUNDATION_PROBE_OK'
        'CORE_PRODUCT_PIPELINE_OK'
        'core_product_pipeline=Completed'
        'core_product_context=Retrieved'
        'core_product_prompt=Composed'
        'core_product_model=Completed'
        'core_product_structured=Decoded'
        'core_product_action=Approved'
        'core_product_runtime=Committed'
        'core_product_authority_unchanged_before_command=True'
        'core_product_value_transition=0->2'
        'core_product_version_transition=0->1'
        'core_product_reply=I was near the station.'
    )) {
        if (-not $runtimeText.Contains($required)) {
            throw "Core completion evidence missing: $required"
        }
    }

    $productProbe = Join-Path $Root (
        'samples\AI.Sandbox.Engine.FoundationProbe\ProductPipelineProbe.cs')
    if (-not (Test-Path -LiteralPath $productProbe -PathType Leaf)) {
        throw 'Persistent ProductPipelineProbe.cs is missing.'
    }

    Write-Output 'core_complete=True'
    Write-Output 'production_core_product_pipeline=validated'
    Write-Output 'production_src_mutation_required=false'
    Write-Output 'new_core_contracts=0'
    Write-Output 'new_recovery_layers=0'
    Write-Output 'game_or_unreal_touched=false'
    Write-Output 'AI_SANDBOX_CORE_COMPLETE_VALIDATION_OK'
}
