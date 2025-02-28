using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _dbContext;
        public CommandRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> SaveChanges()
        {
            return await _dbContext.SaveChangesAsync() >= 0;
        }
        //Platform
        public async Task<IEnumerable<Platform>> GetAllPlatforms()
        {
            return await _dbContext.Platforms.ToListAsync();
        }
        public async void CreatePlatform(Platform plat)
        {
            if (plat == null)
                throw new ArgumentNullException(nameof(plat));
            await _dbContext.Platforms.AddAsync(plat);
        }
        public async Task<bool> PlatformExists(int platformId)
        {
            return await _dbContext.Platforms.AnyAsync(p => p.Id == platformId);
        }

        public async Task<bool> ExternalPlatformExists(int externalPlatformId)
        {
            return await _dbContext.Platforms.AnyAsync(p => p.ExternalID == externalPlatformId);
        }
        //Command
        public Task<IEnumerable<Command>> GetCommandsForPlatform(int PlatformId)
        {
            return Task.FromResult<IEnumerable<Command>>(_dbContext.Commands.Where(c => c.PlatformId == PlatformId));
        }
        public async Task<Command> GetCommand(int PlatformId, int CommandId)
        {
            return await _dbContext.Commands.Where(c => c.PlatformId == PlatformId && c.Id == CommandId).FirstOrDefaultAsync();
        }
        public async void CreateCommand(int PlatformId, Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            command.PlatformId = PlatformId;
            await _dbContext.Commands.AddAsync(command);
        }
    }
}