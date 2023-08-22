using System;
using System.Collections.Generic;

namespace Chinook.Models
{
    public partial class Playlist
    {
        public Playlist()
        {
            Tracks = new HashSet<Track>();
        }

        public long PlaylistId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
        public virtual ICollection<UserPlaylist> UserPlaylists { get; set; }

    }

    public partial class PlaylistCustom
    {

        public long playlistId { get; set; }
        public string? name { get; set; }

    }
}
