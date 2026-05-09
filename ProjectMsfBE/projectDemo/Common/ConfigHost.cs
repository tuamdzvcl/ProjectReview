namespace projectDemo.Common
{
    public static class ConfigHost
    {
        public static string UrlFetest(string url)
        {
            if (url == "loaclhoost")
                return "http://localhost:4200/";
            else if(url =="proc")
            {
                return "https://event-booking.tuananhday.id.vn/";
            }
            return "không có gì cả";
        }
    }
}
