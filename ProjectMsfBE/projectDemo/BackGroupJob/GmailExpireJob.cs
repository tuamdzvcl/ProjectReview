using Microsoft.EntityFrameworkCore;
using projectDemo.Data;

namespace projectDemo.BackGroupJob
{
    public class GmailExpireJob : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<GmailExpireJob> _logger;

        public GmailExpireJob(IServiceScopeFactory serviceScopeFactory, ILogger<GmailExpireJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<EventTickDbContext>();

                var now = DateTime.Now;
                var expireDate = now.AddDays(-7);


                var user = await db.User.Where(x=>x.IsActive==false &&  x.CreatedDate<expireDate)
                    .OrderBy(x=>x.CreatedDate)
                    .Take(100)
                    .ToListAsync(stoppingToken);

                if (user.Any())
                {
                    db.User.RemoveRange(user);

                    await db.SaveChangesAsync(stoppingToken);
                }
                _logger.LogInformation($"Deleted {user.Count} in Table User {expireDate}-{user.Select(x=>x.CreatedDate).ToList()}" );
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken);

            }
            
        }
    }
}
