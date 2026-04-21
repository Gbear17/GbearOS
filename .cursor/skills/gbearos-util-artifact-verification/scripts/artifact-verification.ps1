#Requires -Version 5.1
<#
.SYNOPSIS
  Deterministic spot-checks on minified PB artifacts under dist/ (see docs/architecture/artifact_verification.md).

.DESCRIPTION
  Uses stable string anchors that survive MDK minification: IGC channel literals, FNV-1a constants,
  envelope wire patterns, and PB2-only ingress (TryParse) markers. Does not diff full source trees.

.PARAMETER RepoRoot
  Repository root containing dist/. If omitted, walks upward from this script's directory until a folder containing dist/ is found.

.EXAMPLE
  pwsh -File .cursor/skills/gbearos-util-artifact-verification/scripts/artifact-verification.ps1
  Exit 0 if all checks pass; non-zero on failure or missing inputs.

.NOTES
  Run after: dotnet build (MDK deploy) and copying outputs to dist/ (gbearos-util-dist-mirror).
#>

[CmdletBinding()]
param(
    [string] $RepoRoot = ''
)

$ErrorActionPreference = 'Stop'
if (-not $RepoRoot) {
    $scriptDir = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
    $p = $scriptDir
    $found = $false
    for ($i = 0; $i -lt 16; $i++) {
        $distDir = Join-Path $p 'dist'
        if (Test-Path -LiteralPath $distDir) {
            $RepoRoot = (Resolve-Path $p).Path
            $found = $true
            break
        }
        $parent = Split-Path -Parent $p
        if (-not $parent -or ($parent -eq $p)) { break }
        $p = $parent
    }
    if (-not $found) {
        Write-Host "FAIL: could not locate repository root (no dist/ directory found walking up from script)." -ForegroundColor Red
        exit 2
    }
}
$failures = New-Object System.Collections.Generic.List[string]

function Add-Failure {
    param([string] $Message)
    [void]$failures.Add($Message)
    Write-Host "  FAIL: $Message" -ForegroundColor Red
}

function Test-Contains {
    param(
        [string] $Label,
        [string] $Haystack,
        [string] $Needle
    )
    if ($Haystack.IndexOf($Needle, [System.StringComparison]::Ordinal) -ge 0) {
        Write-Host "  OK   $Label" -ForegroundColor Green
        return $true
    }
    Add-Failure "$Label - missing substring."
    return $false
}

$null = $failures # appease analyzers; used by Add-Failure

function Test-Regex {
    param(
        [string] $Label,
        [string] $Haystack,
        [string] $Pattern
    )
    if ([regex]::IsMatch($Haystack, $Pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)) {
        Write-Host "  OK   $Label" -ForegroundColor Green
        return $true
    }
    Add-Failure "$Label - missing regex match."
    return $false
}

$distPb1 = Join-Path $RepoRoot 'dist\GbearOS_PB1_Core.cs'
$distPb2 = Join-Path $RepoRoot 'dist\GbearOS_PB2_Display.cs'

Write-Host "artifact-verification.ps1 - repo: $RepoRoot" -ForegroundColor Cyan

if (-not (Test-Path -LiteralPath $distPb1)) {
    Write-Host "FAIL: missing $distPb1 (build and mirror dist/ first)." -ForegroundColor Red
    exit 2
}
if (-not (Test-Path -LiteralPath $distPb2)) {
    Write-Host "FAIL: missing $distPb2 (build and mirror dist/ first)." -ForegroundColor Red
    exit 2
}

$pb1 = [System.IO.File]::ReadAllText($distPb1)
$pb2 = [System.IO.File]::ReadAllText($distPb2)

if ($pb1.Length -lt 1000) { Add-Failure 'PB1 dist file unexpectedly small.' }
if ($pb2.Length -lt 1000) { Add-Failure 'PB2 dist file unexpectedly small.' }

# String needles (avoid nested quote parsing issues in the script body)
$pipeFourSplit = "Split(new[]{'|'},4)"
$protocolLiteral = '="1";'
# Allow minifiers to line-wrap tokens, e.g. "StringComparison.\nOrdinal" or insert other whitespace.
$macOrdinalRegex = 'StringComparison\.\s*Ordinal\)\)\s*\{\s*return\s+false;\s*\}'

