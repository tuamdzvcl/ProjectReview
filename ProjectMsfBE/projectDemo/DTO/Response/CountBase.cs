namespace projectDemo.DTO.Response
{
    public class CountBase<TStatus> where TStatus : Enum
    {

        public int All { get; set; }
        public Dictionary<TStatus, int> Items { get; set; } = new();
    }

}
