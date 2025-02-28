using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private AppDbContext _dbContext;
        public PlatformRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> SaveChanges()
        {
            return await _dbContext.SaveChangesAsync() >= 0;
        }

        public async Task<IEnumerable<Platform>> GetAllPlatforms()
        {
            return await _dbContext.Platforms.ToListAsync();
        }
        public async Task<Platform> GetPlatformByID(int id)
        {
            return await _dbContext.Platforms.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            await _dbContext.Platforms.AddAsync(platform);
        }

    }
}
