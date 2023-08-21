using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace Chinook.Models
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ApiService apiService;
        public AlbumRepository(ApiService apiService)
        {
            this.apiService = apiService;
            this._httpClient = apiService.HttpClient;
        }
        public async Task<List<Artist>> GetAlbums()
        {
            return await _httpClient.GetFromJsonAsync<List<Artist>>("/api/Albums");
        }
    }
}
