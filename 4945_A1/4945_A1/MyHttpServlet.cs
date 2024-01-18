using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using HttpMultipartParser;

namespace _4945_A1
{
    public class MyHttpServlet : HttpServlet
    {
        public MyHttpServlet(int port) : base(port)
        {
        }

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
            p.outputStream.WriteLine("<input type=\"file\" id=\"filename\" name=\"filename\">");
            p.outputStream.WriteLine("<input type=\"text\" id=\"myText\" name=\"myText\">");
            p.outputStream.WriteLine("<input type=\"submit\">");
            p.outputStream.WriteLine("</form>");
            p.outputStream.WriteLine("</body>");
            p.outputStream.WriteLine("</html>");
        }


        public override void handlePostRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("POST request: {0}", p.http_url);
            string contentLengthHeader = p.httpHeaders.Get("Content-Length");

            string browserOrNativeApp = p.httpHeaders.Get("User-Agent");


            Console.WriteLine(contentLengthHeader);
            if (contentLengthHeader != null)

            {
                int contentLength = Convert.ToInt32(contentLengthHeader);
                char[] buffer = new char[contentLength];
                inputData.ReadBlock(buffer, 0, contentLength);
                int count = 0;
                String data = new String(buffer);
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                MemoryStream stream = new MemoryStream(byteArray);

                var parser = MultipartFormDataParser.Parse(stream);

                var fileName = parser.GetParameterValue("myText");
                var fileContents = parser.GetParameterValue("filename");


                var file = parser.Files.FirstOrDefault(f => f.Name == "filename");
                if (file != null)
                {
                    using (var reader = new StreamReader(file.Data))
                    {
                        string fileContent = reader.ReadToEnd();
                        writeToFile(fileName, fileContent, browserOrNativeApp,p);
                    }
                }
                else
                {
                    Console.WriteLine("No file uploaded with name 'filename'.");
                }
            }
            else
            {
                p.outputStream.WriteLine("<html><body><h1>Error</h1>");
                p.outputStream.WriteLine("<p>Content-Length header is missing.</p>");
            }
        }

        public void writeToFile(String filename, String myFile, String userAgent, HttpProcessor p)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, filename + ".txt")))
            {
                outputFile.WriteLine(myFile);
                getDirectoryListing(docPath, userAgent,p);
            }
        }

        public void getDirectoryListing(String docPath, String userAgent, HttpProcessor p)
        {
            string[] files = Directory.GetFiles(docPath);
            Array.Sort(files);

            if (userAgent.Contains("Mozilla"))
            {
                Console.WriteLine("browser connected");
                StringBuilder htmlList = new StringBuilder();
                htmlList.AppendLine("List of Files in this directory in Alphabetical Order: ");
                htmlList.AppendLine("<ul>");
                
                foreach (string file in files)
                {
                    htmlList.AppendLine($"<li>{Path.GetFileName(file)}</li>");
                }

                htmlList.AppendLine("</ul>");
                
                // Send the HTML list as part of the HTTP response
                p.writeSuccess();
                p.outputStream.WriteLine(htmlList.ToString());
                
            }
            else if(userAgent.Contains("MyCustomClientApp/1.0"))
            {
                Console.WriteLine("console app is connected");
                JArray jsonArray = new JArray();

                foreach (string file in files)
                {
                    jsonArray.Add(Path.GetFileName(file));
                }

                string jsonString = jsonArray.ToString();
                p.outputStream.WriteLine(jsonString);
            }
            else
            {
                Console.WriteLine("Unexpected error");
            }
        }
    }
}