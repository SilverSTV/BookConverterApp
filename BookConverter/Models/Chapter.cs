namespace WpfBookConverter.Models
{
    public class Chapter
    {
        public string Title { get; set; }
        public string FilePath { get; set; }


// Preview is computed when chapter is selected
        [System.Text.Json.Serialization.JsonIgnore]
        public string Preview { get; set; }
    }
}
