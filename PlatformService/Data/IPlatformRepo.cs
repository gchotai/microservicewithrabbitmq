using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        Task<bool> SaveChanges();
        Task<IEnumerable<Platform>> GetAllPlatforms();
        Task<Platform> GetPlatformByID(int id);
        void CreatePlatform(Platform platform);
    }
}
