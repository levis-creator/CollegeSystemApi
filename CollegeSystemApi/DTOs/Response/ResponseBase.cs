namespace CollegeSystemApi.DTOs.Response
{
    public class ResponseBase(bool success, int statusCode, string message)
    {
        public bool Success { get; set; } = success;
        public int StatusCode { get; set; } = statusCode;
        public string Message { get; set; } = message;
    }
}
