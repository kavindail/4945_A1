using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Threading;

namespace _4945_A1 {
    public class HttpProcessor {
        public TcpClient socket;        
        public HttpServlet srv;

        private StreamReader inputStream; 
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public NameValueCollection httpHeaders = new NameValueCollection();

        public HttpProcessor(TcpClient s, HttpServlet srv) {
            this.socket = s;
            this.srv = srv;                   
        }


        public void process() {
            inputStream = new StreamReader(socket.GetStream()); // Using the raw stream directly
            outputStream = new StreamWriter(socket.GetStream());
            parseRequest();
            readHeaders();
            
            if (http_method.Equals("GET")) {
                srv.handleGetRequest(this);

            } else if (http_method.Equals("POST")) {
                srv.handlePostRequest(this, inputStream); 
            }

            Console.WriteLine("Processing input stream" + inputStream);

            outputStream.Flush();
            outputStream.Close();
            socket.Close();
        }

        public void parseRequest()
        {
            String request = inputStream.ReadLine(); 
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3) {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];
        }

        public void readHeaders() {
            string line;
            while (true) {
                line = inputStream.ReadLine();

                if (string.IsNullOrEmpty(line)) {
                    break;
                }

                int separator = line.IndexOf(':');
                if (separator == -1) {
                    throw new Exception("invalid http header line: " + line);
                }

                string name = line.Substring(0, separator).Trim();
                string value = line.Substring(separator + 1).Trim();
                httpHeaders[name] = value;
            }
        }

        public void writeSuccess() {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Content-Type: text/html");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        public void writeFailure() {
            outputStream.WriteLine("HTTP/1.0 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }
    }
}
