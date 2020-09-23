using System;

namespace Bracketcore.Sket.Responses
{
    /// <summary>
    /// Get Status data response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataResponse<T> :IDisposable
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}