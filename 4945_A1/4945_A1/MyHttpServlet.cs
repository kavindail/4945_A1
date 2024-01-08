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
            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
            p.outputStream.WriteLine("<input type=submit name=submit value=submit>");
            p.outputStream.WriteLine("</form>");     
        }


        public override void handlePostRequest(HttpProcessor p, StreamReader inputData)
        {
            
            Console.WriteLine("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();
        
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
            Console.WriteLine("Data start");
            Console.WriteLine(data);
            Console.WriteLine("Data end");

        }
    }
}