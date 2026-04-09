using EventTick.Model.Models;

namespace projectDemo.Repository.PaymentRepository
{
    public interface IPaymentRepository
    {
        Task<Payment> Create(Payment entity);
        void Update(Payment entity);
        void Delete(Payment entity);
        Task<List<Payment>> FinAll();
        Task<Payment> FinById(int Id);
        Task<Payment?> FindByOrderId(Guid orderId);
        
    }
}
