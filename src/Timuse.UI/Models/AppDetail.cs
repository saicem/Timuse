using System.Diagnostics;
using System.Drawing;

namespace Timuse.UI.Models;

public record AppDetail(string Path, FileVersionInfo? VersionInfo, Icon? Icon)
{
    public string Name { get => VersionInfo?.FileDescription ?? VersionInfo?.FileName ?? Path; }
}
