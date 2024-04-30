namespace HYAutomation.Core.Web
{
    public class WebApiDataAccess
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool isSuccess { get; set; }
    }
    public class WebApiDataAccess<T> : WebApiDataAccess
    {
        public T data { get; set; }
    }
    public class WebApiDataAccess_Upload<T>
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public T data { get; set; }
    }
}
