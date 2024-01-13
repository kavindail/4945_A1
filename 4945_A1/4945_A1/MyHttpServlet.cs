using System;
using System.IO;
using System.Linq;
using System.Text;
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
            p.outputStream.WriteLine($"<p>URL: {p.http_url}</p>");
            p.outputStream.WriteLine("<form method=\"post\" action=\"/form\" enctype=\"multipart/form-data\">");
            p.outputStream.WriteLine("<input type=\"file\" id=\"data\" name=\"filename\">");
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
                Console.WriteLine("content length converted:" + contentLength);
                char[] buffer = new char[contentLength];
                inputData.ReadBlock(buffer, 0, contentLength);
                
                String data = new String(buffer);
                int myIndex = data.IndexOf("Content-Type");
                String newData = data.Substring(myIndex + 24, contentLength - myIndex - 24);
                
                int newLength = newData.Length;
                Console.WriteLine("New content length: " + newLength);
                
                int myNewIndex = newData.IndexOf("------");
                
                String newDatav2 = newData.Substring(myNewIndex , newLength - myNewIndex );

                //Trimmed data is just the binary data for the image 
                String trimmedData = newData.Replace(newDatav2, "");
                trimmedData.Trim();
                
                
                byte[] bytes = Encoding.UTF8.GetBytes(trimmedData);
                // This code can convert the text binary into real binary to translate into an image 
                foreach (byte b in bytes)
                {
                    string binary = Convert.ToString(b, 2);
                    Console.WriteLine(binary);
                }                
                
                // Console.WriteLine("Data start ---------------------------------------------------------------------------------------------------------");
                // Console.WriteLine(trimmedData);
                // Console.WriteLine("Data end---------------------------------------------------------------------------------------------------------");
            }
             else
            {
                p.outputStream.WriteLine("<html><body><h1>Error</h1>");
                p.outputStream.WriteLine("<p>Content-Length header is missing.</p>");
            }
        }
    }
}