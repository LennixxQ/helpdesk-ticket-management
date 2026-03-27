using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Application.Common
{
    public class BaseResponse<T>
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public IEnumerable<string>? Errors { get; init; }

        public static BaseResponse<T> Ok(T data, string message = "Operation Successful") =>
            new() { Success = true, Message = message, Data = data };

        public static BaseResponse<T> Fail(string message, IEnumerable<string>? errors = null) =>
            new() { Message = message, Errors = errors, Success = false };
    }

    public class BaseResponse
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public IEnumerable<string>? Errors { get; init; }

        public static BaseResponse Ok(string message = "Operation successful.")
            => new() { Success = true, Message = message };

        public static BaseResponse Fail(string message, IEnumerable<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }
}
