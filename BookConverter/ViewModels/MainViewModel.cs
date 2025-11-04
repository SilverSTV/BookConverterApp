using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfBookConverter.Models;
using WpfBookConverter.Services;
using System.Linq;
using System;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using WpfBookConverter.Views;


namespace WpfBookConverter.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly StorageService _storage;
        private readonly ConverterService _converter;
        private const string StorageFile = "books.json";


        public ObservableCollection<Book> Books { get; set; }


        private Book _selectedBook;

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
                SelectedChapter = null;
            }
        }


        private Chapter _selectedChapter;

        public Chapter SelectedChapter
        {
            get => _selectedChapter;
            set
            {
                _selectedChapter = value;
                OnPropertyChanged();
                LoadPreviewForSelectedChapter();
            }
        }


        public ICommand AddBookCommand { get; }
        public ICommand RemoveBookCommand { get; }
        public ICommand ConvertBookCommand { get; }
        public ICommand AddChapterCommand { get; }
        public ICommand RemoveChapterCommand { get; }


        public MainWindowViewModel()
        {
            _storage = new StorageService(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WpfBookConverter",
                "books.json"));
            _converter = new ConverterService();


            Books = _storage.Load();


            AddBookCommand = new RelayCommand(_ => AddBook());
            RemoveBookCommand = new RelayCommand(_ => RemoveBook(), _ => SelectedBook != null);
            ConvertBookCommand = new RelayCommand(async _ => await ConvertBookAsync(),
                _ => SelectedBook != null && SelectedBook.Chapters.Any());
            AddChapterCommand = new RelayCommand(_ => AddChapter());
            RemoveChapterCommand = new RelayCommand(_ => RemoveChapter(), _ => SelectedChapter != null);
        }

        private void AddBook()
        {
// simple Input dialog
            var dlg = new InputDialog("Название книги:");
            if (dlg.ShowDialog() == true)
            {
                var book = new Book {Id = Guid.NewGuid().ToString(), Title = dlg.Value};
                Books.Add(book);
                _storage.Save(Books);
            }
        }


        private void RemoveBook()
        {
            if (SelectedBook == null) return;
            Books.Remove(SelectedBook);
            SelectedBook = null;
            _storage.Save(Books);
        }


        private void AddChapter()
        {
            if (SelectedBook == null) return;
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Text and Word|*.txt;*.docx|All files|*.*";
            if (ofd.ShowDialog() == true)
            {
                AddChaptersFromPaths(ofd.FileNames);
            }
        }

        public void AddChaptersFromPaths(string[] paths)
        {
            if (SelectedBook == null) return;

            var sortedChapters = SortByNumbers(paths);  
            
            foreach (var p in sortedChapters)
            {
                var title = Path.GetFileNameWithoutExtension(p);
                SelectedBook.Chapters.Add(new Chapter { Title = title, FilePath = p });
            }
            _storage.Save(Books);
            OnPropertyChanged(nameof(SelectedBook));
        }

        private void RemoveChapter()
        {
            if (SelectedBook == null || SelectedChapter == null) return;
            SelectedBook.Chapters.Remove(SelectedChapter);
            SelectedChapter = null;
            _storage.Save(Books);
        }


        private async Task ConvertBookAsync()
        {
            var dlg = new FormatDialog();
            if (dlg.ShowDialog() != true) return;


            var sfd = new SaveFileDialog();
            if (dlg.SelectedFormat == OutputFormat.PDF)
            {
                sfd.Filter = "PDF file|*.pdf";
                sfd.FileName = SelectedBook.Title + ".pdf";
            }
            else
            {
                sfd.Filter = "FB2 file|*.fb2";
                sfd.FileName = SelectedBook.Title + ".fb2";
            }


            if (sfd.ShowDialog() != true) return;


            try
            {
                if (dlg.SelectedFormat == OutputFormat.PDF)
                    await _converter.ConvertToPdfAsync(SelectedBook, sfd.FileName);
                else
                    await _converter.ConvertToFb2Async(SelectedBook, sfd.FileName);


                System.Windows.MessageBox.Show("Конвертация завершена", "Готово", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }


        private void LoadPreviewForSelectedChapter()
        {
            if (SelectedChapter == null) return;
            try
            {
                var path = SelectedChapter.FilePath;
                if (!File.Exists(path))
                {
                    SelectedChapter.Preview = "Файл не найден";
                    OnPropertyChanged(nameof(SelectedChapter));
                    return;
                }

                var ext = Path.GetExtension(path).ToLowerInvariant();
                string text = string.Empty;
                if (ext == ".txt") text = DocumentHelper.ReadTxt(path);
                else if (ext == ".docx") text = DocumentHelper.ReadDocxAllText(path);


                if (string.IsNullOrEmpty(text)) SelectedChapter.Preview = "Файл пустой или не удалось прочитать";
                else
                {
                    var preview = text.Length > 2000 ? text.Substring(0, 2000) + "..." : text;
                    SelectedChapter.Preview = preview;
                }


                OnPropertyChanged(nameof(SelectedChapter));
            }
            catch (Exception ex)
            {
                SelectedChapter.Preview = "Ошибка при чтении: " + ex.Message;
                OnPropertyChanged(nameof(SelectedChapter));
            }
        }
        
        private List<string> SortByNumbers(IEnumerable<string> input)
        {
            // Регулярка находит первое число в строке
            var regex = new Regex(@"\d+");

            return input
                .Select(s => new
                {
                    Text = s,
                    Number = regex.Match(s) is Match m && m.Success ? int.Parse(m.Value) : int.MaxValue
                })
                .OrderBy(x => x.Number)
                .ThenBy(x => x.Text)
                .Select(x => x.Text)
                .ToList();
        }


// Simple dialogs are implemented below as classes


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
