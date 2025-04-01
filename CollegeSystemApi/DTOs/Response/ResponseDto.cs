using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CollegeSystemApi.DTOs.Response
{
    /// <summary>
    /// Represents a standard response DTO for API responses.
    /// </summary>
    public class ResponseDto
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The HTTP status code associated with the response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// A message providing additional information about the response.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The response data (if any).
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseDto"/> class.
        /// </summary>
        public ResponseDto(bool success, int statusCode, string? message = null, object? data = null)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        public static ResponseDto SuccessResult(object? data = null, string? message = "Success", int statusCode = 200)
            => new ResponseDto(true, statusCode, message, data);

        /// <summary>
        /// Creates an error response.
        /// </summary>
        public static ResponseDto ErrorResult(int statusCode, string message, object? data = null)
            => new ResponseDto(false, statusCode, message, data);

        /// <summary>
        /// Converts the response DTO to a JSON string.
        /// </summary>
        public virtual string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
    }

    /// <summary>
    /// Represents a generic response DTO for strongly-typed API responses.
    /// </summary>
    public class ResponseDto<T> : ResponseDto
    {
        /// <summary>
        /// The strongly-typed response data (if applicable).
        /// </summary>
        public T? TypedData { get; set; }

        /// <summary>
        /// A collection of items of type <typeparamref name="T"/>.
        /// </summary>
        public List<T>? Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseDto{T}"/> class.
        /// </summary>
        public ResponseDto(bool success, int statusCode, string? message = null, T? data = default, List<T>? items = null)
            : base(success, statusCode, message, null) // Base class Data is null to avoid duplication
        {
            TypedData = data;
            Items = items;
        }

        /// <summary>
        /// Creates a success response with a single object.
        /// </summary>
        public static ResponseDto<T> SuccessResultForData(T data, string? message = "Success", int statusCode = 200)
            => new ResponseDto<T>(true, statusCode, message, data);

        /// <summary>
        /// Creates a success response with a list of objects.
        /// </summary>
        public static ResponseDto<T> SuccessResultForList(List<T> items, string? message = "Success", int statusCode = 200)
            => new ResponseDto<T>(true, statusCode, message, default, items);

        /// <summary>
        /// Creates an error response with a single object.
        /// </summary>
        public static ResponseDto<T> ErrorResultForData(int statusCode, string message, T? data = default)
            => new ResponseDto<T>(false, statusCode, message, data);

        /// <summary>
        /// Creates an error response with a list of objects.
        /// </summary>
        public static ResponseDto<T> ErrorResultForList(int statusCode, string message, List<T>? items = null)
            => new ResponseDto<T>(false, statusCode, message, default, items);

        /// <summary>
        /// Converts the generic response DTO to a JSON string.
        /// </summary>
        public override string ToJson() => JsonSerializer.Serialize(new
        {
            Success,
            StatusCode,
            Message,
            Data = TypedData, // Ensuring correct serialization
            Items
        }, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });

        public static ResponseDto<T> FromResponseDto(ResponseDto response)
        {
            if (response is ResponseDto<T> genericResponse)
            {
                return genericResponse;
            }

            if (response.Data is T data)
            {
                return new ResponseDto<T>(response.Success, response.StatusCode, response.Message, data);
            }
            else
            {
                throw new InvalidCastException(
                    $"Unable to cast Data property of type {response.Data?.GetType().FullName ?? "null"} " +
                    $"to type {typeof(T).FullName}");
            }
        }
    }
}
