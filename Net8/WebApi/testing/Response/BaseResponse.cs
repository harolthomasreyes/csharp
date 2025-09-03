namespace testing.Response
{
    public class BaseResponse
    {
        public Error Error { get; set; }
    }


    public sealed class Error
    {
        public string Message { get; set; }
    }
}
