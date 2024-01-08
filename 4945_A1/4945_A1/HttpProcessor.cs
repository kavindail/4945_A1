using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Specialized;
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
        int next_char;
        string data = "";
        while (true) {
            next_char = inputStream.ReadByte();
            if (next_char == '\n') { break; }
            if (next_char == '\r') { continue; }
            if (next_char == -1) { Thread.Sleep(1); continue; };
            data += Convert.ToChar(next_char);
        }            
        return data;
    }

    public void process() {
        inputStream = new BufferedStream(socket.GetStream());
        outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
        try {

            parseRequest();
            readHeaders();
           
            if (http_method.Equals("GET")) {
                srv.handleGetRequest(this);
            } else if (http_method.Equals("POST")) {
                srv.handlePostRequest(this, new StreamReader(inputStream));
            }
            
            
        } catch (Exception e) {
            Console.WriteLine("Exception: " + e.ToString());
            writeFailure();
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
       
        // Console.WriteLine("Http method");
        // Console.WriteLine(http_method);
        // Console.WriteLine("Http Url");
        // Console.WriteLine(http_url);
        // Console.WriteLine("Http Url");
        // Console.WriteLine(http_protocol_versionstring);
        
        
    }

    public void readHeaders() {
        Console.WriteLine("readHeaders()");
        String line;
        while ((line = streamReadLine(inputStream)) != null) {
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