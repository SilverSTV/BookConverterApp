using CommunityToolkit.Mvvm.ComponentModel;

namespace BookConverterApp.ViewModels;

public partial class ChapterViewModel : ObservableObject
{
    private string _title;

    private string _content;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title,value);
    }

    public string Content
    {
        get => _content;
        set => SetProperty(ref _content,value);
    }
}
