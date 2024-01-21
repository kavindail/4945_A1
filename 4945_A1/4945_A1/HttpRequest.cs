using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Sockets;

namespace _4945_A1
{
    public class HttpRequest
    {
        public TcpClient Socket { get; }
        public StreamReader InputStream { get; private set; }
        public NameValueCollection Headers { get; private set; }
        public string HttpMethod { get; private set; }
        public string HttpUrl { get; private set; }
        public string HttpProtocolVersionString { get; private set; }

        public HttpRequest(TcpClient socket)
        {
            Socket = socket;
            InputStream = new StreamReader(socket.GetStream());
            Headers = new NameValueCollection();
            ParseRequest();
            ReadHeaders();
        }

        private void ParseRequest()
        {
            string requestLine = InputStream.ReadLine();

            if (string.IsNullOrWhiteSpace(requestLine))
            {
                return; 
            }

            string[] tokens = requestLine.Split(' ');
            if (tokens.Length != 3)
            {
                return;
            }

            HttpMethod = tokens[0].ToUpperInvariant();
            HttpUrl = tokens[1];
            HttpProtocolVersionString = tokens[2];
        }

        private void ReadHeaders()
        {
            string line;
            while (!string.IsNullOrEmpty(line = InputStream.ReadLine()))
            {
                int separatorIndex = line.IndexOf(':');
                if (separatorIndex == -1)
                {
                    throw new Exception($"Invalid HTTP header line: {line}");
                }

                string name = line.Substring(0, separatorIndex).Trim();
                string value = line.Substring(separatorIndex + 1).Trim();
                Headers[name] = value;
            }
        }
    }
}
