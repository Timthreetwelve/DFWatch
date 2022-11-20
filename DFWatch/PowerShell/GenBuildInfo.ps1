﻿Param(
    [Parameter(Mandatory = $true)] [string] $assemblyName,
    [Parameter(Mandatory = $true)] [string] $assemblyVersion,
    [Parameter(Mandatory = $false)] [string] $outputFile="BuildInfo.cs"
)

$nowUTC = (Get-Date).ToUniversalTime()

$commitID = git rev-parse --short HEAD
if ($commitID.Length -lt 1 ) {
    $commitID = "n/a"
}

$commitIDFull = git rev-parse HEAD
if ($commitIDFull.Length -lt 1 ) {
    $commitIDFull = "n/a"
}

$class =
"// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.
//
// This file is generated during the pre-build event by GenBuildInfo.ps1.
// Any edits to this file will be overwritten during the next build!

namespace $assemblyName
{
    /// <summary>Class for information about the current build.</summary>
    public static class BuildInfo
    {
        public const string CommitIDString = `"$commitID`";

        public const string CommitIDFullString = `"$commitIDFull`";

        public const string VersionString = `"$assemblyVersion`";

        public const string BuildDateString = `"$nowUTC`";

        public static readonly DateTime BuildDateUtc = DateTime.SpecifyKind(DateTime.Parse(BuildDateString), DateTimeKind.Utc);

        public static readonly DateTime BuildDateLocal = BuildDateUtc.ToLocalTime();
    }
}"

Set-Content -Path $outputFile -Value $class

$fullName = Get-Item $outputFile
Write-Host "GenBuildInfo: Output written to $fullName"