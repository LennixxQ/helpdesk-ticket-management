namespace HelpDesk.Application.Common
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static BaseResponse<T> Ok(T data, string message = "Success") =>
            new() { Success = true, Message = message, Data = data };

        public static BaseResponse<T> Fail(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors ?? new() };

        public static BaseResponse<T> Fail(List<string> errors) =>
            new() { Success = false, Message = "Validation failed.", Errors = errors };
    }

    public class BaseResponse : BaseResponse<object>
    {
        public static BaseResponse Ok(string message = "Success") =>
            new() { Success = true, Message = message };

        public static new BaseResponse Fail(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors ?? new() };
    }
}
