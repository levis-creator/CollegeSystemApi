namespace CollegeSystemApi.DTOs.Response
{
    public class ResponseDtoData<T>(bool success, int statusCode, string message, T data)
        : ResponseBase(success, statusCode, message)
    {

        public T? Data { get; set; } = data;

        public static ResponseDtoData<T> SuccessResult(T? data, string? message = "Success", int statusCode = 200) =>
            new ResponseDtoData<T>(true, statusCode, message!, data!);

        public static ResponseDtoData<T> ErrorResult(  int statusCode, string? message, T? data = default) =>
            new ResponseDtoData<T>(false, statusCode, message!, data!);


    }
}
