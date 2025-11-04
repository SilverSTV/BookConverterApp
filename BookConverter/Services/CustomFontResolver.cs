using System.IO;
using System.Reflection;
using PdfSharp.Fonts;

namespace BookConverter.Services;

public class CustomFontResolver : IFontResolver
{
    private static readonly string FontPath =
        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets", "Fonts", "times.ttf");

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
        if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
        {
            return new FontResolverInfo("Times New Roman");
        }

        return PlatformFontResolver.ResolveTypeface(familyName, bold, italic);
    }

    public byte[] GetFont(string faceName)
    {
        if (faceName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
        {
            return File.ReadAllBytes(FontPath);
        }

        throw new ArgumentException($"Font '{faceName}' not found");
    }
}
