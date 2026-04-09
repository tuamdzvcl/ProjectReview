using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response.Momo;

namespace projectDemo.Service.MomoService
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(MomoRequest request);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);

        Task<string> MomoCallBack(MomoIpnRequest request);   
        bool IsValidMomoIpnSignature(MomoIpnRequest request);
    }
}
