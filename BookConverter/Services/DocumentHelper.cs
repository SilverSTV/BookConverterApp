using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;


namespace WpfBookConverter.Services
{
// Simple helper to extract text from docx using Open XML SDK
    public static class DocumentHelper
    {
        public static string ReadDocxAllText(string path)
        {
            if (!File.Exists(path)) return string.Empty;
            using var doc = WordprocessingDocument.Open(path, false);
            var body = doc.MainDocumentPart.Document.Body;
            var sb = new StringBuilder();
            foreach (var para in body.Elements<Paragraph>())
            {
                var texts = para.Descendants<Text>();
                foreach (var t in texts)
                    sb.Append(t.Text);
                sb.AppendLine();
            }
            return sb.ToString();
        }


        public static string ReadTxt(string path)
        {
            if (!File.Exists(path)) return string.Empty;
            return File.ReadAllText(path);
        }
    }
}
