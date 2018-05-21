using System;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace ttrssclientgui.requests
{
    public class LoginRequest // : IHttpContent
    {
        /// <summary>
        /// operation
        /// </summary>
        public string op { get { return "login"; } }
        public string user { get; set; }
        public string password { get; set; }

        public IAsyncOperationWithProgress<ulong, ulong> BufferAllAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperationWithProgress<IBuffer, ulong> ReadAsBufferAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperationWithProgress<IInputStream, ulong> ReadAsInputStreamAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperationWithProgress<string, ulong> ReadAsStringAsync()
        {
            throw new NotImplementedException();
        }

        public bool TryComputeLength(out ulong length)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperationWithProgress<ulong, ulong> WriteToStreamAsync(IOutputStream outputStream)
        {
            throw new NotImplementedException();
        }

        // private HttpContentHeaderCollection headers;

        public HttpContentHeaderCollection Headers { get; }
            //= new HttpContentHeaderCollection();
            // => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
