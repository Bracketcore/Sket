namespace Bracketcore.KetAPI.Responses
{
    public class DataResponse<T>
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public T Data { get; set; }

        public DataResponse(string message, string status, T data)
        {
            Message = message;
            Status = status;
            Data = data;
        }
    }
}