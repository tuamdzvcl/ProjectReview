using projectDemo.DTO.Respone;
using System.Net.NetworkInformation;

namespace projectDemo.DTO.Response
{
    public class PageResponseCount<T,TStatus> :PageResponse<T> where TStatus : Enum
    {
        public CountBase<TStatus>? Counts { get; set; }
    }
}
