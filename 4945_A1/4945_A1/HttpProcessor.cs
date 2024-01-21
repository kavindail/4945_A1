using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Threading;

namespace _4945_A1 {
    public class HttpProcessor {
        private TcpClient socket;
        private Servlet servlet;

        public HttpProcessor(TcpClient socket, Servlet servlet)
        {
            this.socket = socket;
            this.servlet = servlet;
        }


        public void Process()
        {
            try
            {
                HttpRequest request = new HttpRequest(socket);
                HttpResponse response = new HttpResponse(socket);

                if (request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    servlet.doGet(request, response);
                }
                else if (request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    servlet.doPost(request, response);
                }
                else {}

                response.OutputStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing request: " + ex.Message);
            }
            finally
            {
                if (socket != null)
                {
                    socket.Close();
                }
            }
        }
    }
}
