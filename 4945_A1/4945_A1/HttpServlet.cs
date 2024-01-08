using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace _4945_A1 {
    public abstract class HttpServlet
    {
        protected int port;
        TcpListener listener;
        bool is_active = true;

        public HttpServlet(int port)
        {
            this.port = port;
        }

        public void listen()
        {
            listener = new TcpListener(port);
            listener.Start();

            while (is_active == true)
            {
                TcpClient client = listener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(client, this);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);
            }
        }

        public abstract void handleGetRequest(HttpProcessor p);
        public abstract void handlePostRequest(HttpProcessor p, StreamReader inputData);

    }
}