using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4945_A1
{
    public class ServerThread
    {
        private TcpClient client;
        private Servlet servlet;

        public ServerThread(TcpClient client, Servlet servlet)
        {
            this.client = client;
            this.servlet = servlet;
        }

        public void Run()
        {
            HttpRequest request = new HttpRequest(client);
            HttpResponse response = new HttpResponse(client);

            if (request.HttpMethod == "GET")
            {
                servlet.doGet(request, response);
            }
            else if (request.HttpMethod == "POST")
            {
                servlet.doPost(request, response);
            }

            response.OutputStream.Flush();
            response.OutputStream.Close();
            client.Close();
        }
    }
}
