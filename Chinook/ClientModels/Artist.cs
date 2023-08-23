namespace Chinook.ClientModels
{
    public class Artist
    {
        public long ArtistId { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<Album> Albums { get; set; }
    }
}
