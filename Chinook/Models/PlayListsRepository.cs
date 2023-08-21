using Azure;
using Azure.Core;
using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Composition;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Chinook.Models
{
    public class PlayListsRepository : IPlayListsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ApiService _apiService;
        public PlayListsRepository(ApiService apiService, HttpClient httpClient) {
            _apiService = apiService;
            _httpClient = _apiService.HttpClient;
        }

        public async Task<List<Playlist>> CreatePlaylists(PlaylistTrack playlistTrack, string playListName , string userId)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_httpClient.BaseAddress, $"/api/Albums/{playListName}/{userId}"),
                Content = new StringContent(JsonSerializer.Serialize(playlistTrack), Encoding.UTF8, "application/json")
            };
            try
            {
                using var httpResponse = (await _httpClient.SendAsync(httpRequest)).EnsureSuccessStatusCode();
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                //ToDo: following is hack
                // the http respose content : "responseContent", simplified the object properties
                //As the "<List<Playlist>" type cannot be used to deserielized, instead used a custom one : "List<PlaylistCustom>"
                //And re-used to the return type :<List<Playlist>

                var tempResult = JsonSerializer.Deserialize<List<PlaylistCustom>>(responseContent);
                var list = tempResult?.Select(t => new Playlist()
                {
                    Name = t.name, PlaylistId = t.playlistId,
                }).ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        //Get action
        //Returns "PlayList" data 
        async Task<List<Playlist>> IPlayListsRepository.GetPlayLists(string userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Playlist>>($"/api/Albums/{userId}");
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
