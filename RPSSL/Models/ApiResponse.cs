using System.Net;

namespace RPSSL.Models
{
    public abstract class ApiResponseBase
    {
        public bool IsSuccessful { get; protected set; }
    }

    public class BadApiResponse : ApiResponseBase
    {
        public BadApiResponse() 
        {
            Error = new Error((int)HttpStatusCode.InternalServerError, "Something went wrong");
        }

        public BadApiResponse(string message)
        {
            Error = new Error(default, message);
        }

        public BadApiResponse(int code, string message)
        {
            Error = new Error(code, message);
        }

        public Error? Error { get; private set; }
    }

    public class ApiResponse<T> : ApiResponseBase
    {
        private ApiResponse() {}

        public ApiResponse(T? data)
        {
            IsSuccessful = true;
            Data = data;
        }

        public T? Data { get; set; }        
    }
}
