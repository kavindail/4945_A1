using HttpMultipartParser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4945_A1
{
    public class FileUploadServlet : Servlet
    {
        public override void doGet(HttpRequest request, HttpResponse response)
        {
            Console.WriteLine("GET request: {0}", request.HttpUrl);
            response.WriteSuccess();
            response.OutputStream.WriteLine("<!DOCTYPE html>");
            response.OutputStream.WriteLine("<html>");
            response.OutputStream.WriteLine("<head><title>Test Server</title></head>");
            response.OutputStream.WriteLine("<body>");
            response.OutputStream.WriteLine($"<p>URL: {request.HttpUrl}</p>");
            response.OutputStream.WriteLine("<form method=\"post\" action=\"/form\" enctype=\"multipart/form-data\">");
            response.OutputStream.WriteLine("<input type=\"file\" id=\"filename\" name=\"filename\">");
            response.OutputStream.WriteLine("<input type=\"text\" id=\"myText\" name=\"myText\">");
            response.OutputStream.WriteLine("<input type=\"submit\">");
            response.OutputStream.WriteLine("</form>");
            response.OutputStream.WriteLine("</body>");
            response.OutputStream.WriteLine("</html>");
        }

        public override void doPost(HttpRequest request, HttpResponse response)
        {
            Console.WriteLine("POST request: {0}", request.HttpUrl);
            var contentLengthHeader = request.Headers.Get("Content-Length");
            var browserOrNativeApp = request.Headers.Get("User-Agent");

            Console.WriteLine(contentLengthHeader);
            if (!string.IsNullOrEmpty(contentLengthHeader))
            {
                int contentLength = Convert.ToInt32(contentLengthHeader);
                char[] buffer = new char[contentLength];
                request.InputStream.ReadBlock(buffer, 0, contentLength);
                string data = new string(buffer);
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    var parser = MultipartFormDataParser.Parse(stream);
                    var fileName = parser.GetParameterValue("myText");
                    var file = parser.Files.FirstOrDefault(f => f.Name == "filename");
                    if (file != null)
                    {
                        using (var reader = new StreamReader(file.Data))
                        {
                            string fileContent = reader.ReadToEnd();
                            writeToFile(fileName, fileContent, browserOrNativeApp, response);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No file uploaded with name 'filename'.");
                    }
                }
            }
            else
            {
                response.OutputStream.WriteLine("<html><body><h1>Error</h1>");
                response.OutputStream.WriteLine("<p>Content-Length header is missing.</p>");
            }
        }

        private void writeToFile(string filename, string fileContent, string userAgent, HttpResponse response)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, filename + ".txt")))
            {
                outputFile.WriteLine(fileContent);
            }

            getDirectoryListing(docPath, userAgent, response);
        }

        private void getDirectoryListing(string docPath, string userAgent, HttpResponse response)
        {
            string[] files = Directory.GetFiles(docPath);
            Array.Sort(files);

            if (userAgent.Contains("Mozilla"))
            {
                Console.WriteLine("browser connected");
                StringBuilder htmlList = new StringBuilder();
                htmlList.AppendLine("List of Files in this directory in Alphabetical Order:");
                htmlList.AppendLine("<ul>");

                foreach (string file in files)
                {
                    htmlList.AppendLine($"<li>{Path.GetFileName(file)}</li>");
                }
                htmlList.AppendLine("</ul>");

                response.WriteSuccess();
                response.OutputStream.WriteLine(htmlList.ToString());
            }
            else if (userAgent.Contains("MyCustomClientApp/1.0"))
            {
                Console.WriteLine("console app is connected");
                JArray jsonArray = new JArray(files.Select(Path.GetFileName));

                response.OutputStream.WriteLine(jsonArray.ToString());
            }
            else
            {
                Console.WriteLine("Unexpected error");
            }
        }
    }
}
