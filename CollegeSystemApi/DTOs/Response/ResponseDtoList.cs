namespace CollegeSystemApi.DTOs.Response;

public class ResponseDtoList<T> : ResponseBase
{
    public T? Data { get; set; }  // Ensure it's not nullable

    public ResponseDtoList(bool success, int statusCode, string message, T data)
        : base(success, statusCode, message)
    {
        Data = data; // Prevent null assignments
    }

    public static ResponseDtoList<T> SuccessResult(T? data, string message = "Success", int statusCode = 200) =>
        new ResponseDtoList<T>(true, statusCode, message, data!);

    public static ResponseDtoList<T> ErrorResult(int statusCode, string message, T? data) =>
        new ResponseDtoList<T>(false, statusCode, message, data!);
}