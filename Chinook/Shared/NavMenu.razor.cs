using global::Microsoft.AspNetCore.Components;
using Chinook.ClientModels;
using Chinook.Services;

namespace Chinook.Shared
{
    public partial class NavMenu
    {
        [Inject]
        IPlayListsService PlayListsRepo { get; set; }

        [Inject]
        InvokeSideBarService InvokeNavBar { get; set; }

        private bool CollapseNavMenu = true;
        private List<PlaylistsViewModel> Playlists;
        private string? NavMenuCssClass => CollapseNavMenu ? "collapse" : null;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            InvokeNavBar.changeEvent += (o, args) =>
            {
                _ = LoadData();
                InvokeAsync(StateHasChanged);
            };
        }
        private async Task LoadData()
        {
            Playlists = await PlayListsRepo.GetPlayLists();
        }

        private void ToggleNavMenu()
        {
            CollapseNavMenu = !CollapseNavMenu;
        }
    }
}