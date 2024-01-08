using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Threading;


namespace _4945_A1
{
public class HttpProcessor {
    public TcpClient socket;        
    public HttpServlet srv;

    private Stream inputStream;
    public StreamWriter outputStream;

    public String http_method;
    public String http_url;
    public String http_protocol_versionstring;
    public NameValueCollection httpHeaders = new NameValueCollection();

    public HttpProcessor(TcpClient s, HttpServlet srv) {
        this.socket = s;
        this.srv = srv;                   
    }

    private string streamReadLine(Stream inputStream) {
        StringBuilder data = new StringBuilder();
        int next_char;
        bool lastCharWasCR = false;

        while (true) {
            next_char = inputStream.ReadByte();
            if (next_char == '\r') { // Carriage return
                lastCharWasCR = true;
            } else if (next_char == '\n' && lastCharWasCR) { // Line feed following carriage return
                break;
            } else {
                lastCharWasCR = false;
                data.Append(Convert.ToChar(next_char));
            }
        }

        string result = data.ToString();
        // Check if the result is an empty line, indicating the end of headers
        if (string.IsNullOrWhiteSpace(result)) {
            return null;
        }

        return result;
    }


    
    public void process() {
        inputStream = new BufferedStream(socket.GetStream());
        outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));

            parseRequest();
            readHeaders();
           
            if (http_method.Equals("GET")) {
                srv.handleGetRequest(this);
                
            }
            else if (http_method.Equals("POST")) {
                srv.handlePostRequest(this, new StreamReader(inputStream));
            }
            
            
        outputStream.Flush();
        inputStream.Close();
        outputStream.Close();
        socket.Close();
    }

    public void parseRequest() {
        String request = streamReadLine(inputStream);
        string[] tokens = request.Split(' ');
        if (tokens.Length != 3) {
            throw new Exception("invalid http request line");
        }
        http_method = tokens[0].ToUpper();
        http_url = tokens[1];
        http_protocol_versionstring = tokens[2];
    }

    public void readHeaders() {
        String line;
        while ((line = streamReadLine(inputStream)) != null ) {
            if (line.Equals("")) {
                return;
            }
            int separator = line.IndexOf(':');
            if (separator == -1) {
                throw new Exception("invalid http header line: " + line);
            }
            String name = line.Substring(0, separator);
            int pos = separator + 1;
            while ((pos < line.Length) && (line[pos] == ' ')) {
                pos++; 
            }
            string value = line.Substring(pos, line.Length - pos);
            httpHeaders[name] = value;
            
        }
        Console.WriteLine("Content-Length: " + httpHeaders["Content-Length"]);
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