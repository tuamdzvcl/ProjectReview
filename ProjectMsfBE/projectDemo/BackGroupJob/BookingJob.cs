using EventTick.Model.Enum;
using projectDemo.Service.OrderService;

namespace projectDemo.BackGroupJob
{
    public class BookingJob : BackgroundService

    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<GmailExpireJob> _logger;
        private readonly IOrderService _orderService;

        public BookingJob(IOrderService orderService, IServiceScopeFactory serviceScopeFactory, ILogger<GmailExpireJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _orderService = orderService;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var order = await _orderService.GetOrder();
                //    order.Data.ForEach(x =>
                //    {
                //        x.Status==
                //    });
                //    if(order.StatusCode == EnumStatusOrder.PENDING) {
                //    }
                //}
                //return Task.CompletedTask;
            }
        }
    }
}
