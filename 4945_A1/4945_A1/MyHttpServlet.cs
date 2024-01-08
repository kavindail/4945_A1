using System;
using System.IO;

namespace _4945_A1
{
    public class MyHttpServlet : HttpServlet
    {
        public MyHttpServlet(int port) : base(port) {}

        public override void handleGetRequest(HttpProcessor p)
        {
            Console.WriteLine("request: {0}", p.http_url);
            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
            p.outputStream.WriteLine("url : {0}", p.http_url);
        
            p.outputStream.WriteLine("<form method=post action=/form>");
            p.outputStream.WriteLine("<input type=text name=data>");
            p.outputStream.WriteLine("<input type=submit>");
            p.outputStream.WriteLine("</form>");     
        }


        public override void handlePostRequest(HttpProcessor p, StreamReader inputData)
        {
            
            Console.WriteLine("POST request: {0}", p.http_url);
            string contentLengthHeader = p.httpHeaders.Get("Content-Length");
            if (contentLengthHeader != null)
            {
                
                int contentLength = Convert.ToInt32(contentLengthHeader);
                Console.WriteLine(contentLength);

                String data = inputData.ReadToEnd();
                Console.WriteLine("Data start");
                Console.WriteLine(data);
                Console.WriteLine("Data end");
            }
            else
            {
                p.outputStream.WriteLine("<html><body><h1>Error</h1>");
                p.outputStream.WriteLine("<p>Content-Length header is missing.</p>");
            }
        }
    }
}