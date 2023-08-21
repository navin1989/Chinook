using Chinook.ClientModels;
using Microsoft.AspNetCore.SignalR;

namespace Chinook.Hubs
{
    public class SignalHub : Hub
    {
        public async Task SendSignal(PlaylistTrack track , string listName )
        {
            await Clients.All.SendAsync("ReceivedSignal", track, listName);
        }
    }
}