# --- IGC channel literals (must match GbearOS_Shared/igc/channels.cs) ---
# PB2 does not embed PB2ToPB1 (PB1 registers the reverse listener only).
$channelsPb1 = @(
    'SYS_STATUS',
    'PB1_WARNINGS',
    'PB1ToPB2_InventorySummary',
    'PB1ToPB2_RefineryStatus',
    'PB1ToPB2_IceStatus',
    'PB1ToPB2_PowerStatus',
    'PB1ToPB2_InventoryDynamic',
    'PB2ToPB1'
)
$channelsPb2 = @(
    'SYS_STATUS',
    'PB1_WARNINGS',
    'PB1ToPB2_InventorySummary',
    'PB1ToPB2_RefineryStatus',
    'PB1ToPB2_IceStatus',
    'PB1ToPB2_PowerStatus',
    'PB1ToPB2_InventoryDynamic'
)

Write-Host ""
Write-Host "[PB1] Channel literals" -ForegroundColor Cyan
foreach ($c in $channelsPb1) {
    [void](Test-Contains "PB1 channel: $c" $pb1 $c)
}

Write-Host ""
Write-Host "[PB2] Channel literals" -ForegroundColor Cyan
foreach ($c in $channelsPb2) {
    [void](Test-Contains "PB2 channel: $c" $pb2 $c)
}

# --- PB1: SenderEnvelope Wrap + FNV fold (TryParse usually stripped) ---
Write-Host ""
Write-Host "[PB1] Envelope / MAC anchors (Wrap path)" -ForegroundColor Cyan
[void](Test-Contains 'PB1 FNV offset basis' $pb1 '2166136261')
[void](Test-Contains 'PB1 FNV prime' $pb1 '16777619')
[void](Test-Contains 'PB1 MAC hex width' $pb1 'ToString("X8")')
[void](Test-Contains 'PB1 Base64 payload' $pb1 'Convert.ToBase64String')
[void](Test-Contains 'PB1 UTF-8 payload bytes' $pb1 'Encoding.UTF8.GetBytes')
[void](Test-Contains 'PB1 FNV fold high-byte XOR' $pb1 '>>8)&0xFF)')
[void](Test-Contains 'PB1 UtcNow.Ticks' $pb1 'UtcNow.Ticks')

if ($pb1.IndexOf($pipeFourSplit, [System.StringComparison]::Ordinal) -ge 0) {
    Add-Failure 'PB1 unexpectedly contains four-part pipe Split - expected Wrap-only SenderEnvelope on PB1.'
} else {
    Write-Host '  OK   PB1 has no four-part envelope Split (TryParse stripped)' -ForegroundColor Green
}

# --- PB2: ingress TryParse + deserialize bodies ---
Write-Host ""
Write-Host "[PB2] Envelope TryParse + protocol" -ForegroundColor Cyan
[void](Test-Contains 'PB2 FNV offset basis' $pb2 '2166136261')
[void](Test-Contains 'PB2 FNV prime' $pb2 '16777619')
[void](Test-Contains 'PB2 four-part Split' $pb2 $pipeFourSplit)
# Anchor may be line-wrapped by MDK minifier (Convert.\nFromBase64String); substring must survive on one logical path.
[void](Test-Contains 'PB2 Base64 decode' $pb2 'FromBase64String')
[void](Test-Contains 'PB2 UTF-8 decode' $pb2 'Encoding.UTF8.GetString')
[void](Test-Contains 'PB2 MAC compare' $pb2 'ToString("X8")')
[void](Test-Contains 'PB2 replay dictionary' $pb2 'Dictionary<string,long>')
[void](Test-Contains 'PB2 protocol version const' $pb2 'private const string')
if ($pb2.IndexOf($protocolLiteral, [System.StringComparison]::Ordinal) -lt 0) {
    Add-Failure 'PB2 - expected protocol version literal pattern (IGCSerializer ProtocolVersion).'
} else {
    Write-Host '  OK   PB2 protocol literal pattern found' -ForegroundColor Green
}

$splitNeedle = ".Split(';')"
$splitSemi = ([regex]::Matches($pb2, [regex]::Escape($splitNeedle))).Count
if ($splitSemi -lt 6) {
    Add-Failure "PB2 - expected multiple DTO Split(';') calls; found $splitSemi (minimum 6)."
} else {
    Write-Host "  OK   PB2 DTO Split(';') count: $splitSemi" -ForegroundColor Green
}

[void](Test-Regex 'PB2 MAC verify Ordinal' $pb2 $macOrdinalRegex)

if ($failures.Count -gt 0) {
    Write-Host ""
    Write-Host "--- $($failures.Count) check(s) failed ---" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "All artifact checks passed." -ForegroundColor Green
exit 0
