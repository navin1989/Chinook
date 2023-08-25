using Chinook.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace Chinook.Provider
{
    public class AlbumProvider : IAlbumProvider
    {
        private readonly ChinookContext _dbContext;
        public AlbumProvider(ChinookContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Artist>> GetAlbums()
        {
            try
            {
                return _dbContext.Artists.Include(a => a.Albums).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
