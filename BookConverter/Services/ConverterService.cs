using System.Collections.ObjectModel;
using WpfBookConverter.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;


namespace WpfBookConverter.Services
{
    public class ConverterService
    {
// Простой конвертер в PDF и FB2.
// Для production-качества нужно использовать более продвинутые библиотеки.


        public async Task ConvertToPdfAsync(Book book, string outputPath)
        {
// Very simple implementation using PdfSharp: each chapter append text.
            var doc = new PdfDocument();
            doc.Info.Title = book.Title;


            for (var index = book.Chapters.Count-1; index >= 0 ; index--)
            {
                var ch = book.Chapters[index];
                var text = await ReadChapterTextAsync(ch);
                var page = doc.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Times New Roman", 12);


                var y = 20.0;
                gfx.DrawString(ch.Title, new XFont("Times New Roman", 16, XFontStyleEx.Bold), XBrushes.Black,
                    new XRect(40, y, page.Width - 80, page.Height - 40), new XStringFormat());
                y += 30;


                var lines = SplitLines(text, 90);
                foreach (var line in lines)
                {
                    gfx.DrawString(line, font, XBrushes.Black, new XRect(40, y, page.Width - 80, page.Height - 40),
                        new XStringFormat());
                    y += 14;
                    if (y > page.Height - 40)
                    {
// next page
                        page = doc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 20;
                    }
                }
            }


            doc.Save(outputPath);
        }


        public async Task ConvertToFb2Async(Book book, string outputPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine(
                "<FictionBook xmlns=\"http://www.gribuser.ru/xml/fictionbook/2.0\">\n<header>\n<title-info>\n<genre>other</genre>");
            sb.AppendLine($"<book-title>{EscapeXml(book.Title)}</book-title>");
            sb.AppendLine("</title-info>\n</header>\n<body>");


            foreach (var ch in book.Chapters)
            {
                var text = await ReadChapterTextAsync(ch);
                sb.AppendLine("<section>");
                sb.AppendLine($"<title><p>{EscapeXml(ch.Title)}</p></title>");
                var paragraphs = text.Split(new[] {"\r\n", "\n"}, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in paragraphs)
                {
                    sb.AppendLine($"<p>{EscapeXml(p)}</p>");
                }

                sb.AppendLine("</section>");
            }


            sb.AppendLine("</body>\n</FictionBook>");
            await File.WriteAllTextAsync(outputPath, sb.ToString(), Encoding.UTF8);
        }


        private static string EscapeXml(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return System.Security.SecurityElement.Escape(s);
        }


        private async Task<string> ReadChapterTextAsync(Chapter ch)
        {
            var path = ch.FilePath ?? string.Empty;
            if (!File.Exists(path)) return string.Empty;
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".txt") return await Task.FromResult(Services.DocumentHelper.ReadTxt(path));
            if (ext == ".docx") return await Task.FromResult(Services.DocumentHelper.ReadDocxAllText(path));
            return string.Empty;
        }


        private string[] SplitLines(string text, int maxChars)
        {
            if (string.IsNullOrEmpty(text)) return new string[0];
            var words = text.Split(new[] {' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var lines = new System.Collections.Generic.List<string>();
            var cur = new StringBuilder();
            foreach (var w in words)
            {
                if (cur.Length + w.Length + 1 > maxChars)
                {
                    lines.Add(cur.ToString());
                    cur.Clear();
                }

                if (cur.Length > 0) cur.Append(' ');
                cur.Append(w);
            }

            if (cur.Length > 0) lines.Add(cur.ToString());
            return lines.ToArray();
        }
    }
}
