using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4945_A1
{
    public class HttpResponse
    {
        public StreamWriter OutputStream { get; private set; }

        public HttpResponse(TcpClient socket)
        {
            OutputStream = new StreamWriter(socket.GetStream());
        }
        public void WriteSuccess(string contentType = "text/html")
        {
            OutputStream.WriteLine("HTTP/1.1 200 OK");
            OutputStream.WriteLine($"Content-Type: {contentType}");
            OutputStream.WriteLine("Connection: close");
            OutputStream.WriteLine(); 
        }


        public void WriteFailure(int statusCode = 404, string statusDescription = "Not Found")
        {
            OutputStream.WriteLine($"HTTP/1.1 {statusCode} {statusDescription}");
            OutputStream.WriteLine("Connection: close");
            OutputStream.WriteLine(); 
        }

    }
}
