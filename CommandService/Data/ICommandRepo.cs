using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepo
    {
        Task<bool> SaveChanges();
        //Platform
        Task<IEnumerable<Platform>> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        Task<bool> PlatformExists(int platformId);
        Task<bool> ExternalPlatformExists(int externalPlatformId);
        //Command
        Task<IEnumerable<Command>> GetCommandsForPlatform(int PlatformId);
        Task<Command> GetCommand(int PlatformId, int CommandId);
        void CreateCommand(int PlatformId, Command command);

    }
}