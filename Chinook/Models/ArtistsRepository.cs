using Chinook.ClientModels;
using Chinook.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Chinook.Models
{
    public class ArtistsRepository : IArtistsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ApiService apiService;
        public ArtistsRepository(ApiService apiService)
        {
            this.apiService = apiService;
            this._httpClient = apiService.HttpClient;
        }

        //Get action
        //Returns "ArtistData" 
        public async Task<ArtistData> GetArtists(long artistId, string userId)
        {
            return await _httpClient.GetFromJsonAsync<ArtistData>($"/api/Albums/{artistId}/{userId}");
        }

        //Post action
        //Add the track to the favorite list
        public async Task AddFavorite(PlaylistTrack playlistTrack, string userId)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_httpClient.BaseAddress, $"/api/Albums/{userId}"),
                Content = new StringContent(JsonSerializer.Serialize(playlistTrack), Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                using var httpResponse = (await _httpClient.SendAsync(httpRequest)).EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);    
            }
        }

    }
}
