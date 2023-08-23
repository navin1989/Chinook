using AutoMapper;
using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace Chinook.Services
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly ChinookContext _dbContext;
        private readonly IMapper _mapper;
        public AlbumRepository(ChinookContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<List<Artist>> GetAlbums()
        {
            try
            {
                return _dbContext.Artists.Include(a=>a.Albums)
                    .Select(a=>_mapper.Map<Artist>(a)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
