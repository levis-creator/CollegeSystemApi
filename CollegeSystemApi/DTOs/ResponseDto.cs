using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CollegeSystemApi.DTOs
{
    // Non-generic base class for general response handling
    public class ResponseDto
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public ResponseDto(bool success, int statusCode, string? message = null, object? data = null)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        // Static factory method for success response
        public static ResponseDto SuccessResult(object? data = null, string? message = null, int statusCode = 200)
            => new ResponseDto(true, statusCode, message, data);

        // Static factory method for error response
        public static ResponseDto ErrorResult(int statusCode, string message, object? data = null)
            => new ResponseDto(false, statusCode, message, data);

        public virtual string ToJson() => JsonSerializer.Serialize(this);
    }

    // Generic subclass for strongly-typed responses
    public class ResponseDto<T> : ResponseDto
    {
        // New data and items properties specific to type T
        public new T? Data { get; set; }
        public List<T>? Items { get; set; }

        public ResponseDto(bool success, int statusCode, string? message = null, T? data = default, List<T>? items = null)
            : base(success, statusCode, message, data)
        {
            Data = data;
            Items = items;
        }

        // Factory method for **a single object**
        public static ResponseDto<T> SuccessResultForData(T data, string? message = "Success", int statusCode = 200)
            => new ResponseDto<T>(true, statusCode, message, data);

        // Factory method for **a list of objects**
        public static ResponseDto<T> SuccessResultForList(List<T> items, string? message = "Success", int statusCode = 200)
            => new ResponseDto<T>(true, statusCode, message, default, items);

        // Error result for a single object
        public static ResponseDto<T> ErrorResultForData(int statusCode, string message, T? data = default)
            => new ResponseDto<T>(false, statusCode, message, data);

        // Error result for a list of objects
        public static ResponseDto<T> ErrorResultForList(int statusCode, string message, List<T>? items = null)
            => new ResponseDto<T>(false, statusCode, message, default, items);

        public override string ToJson() => JsonSerializer.Serialize(this);
    }
}
