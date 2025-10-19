using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace BookConverterApp.Services;

public class ConverterService
{
    public string Convert(string text, string format)
    {
        string output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            $"Book_{DateTime.Now:yyyyMMdd_HHmm}.{format.ToLower()}");

        if (format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Content().Text(text);
                });
            }).GeneratePdf(output);
        }
        else if (format.Equals("FB2", StringComparison.OrdinalIgnoreCase))
        {
            File.WriteAllText(output, BuildFb2(text));
        }

        return output;
    }

    private string BuildFb2(string content)
    {
        return $"""
        <?xml version="1.0" encoding="utf-8"?>
        <FictionBook xmlns="http://www.gribuser.ru/xml/fictionbook/2.0">
          <body>
            <section>
              <p>{System.Security.SecurityElement.Escape(content)}</p>
            </section>
          </body>
        </FictionBook>
        """;
    }
}
