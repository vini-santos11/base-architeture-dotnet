using System.Net;

namespace Application.Models
{
    public static class Response
    {
        public static Response<T> Ok<T>(T data, string message = "Operation succeeded")
            => new()
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null,
                StatusCode = (int)HttpStatusCode.OK
            };

        public static Response<T> Created<T>(T data, string message = "Resource created")
            => new()
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null,
                StatusCode = (int)HttpStatusCode.Created
            };

        public static Response<T> Fail<T>(
            string message,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            Dictionary<string, List<string>> errors = null,
            T data = default
        )
            => new()
            {
                Success = false,
                Message = message,
                Errors = errors,
                Data = data,
                StatusCode = (int)statusCode
            };
    }
}