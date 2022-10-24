// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

public static class BuildInfo
{
    public const string CommitIDString = "n/a";

    public const string CommitIDFullString = "n/a";

    public const string VersionString = "0.0.0.0";

    public const string BuildDateString = "10/10/2022 10:10:10";

    public static readonly DateTime BuildDateUtc = DateTime.SpecifyKind(DateTime.Parse(BuildDateString), DateTimeKind.Utc);

    public static readonly DateTime BuildDateLocal = BuildDateUtc.ToLocalTime();
}
