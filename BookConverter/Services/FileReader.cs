using System.Collections.Generic;
using System.IO;

namespace BookConverterApp.Services;

using Xceed.Words.NET;

public class FileReader
{
    public string ReadFile(string path)
    {
        if (path.EndsWith(".txt"))
            return File.ReadAllText(path);
        if (path.EndsWith(".docx"))
            return DocX.Load(path).Text;

        return string.Empty;
    }

    public IEnumerable<string> ReadFiles(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            yield return ReadFile(path);
        }
    }
}
