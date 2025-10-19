using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BookConverterApp.Models;

namespace BookConverter.Services;

public class LibraryService
{
    private const string LibraryPath = "Data/library.json";

    public List<Book> Books { get; private set; } = new();

    public LibraryService()
    {
        Load();
    }

    public void AddBook(Book book)
    {
        Books.Add(book);
        Save();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LibraryPath)!);
        File.WriteAllText(LibraryPath, JsonSerializer.Serialize(Books, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    public void Load()
    {
        if (File.Exists(LibraryPath))
        {
            var json = File.ReadAllText(LibraryPath);
            Books = JsonSerializer.Deserialize<List<Book>>(json) ?? new();
        }
    }

    public void AddChapter(Guid bookId, Chapter chapter)
    {
        var book = Books.FirstOrDefault(b => b.Id == bookId);
        if (book == null) return;
        book.Chapters.Add(chapter);
        Save();
    }
}
