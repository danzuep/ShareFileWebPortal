using System.Net;
using ShareFile.Api.Helpers.Models;

namespace ShareFileWebPortal.Models;

public record AppsettingsOptions
{
    public ConfigOptions Config { get; set; }
    public ExtendedShareFileApiOptions ShareFile { get; set; }
    public CredentialsOptions Credentials { get; set; }
    public ConnectionStringsOptions ConnectionStrings { get; set; }
}

public record ConfigOptions
{
    public const string Name = "Config";

    public string ProgramName { get; set; }
    public string DownloadPath { get; set; }
}

public class ExtendedShareFileApiOptions : ShareFileApiOptions
{
    public const string Name = "ShareFile";

    public ShareFileFolderOptions Folder { get; set; }
}

public record ShareFileFolderOptions
{
    public const string Name = "Folder";

    public int DepthLimit { get; set; } = -1;
    public string ToProcess { get; set; } = "allshared";
    public IList<string>? ToSkip { get; set; }
}

public record ConnectionStringsOptions
{
    public const string Name = "ConnectionStrings";

    public string Default { get; set; }
    public string ShareFileDb { get; set; }
    public string ShareFileWebPortal { get; set; }
}

public record CredentialsOptions
{
    public const string Name = "Credentials";

    public NetworkCredential? Api { get; set; }
}