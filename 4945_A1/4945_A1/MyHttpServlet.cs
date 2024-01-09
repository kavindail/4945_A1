using System;
using System.IO;
using System.Threading;

namespace _4945_A1
{
    public class MyHttpServlet : HttpServlet
    {
        private int count = 0;
        public MyHttpServlet(int port) : base(port) {}

        public override void handleGetRequest(HttpProcessor p)
        {
            Console.WriteLine("request: {0}", p.http_url);
            p.writeSuccess();
            p.outputStream.WriteLine("<!DOCTYPE html>");
            p.outputStream.WriteLine("<html>");
            p.outputStream.WriteLine("<head>");
            p.outputStream.WriteLine("<title>Test Server</title>");
            p.outputStream.WriteLine("</head>");
            p.outputStream.WriteLine("<body>");
            p.outputStream.WriteLine("<h1>Test Server</h1>");
            p.outputStream.WriteLine($"<p>URL: {p.http_url}</p>");

            p.outputStream.WriteLine("<form method=\"post\" action=\"/form\">");
            p.outputStream.WriteLine("<input type=\"text\" name=\"data\">");
            p.outputStream.WriteLine("<input type=\"submit\">");
            p.outputStream.WriteLine("</form>");
            p.outputStream.WriteLine("</body>");
            p.outputStream.WriteLine("</html>");
        }


        public override void handlePostRequest(HttpProcessor p, StreamReader inputData)
        {
            
            Console.WriteLine("POST request: {0}", p.http_url);
            string contentLengthHeader = p.httpHeaders.Get("Content-Length");
            Console.WriteLine(contentLengthHeader);
            if (contentLengthHeader != null)
            {
                int contentLength = Convert.ToInt32(contentLengthHeader);
                char[] buffer = new char[contentLength];
                inputData.ReadBlock(buffer, 0, contentLength);
                String data = new String(buffer);
                Console.WriteLine(data);
            }
            else
            {
                p.outputStream.WriteLine("<html><body><h1>Error</h1>");
                p.outputStream.WriteLine("<p>Content-Length header is missing.</p>");
            }
        }
    }
}