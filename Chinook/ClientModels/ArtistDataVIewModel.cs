using Chinook.Models;
using Chinook.Provider;

namespace Chinook.ClientModels
{
    public class ArtistDataViewModel
    {
        public List<PlaylistTrackViewModel> Tracks { get; set; }
        public ArtistViewModel Artist { get; set; }
    }
}
