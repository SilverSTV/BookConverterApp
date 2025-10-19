using System.Collections.ObjectModel;
using System.IO;
using BookConverterApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookConverter.ViewModels;

public partial class BookViewModel : ObservableObject
{
    [ObservableProperty]
    private string title;

    private ObservableCollection<ChapterViewModel> _chapters;

    public BookViewModel(string title)
    {
        this.title = title;
    }

    public ObservableCollection<ChapterViewModel> Chapters
    {
        get => _chapters;
        set => SetProperty(ref _chapters,value);
    }

    public void AddChapterFromFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        Chapters.Add(new ChapterViewModel
        {
            Title = Path.GetFileNameWithoutExtension(filePath),
            Content = content
        });
    }
}