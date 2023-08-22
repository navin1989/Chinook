using Chinook.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace Chinook.Services
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly IDbContextFactory<ChinookContext> _dbContextFactory;
        private ChinookContext dbContext;
        public AlbumRepository(IDbContextFactory<ChinookContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;            
        }
        public async Task<List<Artist>> GetAlbums()
        {
            dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                return dbContext.Artists.Include(a=>a.Albums).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
