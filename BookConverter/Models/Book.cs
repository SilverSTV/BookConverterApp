using System;
using System.Collections.Generic;
using System.Linq;

namespace BookConverterApp.Models;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = "Неизвестен";
    public DateTime Created { get; set; } = DateTime.Now;

    public List<Chapter> Chapters { get; set; } = new();

    public string GetFullText() => string.Join("\n\n", Chapters.Select(c => c.Content));   
}