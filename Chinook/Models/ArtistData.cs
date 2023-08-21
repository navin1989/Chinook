using Chinook.ClientModels;

namespace Chinook.Models
{
    public class ArtistData
    {
        public List<PlaylistTrack> Tracks { get; set; }
        public Artist Artist { get; set; }
    }
}
