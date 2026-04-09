using System.Text.Json.Serialization;

namespace projectDemo.DTO.Request
{
    public class MomoIpnRequest
    {
        [JsonPropertyName("partnerCode")]
        public string PartnerCode { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("orderInfo")]
        public string OrderInfo { get; set; }

        [JsonPropertyName("orderType")]
        public string OrderType { get; set; }

        [JsonPropertyName("transId")]
        public string TransId { get; set; }

        [JsonPropertyName("resultCode")]
        public int ResultCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("payType")]
        public string? PayType { get; set; } = "Momo";

        [JsonPropertyName("responseTime")]
        public long ResponseTime { get; set; }

        [JsonPropertyName("extraData")]
        public string? ExtraData { get; set; } = "test";

        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }
}
