namespace CleanArchitecture.Application.Common.Responses
{
    public class Response<T>
    {
        public int StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public object Meta { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }
    }
}
