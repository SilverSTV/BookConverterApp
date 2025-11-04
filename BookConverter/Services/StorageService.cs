using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using WpfBookConverter.Models;
using System.Linq;


namespace WpfBookConverter.Services
{
    public class StorageService
    {
        private readonly string _filePath;


        public StorageService(string filePath)
        {
            _filePath = filePath;
            EnsureFile();
        }


        private void EnsureFile()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
        }


        public ObservableCollection<Book> Load()
        {
            var json = File.ReadAllText(_filePath);
            var books = JsonSerializer.Deserialize<ObservableCollection<Book>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ObservableCollection<Book>();
            return books;
        }


        public void Save(ObservableCollection<Book> books)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(books, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
