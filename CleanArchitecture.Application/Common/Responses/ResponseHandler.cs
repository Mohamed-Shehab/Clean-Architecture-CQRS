using CleanArchitecture.Application.Common.Models.Querying;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Application.Common.Responses
{
    public static class ResponseHandler
    {
        public static Response<T> Success<T>(T? data = default, 
                                             string message = "Success")
        {
            return new Response<T>
            {
                StatusCode = 200,
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static Response<T> SuccessPaged<T>(T data, 
                                                  PaginationModel pagination, 
                                                  int totalCount, 
                                                  string message = "Success")
        {
            return new Response<T>
            {
                StatusCode = 200,
                Succeeded = true,
                Data = data,
                Message = message,
                Meta = new PagedMetaData
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount
                }
            };
        }

        public static Response<T> Created<T>(T data, 
                                             string message = "Created")
        {
            return new Response<T>
            {
                StatusCode = 201,
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static Response<T> NoContent<T>(string message = "No Content")
        {
            return new Response<T>
            {
                StatusCode = 204,
                Succeeded = true,
                Message = message
            };
        }

        public static Response<T> NotFound<T>(string message = "Not Found",
                                              string? errorCode = null)
        {
            return new Response<T>
            {
                StatusCode = 404,
                Succeeded = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        public static Response<T> BadRequest<T>(string message = "Bad Request",
                                                string? errorCode = null,
                                                List<string>? errors = null, 
                                                T? data = default)
        {
            return new Response<T>
            {
                StatusCode = 400,
                Succeeded = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = errors,
                Data = data
            };
        }

        public static Response<T> Conflict<T>(string message = "Conflict", 
                                              T? data = default, 
                                              string? errorCode = null,
                                              List<string>? errors = null)
        {
            return new Response<T>
            {
                Succeeded = false,
                StatusCode = StatusCodes.Status409Conflict,
                Message = message,
                Data = data,
                ErrorCode = errorCode,
                Errors = errors
            };
        }

        public static Response<T> InternalServerError<T>(string message = "Internal Server Error", 
                                                         List<string>? errors = null)
        {
            return new Response<T>
            {
                Succeeded = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = message,
                Errors = errors
            };
        }
    }
}
