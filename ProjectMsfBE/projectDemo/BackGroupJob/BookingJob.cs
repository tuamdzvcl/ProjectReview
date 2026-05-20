using EventTick.Model.Enum;
using projectDemo.Service.OrderService;

namespace projectDemo.BackGroupJob
{
    public class BookingJob : BackgroundService

    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<GmailExpireJob> _logger;

        public BookingJob( IServiceScopeFactory serviceScopeFactory, ILogger<GmailExpireJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var sevice = scope.ServiceProvider.GetRequiredService<IOrderService>();

                await sevice.ListOrderBackJob();
                

                    await Task.Delay(
                        TimeSpan.FromMinutes(1), stoppingToken);
                
                _logger.LogInformation($"check:");
            }
        }
    }
}
