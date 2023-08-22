using Chinook.Models;

namespace Chinook.ClientModels
{
    public class ArtistData
    {
        public List<PlaylistTrack> Tracks { get; set; }
        public Artist Artist { get; set; }
    }
}
