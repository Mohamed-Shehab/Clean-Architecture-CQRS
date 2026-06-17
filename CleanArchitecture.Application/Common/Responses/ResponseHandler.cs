using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Responses
{
    public static class ResponseHandler
    {
        public static Response<T> Success<T>(T? data = default, string message = "Success")
        {
            return new Response<T>
            { 
                StatusCode = 200,
                Succeeded = true,
                Data = data, 
                Message = message
            };
        }

        public static Response<T> SuccessPaged<T>(T data, int pageNumber, int pageSize, int totalCount, string message = "Success")
        {
            return new Response<T>
            {
                StatusCode = 200,
                Succeeded = true,
                Data = data,
                Message = message,
                Meta = new PagedMetaData
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                }
            };
        }

        public static Response<T> Created<T>(T data, string message = "Created")
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

        public static Response<T> NotFound<T>(string message = "Not Found")
        {
            return new Response<T>
            {
                StatusCode = 404,
                Succeeded = false,
                Message = message
            };
        }

        public static Response<T> BadRequest<T>(string message = "Bad Request")
        {
            return new Response<T>
            {
                StatusCode = 400,
                Succeeded = false,
                Message = message
            };
        }
    }
}
