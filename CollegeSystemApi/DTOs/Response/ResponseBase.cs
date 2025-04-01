namespace CollegeSystemApi.DTOs.Response
{
    public class ResponseBase
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public ResponseBase(bool success, int statusCode, string message)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
        }
      
      
    }
}
