using global::Microsoft.AspNetCore.Components;
using Chinook.ClientModels;
using Chinook.Services;

namespace Chinook.Pages
{
    public partial class Index
    {
        private List<ArtistsViewModel> Artists;
        private List<ArtistsViewModel> TempArtists;
        private string SearchKeyWord;
        [Inject]
        IAlbumRepository AlbumRepository { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            Artists = await AlbumRepository.GetAlbums(); //Gets the albums using an API service.
            TempArtists = Artists; //Search purpose
        }

        private void FilterByArtist(ChangeEventArgs e)
        {
            Artists = TempArtists;
            if (e.Value is not null)
            {
                Artists = Artists.Where(a => a.Name.Contains(e.Value.ToString())).ToList();
            }
            else
            {
                Artists = TempArtists;
            }
        }
    }
}