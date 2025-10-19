using System.Collections.ObjectModel;
using System.Windows.Forms;
using BookConverterApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookConverter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private ObservableCollection<BookViewModel> _books = new();

    private BookViewModel? _selectedBook;

    private ChapterViewModel? _selectedChapter;
public ObservableCollection<BookViewModel> Books
    {
        get => _books;
        set => SetProperty(ref _books,value);
    }

    public BookViewModel? SelectedBook
    {
        get => _selectedBook;
        set => SetProperty(ref _selectedBook, value);
    }

    public ChapterViewModel? SelectedChapter
    {
        get => _selectedChapter;
        set => SetProperty(ref _selectedChapter, value);
    }
    
    public MainViewModel()
    {
        // Для теста
        _books.Add(new BookViewModel("Книга 1"));
        _books.Add(new BookViewModel("Книга 2"));
        
    }

    [RelayCommand]
    public void AddBook()
    {
        _books.Add(new BookViewModel($"Новая книга {Books.Count + 1}"));
    }

    [RelayCommand]
    private void AddChapters()
    {
        if (SelectedBook == null) return;

        var dlg = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Текстовые файлы|*.txt;*.docx"
        };

        if (dlg.ShowDialog() == DialogResult.Yes)
        {
            foreach (var file in dlg.FileNames)
                SelectedBook.AddChapterFromFile(file);
        }
    }
}
