using System.Collections.ObjectModel;
using System.Text.Json.Serialization;


namespace WpfBookConverter.Models
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<Chapter> Chapters { get; set; } = new ObservableCollection<Chapter>();
    }
}
