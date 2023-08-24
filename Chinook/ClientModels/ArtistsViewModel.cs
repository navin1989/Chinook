namespace Chinook.ClientModels
{
    public class ArtistsViewModel
    {
        public long ArtistId { get; set; }
        public string? Name { get; set; }
        public int AlbumsCount { get; set; } = 0;
    }
}
